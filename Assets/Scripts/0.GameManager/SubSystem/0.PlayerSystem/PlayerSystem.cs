using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;

public sealed class PlayerSystem : SubSystemBase
{
    private readonly Dictionary<int, PlayerStatsRuntime> _playerStatsRuntimes = new();
    private readonly HashSet<int> _unlockedIds = new();

    private PlayerSpawner _spawner;
    public SkillSystem SkillSystem => _skillSystem;
    private SkillSystem _skillSystem;

    public IReadOnlyDictionary<int, PlayerStatsRuntime> PlayerStatsRuntimes => _playerStatsRuntimes;
    public IReadOnlyCollection<int> UnlockedIds => _unlockedIds;

    public IReadOnlyDictionary<int, GameObject> BattleObjects => _spawner.BattleObjects;

    // 事件
    public event Action<int> OnPlayerUnlocked;
    public event Action<int, PlayerStatsRuntime> OnPlayerSpawned;

    public PlayerSystem(GameManager gm) : base(gm) { }

    public override void Initialize() {
        _spawner = new PlayerSpawner(
               new DefaultPlayerFactory(),
               new DropBounceSpawnEffect(),
               GameManager.PlayerBattleParent,
               new CoroutineRunnerAdapter(GameManager)
           );
        _skillSystem = new SkillSystem(this);
    }

    public override void Update(float deltaTime) { }
    public override void Shutdown() { }

    public void SetPlayerStatsRuntimes(PlayerStatData playerStatData) {
        _playerStatsRuntimes.Clear();
        foreach (var stat in playerStatData.playerStatsTemplateList)
        {
            if (!IDValidator.IsPlayerID(stat.StatsData.Id)) continue;
            _playerStatsRuntimes[stat.StatsData.Id] = new PlayerStatsRuntime(stat);
            var Runtime = _playerStatsRuntimes[stat.StatsData.Id];
            Runtime.Initialize(_skillSystem);
        }
    }
    public bool UnlockPlayer(int playerId) {
        if (!IDValidator.IsPlayerID(playerId)) return false;
        bool added = _unlockedIds.Add(playerId);
        //發事件
        if (added) OnPlayerUnlocked?.Invoke(playerId);
        return added;
    }
    public GameObject SpawnBattlePlayer(int playerId) {
        if (!_playerStatsRuntimes.TryGetValue(playerId, out var rt)) return null;
        var go = _spawner.SpawnBattlePlayer(rt);
        //發事件
        OnPlayerSpawned?.Invoke(playerId, rt);
        return go;
    }

    public void ActivateAllPlayer() => _spawner.ActivateAll(GameManager.PlayerSpawnPosition);
    public void DeactivateAllPlayer() => _spawner.DeactivateAll();
    public bool TryGetStatsRuntime(int id, out PlayerStatsRuntime rt) => _playerStatsRuntimes.TryGetValue(id, out rt);

}
