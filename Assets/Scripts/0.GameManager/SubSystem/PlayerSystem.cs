using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerSystem
{
    public sealed class Config
    {
        public Transform BattleParent;      // 取代原 playerParent
        public Transform UiParent;          // 取代原 playerPreviewParent
        public Vector3 StageSpawnPosition;  // 取代原 stageSpawnPosition
        public ICoroutineRunner Runner;     // 用來跑協程
        public UIManager UiManager;         // 先保留（之後可改事件）
    }

    private readonly Config _cfg;

    private readonly Dictionary<int, PlayerStatsRuntime> _playerStatsDtny = new();
    public readonly HashSet<int> UnlockedIds = new();
    public readonly Dictionary<int, GameObject> DeployedPlayers = new(); // 原 deployedPlayersGameObjectDtny
    public readonly Dictionary<int, GameObject> UiPlayers = new();       // 原 uiPlayersGameObjectDtny

    public PlayerSystem(Config cfg) {
        _cfg = cfg;
    }

    public void SetPlayerStatsRuntimes(PlayerStatData playerStatData) {
        _playerStatsDtny.Clear();
        foreach (var stat in playerStatData.playerStatsTemplateList)
        {
            if (!IDValidator.IsPlayerID(stat.StatsData.Id)) {Debug.LogError($"[PlayerSystem] Id {stat.StatsData.Id} 不合法");  continue;}
            if (_playerStatsDtny.ContainsKey(stat.StatsData.Id)){Debug.LogError($"[PlayerSystem] 重複 Id {stat.StatsData.Id}");continue;}
            _playerStatsDtny[stat.StatsData.Id] = new PlayerStatsRuntime(stat);
        }
    }

    public PlayerStatsRuntime GetStatsRuntime(int id) {
        if (_playerStatsDtny.TryGetValue(id, out var s)) return s;
        
        Debug.LogError($"[PlayerSystem] 找不到 Id {id} 的狀態");
        return null;
    }
    public bool TryGetStatsRuntime(int id, out PlayerStatsRuntime s) => _playerStatsDtny.TryGetValue(id, out s);

    public bool UnlockPlayer(int playerId) {
        if (!IDValidator.IsPlayerID(playerId)){Debug.LogError($" UnlockPlayer: {playerId} 非法");return false;}
        return UnlockedIds.Add(playerId);
    }

    public void SpawnBothPlayers(int playerId) {
        SpawnBattlePlayer(playerId);
        SpawnUIPlayer(playerId);
    }

    public GameObject SpawnBattlePlayer(int playerId) {
        if (!_playerStatsDtny.TryGetValue(playerId, out var rt)) return null;

        var go = SpawnPlayerObject(rt, _cfg.BattleParent);
        if (go != null) DeployedPlayers[playerId] = go;
        rt.BattlePlayerObject = go;
        return go;
    }
    public GameObject SpawnUIPlayer(int playerId) {
        if (!_playerStatsDtny.TryGetValue(playerId, out var rt)) return null;

        var go = SpawnPlayerObject(rt, _cfg.UiParent);
        if (go != null)
        {
            UiPlayers[playerId] = go;

            // 先保留你對 UIManager 的直接寫入（之後可改事件）
            if (_cfg.UiManager != null)
                _cfg.UiManager.activeUIPlayersDtny[playerId] = go;
        }
        rt.UiPlayerObject = go;
        return go;
    }

    private GameObject SpawnPlayerObject(PlayerStatsRuntime rt, Transform parent) {
        if (rt.VisualData.CharPrefab == null) return null;

        var go = Object.Instantiate(rt.VisualData.CharPrefab, Vector3.zero, Quaternion.identity, parent);
        go.SetActive(false);
        go.transform.localPosition = Vector3.zero;

        var player = go.GetComponent<Player>();
        if (player != null) player.Initialize(rt);
        else Debug.LogError($"{go.name} 沒有 Player 組件");

        return go;
    }

    public void SetupPlayerSkillSlot(int playerId, int slotIndex, int skillId) {
        if (!_playerStatsDtny.TryGetValue(playerId, out var rt))
        {
            Debug.LogError($"[SetupPlayerSkillSlot] 找不到玩家 ID:{playerId}");
            return;
        }

        rt.SetSkillAtSlot(slotIndex, skillId);
        var skill = rt.GetSkillDataRuntimeForId(skillId);

        if (DeployedPlayers.TryGetValue(playerId, out var battleGo))
            battleGo.GetComponent<Player>()?.SetSkillSlot(slotIndex, skill);

        if (UiPlayers.TryGetValue(playerId, out var uiGo))
            uiGo.GetComponent<Player>()?.SetSkillSlot(slotIndex, skill);
    }

    public void ActivateAllPlayer() {
        float offsetY = -1f;
        var pos = _cfg.StageSpawnPosition;

        foreach (var kv in DeployedPlayers)
        {
            var go = kv.Value;
            if (!go) continue;

            go.SetActive(true);
            _cfg.Runner?.StartCoroutine(SpawnWithDrop(go, pos));
            pos.y += offsetY;
        }
    }

    private IEnumerator SpawnWithDrop(GameObject player, Vector3 groundPos) {
        Vector3 startPos = groundPos + new Vector3(0, 10f, 0);
        player.transform.position = startPos;

        float t = 0f;
        const float dropDuration = 0.5f;

        while (t < 1f)
        {
            t += Time.deltaTime / dropDuration;
            player.transform.position = Vector3.Lerp(startPos, groundPos, t * t);
            yield return null;
        }

        Vector3 bouncePos = groundPos + new Vector3(0, 0.3f, 0);
        t = 0f;
        const float bounceDuration = 0.2f;

        while (t < 1f)
        {
            t += Time.deltaTime / bounceDuration;
            player.transform.position = Vector3.Lerp(groundPos, bouncePos, Mathf.Sin(t * Mathf.PI));
            yield return null;
        }

        player.transform.position = groundPos;
    }
    public void DeactivateAllPlayer() {
        foreach (var kv in DeployedPlayers)
        {
            var go = kv.Value;
            if (!go) continue;

            go.SetActive(false);
            go.transform.position = Vector2.zero;
        }
    }

    public GameObject GetBattlePlayerObject(int playerId)
        => _playerStatsDtny.TryGetValue(playerId, out var rt) ? rt.BattlePlayerObject : null;

    public GameObject GetUIPlayerObject(int playerId)
        => _playerStatsDtny.TryGetValue(playerId, out var rt) ? rt.UiPlayerObject : null;
}
