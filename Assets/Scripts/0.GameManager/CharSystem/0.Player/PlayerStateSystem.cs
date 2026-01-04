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
    public event Action<int> OnPlayerEnterBattle;
    public event Action<int> OnPlayerUnlocked;

    private Dictionary<int, PlayerStatsRuntime> _playerStatsRuntimeDtny = new();
    private Dictionary<int, Player> _battlePlayerDtny = new();
    private List<int> _unlockedIdList = new();

    private ISpawnEffect _spawnEffect;

    public PlayerStateSystem(GameManager gm) : base(gm) { }
    public override void Initialize() {
        _spawnEffect= new DropBounceSpawnEffect();
        SpawnSystem = new PlayerSpawnSystem();
        SkillSystem = new PlayerSkillSystem();
    }


    public void UnlockPlayer(int id) {
        if (!IDValidator.IsPlayerID(id)) Debug.LogWarning($"PlayerID不合法");
        //解鎖、創造腳色
        _unlockedIdList.Add(id);
        var player=SpawnSystem.SpawnerPlayer(id);
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

        //UnlockPlayer(1002);

        AllPlayerClose();
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

    public void AllPlayerEnterBattle(Vector3 spawnPos) {
        Debug.Log("AllPlayerEnterBattle");
        float offsetY = -1f;
        foreach (var player in PlayerUtility.AllPlayers.Values) {
            if (!player) continue;
            player.gameObject.SetActive(true);
            GameManager.StartCoroutine(_spawnEffect.Play(player.gameObject, spawnPos));
            spawnPos.y += offsetY;

            //發事件
            OnPlayerEnterBattle?.Invoke(player.Rt.StatsData.Id);
        }
    }
    public void AllPlayerClose() {
        foreach (var player in PlayerUtility.AllPlayers.Values) {
            if (!player) continue;
            player.gameObject.SetActive(false);
            player.transform.position = Vector2.zero;
        }
    }
}
