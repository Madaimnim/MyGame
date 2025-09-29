using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;
using System.Linq;

public sealed class PlayerStateSystem : SubSystemBase
{
    public IReadOnlyDictionary<int, PlayerStatsRuntime> PlayerStatsRuntimeDtny => _playerStatsRuntimeDtny;
    public IReadOnlyCollection<int> UnlockedIdList => _unlockedIdList.ToList();
    public IReadOnlyDictionary<int, GameObject> BattleObjects => _spawnSystem.BattleObjects;
    public SkillSystem SkillSystem => _skillSystem;

    private readonly Dictionary<int, PlayerStatsRuntime> _playerStatsRuntimeDtny = new();
    private readonly HashSet<int> _unlockedIdList = new();
    private PlayerSpawnSystem _spawnSystem;
    private SkillSystem _skillSystem;

    // 事件
    public event Action<int> OnPlayerUnlocked;
    public event Action<int, PlayerStatsRuntime> OnPlayerSpawned;

    public PlayerStateSystem(GameManager gm) : base(gm) { }

    public override void Initialize() {
        _spawnSystem = new PlayerSpawnSystem(
               new DefaultPlayerFactory(),
               new DropBounceSpawnEffect(),
               GameManager.PlayerBattleParent,
               new CoroutineRunnerAdapter(GameManager)
           );
        _skillSystem = new SkillSystem(this);
    }

    public override void Update(float deltaTime) { }
    public override void Shutdown() { }

    public void SetPlayerStatsRuntimeDtny(PlayerStatData playerStatData) {
        _playerStatsRuntimeDtny.Clear();
        foreach (var stat in playerStatData.playerStatsTemplateList)
        {
            if (!IDValidator.IsPlayerID(stat.StatsData.Id)) continue;
            _playerStatsRuntimeDtny[stat.StatsData.Id] = new PlayerStatsRuntime(stat);
            var Runtime = _playerStatsRuntimeDtny[stat.StatsData.Id];
            Runtime.Initialize(_skillSystem);
        }
    }
    public bool UnlockPlayer(int playerId) {
        if (!IDValidator.IsPlayerID(playerId)) return false;
        bool added = _unlockedIdList.Add(playerId);
        //發事件
        if (added) OnPlayerUnlocked?.Invoke(playerId);
        return added;
    }
    public GameObject SpawnBattlePlayer(int playerId) {
        if (!_playerStatsRuntimeDtny.TryGetValue(playerId, out var rt)) return null;
        var go = _spawnSystem.SpawnBattlePlayer(rt);
        //發事件
        OnPlayerSpawned?.Invoke(playerId, rt);
        return go;
    }

    public void InitialGameStartPlayerSpawn() {
        UnlockPlayer(1001);
        SpawnBattlePlayer(1001);
        PlayerStatsRuntimeDtny[1001].AddUnlockSkillList(1);
        PlayerStatsRuntimeDtny[1001].AddUnlockSkillList(2);
        SkillSystem.EquipSkill(1001, 0, 1);

        UnlockPlayer(1002);
        SpawnBattlePlayer(1002);
        //_playerStateSystem.PlayerStatsRuntimeDtny[1002].AddUnlockSkillList(1);
        //_playerStateSystem.SkillSystem.EquipSkill(1002, 0, 1);

        DeactivateAllPlayer();
    }
    public void ActivateAllPlayer() => _spawnSystem.ActivateAll(PlayerSpawnPoint.Instance.transform.position);
    public void DeactivateAllPlayer() => _spawnSystem.DeactivateAll();

}
