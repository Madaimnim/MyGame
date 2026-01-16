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
    public PlayerSpawnSystem SpawnSystem { get; private set; }

    private Dictionary<int, PlayerStatsRuntime> _playerStatsRuntimeDtny = new();
    private Dictionary<int, Player> _playerInstanceDtny = new();
    private List<int> _unlockedIdList = new();


    public PlayerStateSystem(GameManager gm) : base(gm) { }
    public override void Initialize() {
        SpawnSystem = new PlayerSpawnSystem();
    }


    public void UnlockPlayer(int id) {
        if (!IDValidator.IsPlayerID(id)) Debug.LogWarning($"PlayerID不合法");
        //解鎖、創造腳色
        _unlockedIdList.Add(id);
        var player=SpawnSystem.SpawnerPlayer(id);
        _playerInstanceDtny.Add(id, player);

        //初始化解鎖、安裝技能
        _playerStatsRuntimeDtny[id].UnlockedSkillIdHashSet.Add(1);
        //SkillSystem.EquipPlayerSkill(id, 0, 1);

        EquipPlayerBaseAttack(id);
        EquipPlayerSkill(id, 1, 1);
        EquipPlayerSkill(id, 2, 2);
        EquipPlayerSkill(id, 3, 3);
        EquipPlayerSkill(id, 4, 4);
    }
    public void PrepareInitialPlayers() {
        UnlockPlayer(1001);
        _playerStatsRuntimeDtny[1001].UnlockedSkillIdHashSet.Add(2);
        //SkillSystem.EquipPlayerSkill(1001, 0, 2);

        //UnlockPlayer(1002);

        HideAllPlayers();
    }
    public void SetPlayerStatsRuntimeDtny(PlayerStatData playerStatData) {
        _playerStatsRuntimeDtny.Clear();
        foreach (var stat in playerStatData.playerStatsTemplateList)
        {
            if (!IDValidator.IsPlayerID(stat.Id)) continue;
            _playerStatsRuntimeDtny[stat.Id] = new PlayerStatsRuntime(stat);
            //var Runtime = _playerStatsRuntimeDtny[stat.Id];
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


    public void EquipPlayerBaseAttack(int playerId) {
        var player = PlayerUtility.GetPlayer(playerId);
        var rt = PlayerUtility.Get(playerId);
        player.CombatComponent.EquipBaseAttack(rt.BaseAttackRuntime);
    }
    public void EquipPlayerSkill(int playerId, int slotNumber, int skillId) {
        var player = PlayerUtility.GetPlayer(playerId);
        var rt = PlayerUtility.Get(playerId);
        if (rt.SkillPool.TryGetValue(skillId, out var skillRt))
            player.CombatComponent.EquipSkill(slotNumber, skillRt);
        
    }

    public void UnlockSkill(int playerId, int skillId) {
        var rtDtny = PlayerUtility.AllRts;
        if (!rtDtny.TryGetValue(playerId, out var rt)) return;
        if (!rt.SkillPool.ContainsKey(skillId)) {
            Debug.LogWarning($"_playerSkillDtny不包含 ID:{skillId}");
            return;
        }
        if (rt.UnlockedSkillIdHashSet.Contains(skillId)) {
            Debug.LogWarning($"_unlockedSkillIdList已解鎖 ID:{skillId} {rt.SkillPool[skillId].Name}");
            return;
        }
        rt.UnlockedSkillIdHashSet.Add(skillId);
        //發事件
        GameEventSystem.Instance.Event_SkillUnlocked?.Invoke(playerId, skillId);
    }

}
