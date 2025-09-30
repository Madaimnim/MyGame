using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SkillSystem
{
    private readonly PlayerStateSystem _playerStateSystem;

    public SkillSystem(PlayerStateSystem playerSystem) {
        _playerStateSystem = playerSystem;
    }

    public void EquipSkill(int playerId, int slotIndex, int skillId) {
        var rt = PlayerUtility.Get(playerId);
        rt.SkillSlots[slotIndex].SetId(skillId);

        if (rt.BattlePlayerObject == null) Debug.LogError("BattlePlayerObject 為 null");
        else rt.BattlePlayerObject.GetComponent<Player>()?.SetSkillSlot(slotIndex, skillId);     
        if(rt.PlayerSkillPool.TryGetValue(skillId,out var skill))
            GameEventSystem.Instance.Event_SkillEquipped?.Invoke(playerId, slotIndex, skill);
    }

    public void UnlockSkill(int playerId, int skillId) {
        if (!_playerStateSystem.PlayerStatsRuntimeDtny.TryGetValue(playerId, out var rt)) return;
        if (!rt.PlayerSkillPool.ContainsKey(skillId))
        {
            Debug.LogWarning($"_playerSkillDtny不包含 ID:{skillId}");
            return;
        }
        if (rt.UnlockedSkillIdList.Contains(skillId))
        {
            Debug.LogWarning($"_unlockedSkillIdList已解鎖 ID:{skillId} {rt.PlayerSkillPool[skillId].StatsData.Name}");
            return;
        }
        rt.UnlockedSkillIdList.Add(skillId);
        //發事件
        GameEventSystem.Instance.Event_SkillUnlocked?.Invoke(playerId,  skillId);
    }

}
