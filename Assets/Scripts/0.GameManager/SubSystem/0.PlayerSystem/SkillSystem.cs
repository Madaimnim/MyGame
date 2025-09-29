using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SkillSystem
{
    private readonly PlayerSystem _playerSystem;

    public SkillSystem(PlayerSystem playerSystem) {
        _playerSystem = playerSystem;
    }

    public void EquipSkill(int playerId, int slotIndex, int skillId) {
        if (!_playerSystem.TryGetStatsRuntime(playerId, out var rt))
        {
            Debug.LogError($"[SkillSystem] 找不到玩家 ID:{playerId}");
            return;
        }
        if (slotIndex < 0 || slotIndex >= rt.StatsData.SkillSlotCount)
        {
            Debug.LogWarning($"SlotIndex{slotIndex} 技能槽數量範圍");
            return;
        }
        if (!rt.UnlockedSkillIdList.Contains(skillId))
        {
            Debug.LogWarning($"角色 {rt.StatsData.Id} 試圖裝備未解鎖的技能 ID: {skillId}");
            return;
        }
        if (!rt.PlayerSkillRuntimeDtny.TryGetValue(skillId, out PlayerSkillRuntime skillData))
        {
            Debug.LogError($"[SetSkillAtSlot] 無法取得技能 ID: {skillId}");
            return;
        }
        rt.SetEquippedSkillIds(slotIndex, skillId);

        if (rt.BattlePlayerObject == null) Debug.LogError("BattlePlayerObject 為 null");
        else rt.BattlePlayerObject.GetComponent<Player>()?.SetSkillSlot(slotIndex, skillData);

        var skillRuntime = rt.GetSkillDataRuntimeForId(skillId);
        //發事件
        GameEventSystem.Instance.Event_SkillEquipped?.Invoke(playerId, slotIndex, skillRuntime);
    }

    public void UnlockSkill(int playerId, int skillId) {
        if (!_playerSystem.TryGetStatsRuntime(playerId, out var rt)) return;
        if (!rt.PlayerSkillRuntimeDtny.ContainsKey(skillId))
        {
            Debug.LogWarning($"_playerSkillRuntimeDtny不包含 ID:{skillId}");
            return;
        }
        if (rt.UnlockedSkillIdList.Contains(skillId))
        {
            Debug.LogWarning($"_unlockedSkillIdList已解鎖 ID:{skillId} {rt.PlayerSkillRuntimeDtny[skillId].SkillName}");
            return;
        }
        rt.AddUnlockSkillList(skillId);
        //發事件
        GameEventSystem.Instance.Event_SkillUnlocked?.Invoke(playerId,  skillId);
    }


}
