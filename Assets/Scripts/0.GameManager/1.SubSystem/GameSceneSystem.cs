using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Collections;


//只負責場景切換流程
//
public class GameSceneSystem : GameSubSystem
{
    private ICoroutineRunner _runner;
    private SceneConfig _sceneConfig;
    private string _currentSceneAddress;
    private bool _isLoadingScene = false;
    private AsyncOperationHandle<SceneInstance>? _currentSceneHandle;

    //新增事件給 UI 使用
    public event System.Action<float> OnSceneLoadProgress; // 0~1
    public event System.Action<string> OnSceneLoaded;           //======此事件不可依賴場景內物件，只能依賴全景移動的物件和方法======

    public GameSceneSystem(GameManager gm) : base(gm) {
        _runner = new CoroutineRunnerAdapter(gm);
    }

    public override void Initialize() {
        _sceneConfig = GameSettingManager.Instance.SceneConfig;
    }
    public void LoadSceneByKey(string key) {
        string address = _sceneConfig.GetAddress(key);
        if (!string.IsNullOrEmpty(address))
            _runner.StartCoroutine(LoadScene(address));
    }
    private IEnumerator LoadScene(string address) {
        if (address == _currentSceneAddress) yield break;
        if (_isLoadingScene) yield break;
        _isLoadingScene = true;
        yield return new WaitUntil(() => GameManager.IsAllDataLoaded);
        yield return FadeManager.Instance.FadeOut();
        yield return new WaitForSecondsRealtime(FadeManager.Instance.fadeDuration);
        UIManager.Instance.SetLoadingUI(true);

        //Single模式本來就會卸載場景
        //// 卸載舊場景
        //if (_currentSceneHandle.HasValue)
        //{
        //    yield return Addressables.UnloadSceneAsync(_currentSceneHandle.Value);
        //    _currentSceneHandle = null;
        //}

        // 載入新場景
        AsyncOperationHandle<SceneInstance> handle =Addressables.LoadSceneAsync(address, UnityEngine.SceneManagement.LoadSceneMode.Single, activateOnLoad: true);
        
        while (!handle.IsDone)
        {
            //發事件
            OnSceneLoadProgress?.Invoke(handle.PercentComplete);
            yield return null;
        }

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            CameraManager.Instance.CleanupExtraMainCameras();
            _currentSceneHandle = handle;
            _currentSceneAddress = address;

            //發事件
            OnSceneLoadProgress?.Invoke(1f);
            yield return new WaitForSeconds(0.5f);
            UIManager.Instance.SetLoadingUI(false);

            yield return FadeManager.Instance.FadeIn();

            //發事件
            OnSceneLoaded?.Invoke(address);
        }
        else
        {
            Debug.LogError($"場景載入失敗: {address}");
        }
        _isLoadingScene = false;
    }

}
