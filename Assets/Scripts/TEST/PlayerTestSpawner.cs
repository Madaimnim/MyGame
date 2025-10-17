using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class PlayerTestSpawner : MonoBehaviour
{
    [Header("���a�ͦ���m")]
    public Vector3 spawnPosition = Vector3.zero;

    private PlayerStateSystem _playerStateSystem;

    private void Awake()
    {
        // �إ�PlayerStateSystem�A�o�̥i�ھڴ��ջݨD�ǤJnull�ΦۭqGameManager
        _playerStateSystem = new PlayerStateSystem(null);
        _playerStateSystem.Initialize();
    }

    private void Start()
    {
        StartCoroutine(LoadAndSpawnPlayers());
    }

    private IEnumerator LoadAndSpawnPlayers()
    {
        // ���J���a���
        string address = "PlayerStatData";
        AsyncOperationHandle<PlayerStatData> handle = Addressables.LoadAssetAsync<PlayerStatData>(address);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _playerStateSystem.SetPlayerStatsRuntimeDtny(handle.Result);

            // �ͦ��Ҧ����a
            foreach (var kvp in _playerStateSystem.PlayerStatsRuntimeDtny)
            {
                int playerId = kvp.Key;
                Player player = CreatePlayer(playerId, kvp.Value);
                player.transform.position = spawnPosition + new Vector3(playerId * 2, 0, 0); // �����קK���|

                // ���աG�i��ܵ�AI��PlayerInputManager����
                // player.SetInputProvider(new AIComponent());
                // ��
                // player.SetInputProvider(FindObjectOfType<PlayerInputController>());
            }
        }
        else
        {
            Debug.LogError("���JPlayerStatData����");
        }
    }

    private Player CreatePlayer(int id, PlayerStatsRuntime stats)
    {
        // �o�̰��]��PrefabConfig�i�ΡA�_�h�Цۦ���wPrefab
        GameObject prefab = Resources.Load<GameObject>("PlayerPrefab");
        GameObject go = Instantiate(prefab);
        Player player = go.GetComponent<Player>();
        player.Initialize(stats);
        return player;
    }
}