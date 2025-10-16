using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;
using System.Linq;

public sealed class PlayerStateSystem : SubSystemBase
{
    public IReadOnlyDictionary<int, PlayerStatsRuntime> PlayerStatsRuntimeDtny => _playerStatsRuntimeDtny;
    public IReadOnlyDictionary<int, Player> BattlePlayerDtny => _battlePlayerDtny;
    public List<int> UnlockedIdList => _unlockedIdList;

    public PlayerSkillSystem SkillSystem { get; private set; }
    public PlayerSpawnSystem SpawnSystem { get; private set; }

    // 事件
    public event Action<int> OnPlayerUnlocked;

    private Dictionary<int, PlayerStatsRuntime> _playerStatsRuntimeDtny = new();
    private Dictionary<int, Player> _battlePlayerDtny = new();
    private List<int> _unlockedIdList = new();


    public PlayerStateSystem(GameManager gm) : base(gm) { }
    public override void Initialize() {
        SpawnSystem = new PlayerSpawnSystem(
               new DefaultPlayerFactory(),
               new DropBounceSpawnEffect(),
               new CoroutineRunnerAdapter(GameManager)
           );
        SkillSystem = new PlayerSkillSystem();
    }


    public void UnlockPlayer(int id) {
        if (!IDValidator.IsPlayerID(id)) Debug.LogWarning($"PlayerID不合法");
        //解鎖、創造腳色
        _unlockedIdList.Add(id);
        var player=SpawnSystem.CreatPlayer(id);
        _battlePlayerDtny.Add(id, player);

        //初始化解鎖、安裝技能
        _playerStatsRuntimeDtny[id].UnlockedSkillIdList.Add(1);
        SkillSystem.EquipPlayerSkill(id, 0, 1);
        
        //發事件
        OnPlayerUnlocked?.Invoke(id);   
    }
    public void InitialGameStartPlayerSpawn() {
        UnlockPlayer(1001);
        _playerStatsRuntimeDtny[1001].UnlockedSkillIdList.Add(2);
        //SkillSystem.EquipPlayerSkill(1001, 0, 2);

        UnlockPlayer(1002);

        SpawnSystem.CloseAllPlayer();
    }

    public void SetPlayerStatsRuntimeDtny(PlayerStatData playerStatData) {
        _playerStatsRuntimeDtny.Clear();
        foreach (var stat in playerStatData.playerStatsTemplateList)
        {
            if (!IDValidator.IsPlayerID(stat.StatsData.Id)) continue;
            _playerStatsRuntimeDtny[stat.StatsData.Id] = new PlayerStatsRuntime(stat);
            var Runtime = _playerStatsRuntimeDtny[stat.StatsData.Id];
        }
    }
    public void SpawnAllPlayer(Vector3 spawnPoint) => SpawnAllPlayer(spawnPoint);
}
