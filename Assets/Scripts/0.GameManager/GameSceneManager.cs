using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GameSceneManager: MonoBehaviour
{
    #region 公開定義
    //public Button BattleSceneButton;
    //public Button PreparationSceneButton;
    public Button GameStartButton;
    #endregion

    public static GameSceneManager Instance { get; private set; }

    #region 私有定義
    public string gameStartScene;
    public string preparationScene;
    public string battleScene ;

    private string currentSceneName;
    private bool isLoadingScene = false;
    #endregion

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        GameStartButton.gameObject.SetActive(false);
    }

    private IEnumerator Start() {
        yield return StartCoroutine(GameManager.Instance.WaitForDataReady());
        GameStartButton.gameObject.SetActive(true);
    }

    #region 公開方法 LoadSceneGameStart()
    public void LoadSceneGameStart() {
        StartCoroutine(LoadScene(gameStartScene));
    }
    #endregion
    #region 公開方法 LoadSceneBattle()
    public void LoadSceneBattle() {
        StartCoroutine(LoadScene(battleScene));
    }
    #endregion
    #region 公開方法 LoadScenePreparation()
    public void LoadScenePreparation() {
        StartCoroutine(LoadScene(preparationScene));
    }
    #endregion

    #region 私有協程LoadScene(string sceneName)
    //順序加載場景
    private IEnumerator LoadScene(string sceneName) {
        //避免加載相同場景
        if (sceneName == currentSceneName)
        {
            Debug.LogWarning($"⚠️ 嘗試加載相同場景 ({sceneName})，加載取消。");
            yield break;
        }

        // 確保不會並行加載
        if (isLoadingScene)
        {
            Debug.LogWarning($"⚠️ 場景加載中，請勿重複加載 ({sceneName})");
            yield break;
        }
        isLoadingScene = true;

        // 確保 GameManager 的資料已加載完畢
        if (!GameManager.Instance.IsAllDataLoaded)
        {
            Debug.LogError("❌ 資料未完成加載，等待中...");
            yield return new WaitUntil(() => GameManager.Instance.IsAllDataLoaded);
        }

        // 執行淡出效果
        yield return FadeManager.Instance.FadeOut();
        yield return new WaitForSecondsRealtime(FadeManager.Instance.fadeDuration);
        
        //加載場景，直到加載完成才進行下一行
        AsyncOperation newScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        yield return new WaitUntil(() => newScene.isDone);

        //紀錄當前場景名稱，並解除Loading鎖定
        currentSceneName = sceneName;
        isLoadingScene = false; 

        // 執行淡入效果
        yield return FadeManager.Instance.FadeIn();
    }
    #endregion
}
