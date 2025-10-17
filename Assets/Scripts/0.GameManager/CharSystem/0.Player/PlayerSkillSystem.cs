using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PlayerSkillSystem
{
    public PlayerSkillSystem() {}
    public void EquipPlayerSkill(int playerId, int slotIndex, int skillId) {
        var rt = PlayerUtility.Get(playerId);
        if(rt.SkillPool.TryGetValue(skillId,out var skill))
        {
            rt.BattleObject.GetComponent<Player>()?.SkillComponent.EquipSkill(slotIndex, skillId, skill.VisualData.DetectPrefab);
        }
 
    }
    public void UnlockSkill(int playerId, int skillId) {
        var rtDtny = PlayerUtility.AllRts;
        if (!rtDtny.TryGetValue(playerId, out var rt)) return;
        if (!rt.SkillPool.ContainsKey(skillId))
        {
            Debug.LogWarning($"_playerSkillDtny不包含 ID:{skillId}");
            return;
        }
        if (rt.UnlockedSkillIdList.Contains(skillId))
        {
            Debug.LogWarning($"_unlockedSkillIdList已解鎖 ID:{skillId} {rt.SkillPool[skillId].StatsData.Name}");
            return;
        }
        rt.UnlockedSkillIdList.Add(skillId);
        //發事件
        GameEventSystem.Instance.Event_SkillUnlocked?.Invoke(playerId,  skillId);
    }
}
