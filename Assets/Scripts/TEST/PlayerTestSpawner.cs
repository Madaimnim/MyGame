using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class PlayerTestSpawner : MonoBehaviour
{
    [Header("玩家生成位置")]
    public Vector3 spawnPosition = Vector3.zero;

    private PlayerStateSystem _playerStateSystem;

    private void Awake()
    {
        // 建立PlayerStateSystem，這裡可根據測試需求傳入null或自訂GameManager
        _playerStateSystem = new PlayerStateSystem(null);
        _playerStateSystem.Initialize();
    }

    private void Start()
    {
        StartCoroutine(LoadAndSpawnPlayers());
    }

    private IEnumerator LoadAndSpawnPlayers()
    {
        // 載入玩家資料
        string address = "PlayerStatData";
        AsyncOperationHandle<PlayerStatData> handle = Addressables.LoadAssetAsync<PlayerStatData>(address);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _playerStateSystem.SetPlayerStatsRuntimeDtny(handle.Result);

            // 生成所有玩家
            foreach (var kvp in _playerStateSystem.PlayerStatsRuntimeDtny)
            {
                int playerId = kvp.Key;
                Player player = CreatePlayer(playerId, kvp.Value);
                player.transform.position = spawnPosition + new Vector3(playerId * 2, 0, 0); // 偏移避免重疊

                // 測試：可選擇給AI或PlayerInputManager控制
                // player.SetInputProvider(new AIComponent());
                // 或
                // player.SetInputProvider(FindObjectOfType<PlayerInputController>());
            }
        }
        else
        {
            Debug.LogError("載入PlayerStatData失敗");
        }
    }

    private Player CreatePlayer(int id, PlayerStatsRuntime stats)
    {
        // 這裡假設有PrefabConfig可用，否則請自行指定Prefab
        GameObject prefab = Resources.Load<GameObject>("PlayerPrefab");
        GameObject go = Instantiate(prefab);
        Player player = go.GetComponent<Player>();
        player.Initialize(stats);
        return player;
    }
}