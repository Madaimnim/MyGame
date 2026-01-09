using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;
using System.Linq;

public sealed class PlayerStateSystem : GameSubSystem
{
    public IReadOnlyDictionary<int, PlayerStatsRuntime> PlayerStatsRuntimeDtny => _playerStatsRuntimeDtny;
    public IReadOnlyDictionary<int, Player> PlayerInstanceDtny => _playerInstanceDtny;
    public List<int> UnlockedIdList => _unlockedIdList;

    public PlayerSkillSystem SkillSystem { get; private set; }
    public PlayerSpawnSystem SpawnSystem { get; private set; }

    private Dictionary<int, PlayerStatsRuntime> _playerStatsRuntimeDtny = new();
    private Dictionary<int, Player> _playerInstanceDtny = new();
    private List<int> _unlockedIdList = new();


    public PlayerStateSystem(GameManager gm) : base(gm) { }
    public override void Initialize() {
        SpawnSystem = new PlayerSpawnSystem();
        SkillSystem = new PlayerSkillSystem();
    }


    public void UnlockPlayer(int id) {
        if (!IDValidator.IsPlayerID(id)) Debug.LogWarning($"PlayerID不合法");
        //解鎖、創造腳色
        _unlockedIdList.Add(id);
        var player=SpawnSystem.SpawnerPlayer(id);
        _playerInstanceDtny.Add(id, player);

        //初始化解鎖、安裝技能
        _playerStatsRuntimeDtny[id].UnlockedSkillIdList.Add(1);
        SkillSystem.EquipPlayerSkill(id, 0, 1);
        SkillSystem.EquipPlayerSkill(id, 1, 2);
        SkillSystem.EquipPlayerSkill(id, 2, 3);
        SkillSystem.EquipPlayerSkill(id, 3, 4);
        SkillSystem.EquipPlayerSkill(id, 4, 5);
    }
    public void PrepareInitialPlayers() {
        UnlockPlayer(1001);
        _playerStatsRuntimeDtny[1001].UnlockedSkillIdList.Add(2);
        //SkillSystem.EquipPlayerSkill(1001, 0, 2);

        //UnlockPlayer(1002);

        HideAllPlayers();
    }
    public void SetPlayerStatsRuntimeDtny(PlayerStatData playerStatData) {
        _playerStatsRuntimeDtny.Clear();
        foreach (var stat in playerStatData.playerStatsTemplateList)
        {
            if (!IDValidator.IsPlayerID(stat.StatsData.Id)) continue;
            _playerStatsRuntimeDtny[stat.StatsData.Id] = new PlayerStatsRuntime(stat);
            //var Runtime = _playerStatsRuntimeDtny[stat.StatsData.Id];
        }
    }
    public void PlayerAppear(Player player,Vector3 spawnPos,AppearType appearType) {    
        var effector = AppearEffectorFactory.CreateEffector(appearType);

        player.gameObject.SetActive(true);
        GameManager.StartCoroutine(effector.Play(player.gameObject, spawnPos));
    }
    public void AllPlayerAppear(Vector3 spawnPos, AppearType playerAppearType) {
        var appearEffector = AppearEffectorFactory.CreateEffector(playerAppearType);

        foreach (var player in PlayerUtility.AllPlayers.Values) {
            if (!player) continue;
            player.gameObject.SetActive(true);
            GameManager.StartCoroutine(appearEffector.Play(player.gameObject, spawnPos));

            //Debug.Log($"{player.name}生成在{spawnPos}");
        }
    }
    public void HideAllPlayers() {
        foreach (var player in PlayerUtility.AllPlayers.Values) {
            if (!player) continue;
            player.gameObject.SetActive(false);
            player.transform.position = Vector2.zero;
        }
    }

}
