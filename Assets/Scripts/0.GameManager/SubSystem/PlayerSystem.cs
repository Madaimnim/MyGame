using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public sealed class PlayerSystem : SubSystemBase
{
    private Transform _playerBattleParent;      // 取代原 playerParent
    private Transform _playerUiParent;          // 取代原 playerPreviewParent
    private Vector3 _stageSpawnPosition;  // 取代原 stageSpawnPosition
    private ICoroutineRunner _runner;     // 用來跑協程

    private UIManager _uiManager;
    private IPlayerFactory _playerFactory;
    private ISpawnEffect _spawnEffect;

    private readonly Dictionary<int, PlayerStatsRuntime> _playerStatsDtny = new();
    public readonly HashSet<int> UnlockedIds = new();
    public readonly Dictionary<int, GameObject> DeployedPlayers = new(); // 原 deployedPlayersGameObjectDtny
    public readonly Dictionary<int, GameObject> UiPlayers = new();       // 原 uiPlayersGameObjectDtny

    public PlayerSystem(GameManager gm) : base(gm) { }
    public override void Initialize() {
        _playerBattleParent = GameManager.PlayerBattleParent;   // 假設你在 PrefabConfig 設定
        _playerUiParent = GameManager.PlayerUiParent;
        _stageSpawnPosition = GameManager.PlayerSpawnPosition;
        _runner = new CoroutineRunnerAdapter(GameManager);
        _uiManager = UIManager.Instance;

        _playerFactory = new DefaultPlayerFactory();
        _spawnEffect = new DropBounceSpawnEffect();
    }

    public override void Update(float deltaTime) { }
    public override void Shutdown() { }



    public void SetPlayerStatsRuntimes(PlayerStatData playerStatData) {
        _playerStatsDtny.Clear();
        foreach (var stat in playerStatData.playerStatsTemplateList)
        {
            if (!IDValidator.IsPlayerID(stat.StatsData.Id)) { Debug.LogError($"[PlayerSystem] Id {stat.StatsData.Id} 不合法"); continue; }
            if (_playerStatsDtny.ContainsKey(stat.StatsData.Id)) { Debug.LogError($"[PlayerSystem] 重複 Id {stat.StatsData.Id}"); continue; }
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
        if (!IDValidator.IsPlayerID(playerId)) { Debug.LogError($" UnlockPlayer: {playerId} 非法"); return false; }
        return UnlockedIds.Add(playerId);
    }

    public void SpawnBothPlayers(int playerId) {
        SpawnBattlePlayer(playerId);
        SpawnUIPlayer(playerId);
    }

    public GameObject SpawnBattlePlayer(int playerId) {
        if (!_playerStatsDtny.TryGetValue(playerId, out var rt)) return null;

        var go = _playerFactory.CreatPlayer(rt, _playerBattleParent);
        if (go != null) DeployedPlayers[playerId] = go;
        rt.BattlePlayerObject = go;
        return go;
    }
    public GameObject SpawnUIPlayer(int playerId) {
        if (!_playerStatsDtny.TryGetValue(playerId, out var rt)) return null;

        var go = _playerFactory.CreatPlayer(rt, _playerUiParent);
        if (go != null)
        {
            UiPlayers[playerId] = go;

            // 先保留你對 UIManager 的直接寫入（之後可改事件）
            if (_uiManager != null)
                _uiManager.activeUIPlayersDtny[playerId] = go;
        }
        rt.UiPlayerObject = go;
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
        //Todo 範圍隨機落下
        float offsetY = -1f;
        var pos = _stageSpawnPosition;

        foreach (var kv in DeployedPlayers)
        {
            var go = kv.Value;
            if (!go) continue;

            go.SetActive(true);
            _runner?.StartCoroutine(_spawnEffect.Play(go, pos));
            pos.y += offsetY;
        }
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