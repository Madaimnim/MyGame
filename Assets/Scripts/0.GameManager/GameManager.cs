using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameManager : MonoBehaviour
{
    #region 定義
    public static GameManager Instance { get; private set; }

    [Header("全局角色材質")]
    public Material normalMaterial;
    public Material flashMaterial;

    public bool IsAllDataLoaded => IsPlayerDataLoaded && IsEnemyDataLoaded;
    public bool IsPlayerDataLoaded { get; private set; } = false;
    public bool IsEnemyDataLoaded { get; private set; } = false;
    #endregion

    #region 生命週期方法
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
    }
    private IEnumerator Start() {
        yield return Addressables.InitializeAsync();

        yield return LoadPlayerStatsList();
        yield return LoadEnemyStatsList();
    }
    #endregion
    private IEnumerator LoadPlayerStatsList() {
        string address = "Assets/GameData/PlayerStatData.asset";
        AsyncOperationHandle<PlayerStatData> handle = Addressables.LoadAssetAsync<PlayerStatData>(address);
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            PlayerStateManager.Instance.SetPlayerStatesDtny(handle.Result);
            IsPlayerDataLoaded = true;
        }
    }

    private IEnumerator LoadEnemyStatsList() {
        string address = "Assets/GameData/EnemyStatData.asset";
        AsyncOperationHandle<EnemyStatData> handle = Addressables.LoadAssetAsync<EnemyStatData>(address);
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            EnemyStateManager.Instance.SetEnemyStatesDtny(handle.Result);
            IsEnemyDataLoaded = true;
        }

    }

    public IEnumerator WaitForDataReady() {
        // 持續等待，直到所有資料載入完成
        while (!IsAllDataLoaded)
        {
            yield return null; // 等待下一幀
        }
        Debug.Log("完成全部資料加載");
    }

}
