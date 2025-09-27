using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public sealed class PlayerSystem : SubSystemBase
{
    private Transform _playerBattleParent;      // ���N�� playerParent
    private Transform _playerUiParent;          // ���N�� playerPreviewParent
    private Vector3 _stageSpawnPosition;  // ���N�� stageSpawnPosition
    private ICoroutineRunner _runner;     // �ΨӶ]��{

    private UIManager _uiManager;
    private IPlayerFactory _playerFactory;
    private ISpawnEffect _spawnEffect;

    private readonly Dictionary<int, PlayerStatsRuntime> _playerStatsDtny = new();
    public readonly HashSet<int> UnlockedIds = new();
    public readonly Dictionary<int, GameObject> DeployedPlayers = new(); // �� deployedPlayersGameObjectDtny
    public readonly Dictionary<int, GameObject> UiPlayers = new();       // �� uiPlayersGameObjectDtny

    public PlayerSystem(GameManager gm) : base(gm) { }
    public override void Initialize() {
        _playerBattleParent = GameManager.PlayerBattleParent;   // ���]�A�b PrefabConfig �]�w
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
            if (!IDValidator.IsPlayerID(stat.StatsData.Id)) { Debug.LogError($"[PlayerSystem] Id {stat.StatsData.Id} ���X�k"); continue; }
            if (_playerStatsDtny.ContainsKey(stat.StatsData.Id)) { Debug.LogError($"[PlayerSystem] ���� Id {stat.StatsData.Id}"); continue; }
            _playerStatsDtny[stat.StatsData.Id] = new PlayerStatsRuntime(stat);
        }
    }

    public PlayerStatsRuntime GetStatsRuntime(int id) {
        if (_playerStatsDtny.TryGetValue(id, out var s)) return s;

        Debug.LogError($"[PlayerSystem] �䤣�� Id {id} �����A");
        return null;
    }
    public bool TryGetStatsRuntime(int id, out PlayerStatsRuntime s) => _playerStatsDtny.TryGetValue(id, out s);

    public bool UnlockPlayer(int playerId) {
        if (!IDValidator.IsPlayerID(playerId)) { Debug.LogError($" UnlockPlayer: {playerId} �D�k"); return false; }
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

            // ���O�d�A�� UIManager �������g�J�]����i��ƥ�^
            if (_uiManager != null)
                _uiManager.activeUIPlayersDtny[playerId] = go;
        }
        rt.UiPlayerObject = go;
        return go;
    }


    public void SetupPlayerSkillSlot(int playerId, int slotIndex, int skillId) {
        if (!_playerStatsDtny.TryGetValue(playerId, out var rt))
        {
            Debug.LogError($"[SetupPlayerSkillSlot] �䤣�쪱�a ID:{playerId}");
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
        //Todo �d���H�����U
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