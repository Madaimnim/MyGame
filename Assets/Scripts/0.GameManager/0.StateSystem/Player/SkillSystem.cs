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
        if (!_playerStateSystem.PlayerStatsRuntimeDtny.TryGetValue(playerId, out var rt))
        {
            Debug.LogError($"[SkillSystem] �䤣�쪱�a ID:{playerId}");
            return;
        }
        if (slotIndex < 0 || slotIndex >= rt.StatsData.SkillSlotCount)
        {
            Debug.LogWarning($"SlotIndex{slotIndex} �ޯ�Ѽƶq�d��");
            return;
        }
        if (!rt.UnlockedSkillIdList.Contains(skillId))
        {
            Debug.LogWarning($"���� {rt.StatsData.Id} �չϸ˳ƥ����ꪺ�ޯ� ID: {skillId}");
            return;
        }

        var skillRuntime = rt.GetSkillRuntime(skillId);
        if (skillRuntime==null)
        {
            Debug.LogError($"[SetSkillAtSlot] �L�k���o�ޯ� ID: {skillId}");
            return;
        }
        rt.SetEquippedSkillIds(slotIndex, skillId);

        if (rt.BattlePlayerObject == null) Debug.LogError("BattlePlayerObject �� null");
        else rt.BattlePlayerObject.GetComponent<Player>()?.SetSkillSlot(slotIndex, skillId);

        //�o�ƥ�
        GameEventSystem.Instance.Event_SkillEquipped?.Invoke(playerId, slotIndex, skillRuntime);
    }
    public void UnlockSkill(int playerId, int skillId) {
        if (!_playerStateSystem.PlayerStatsRuntimeDtny.TryGetValue(playerId, out var rt)) return;
        if (!rt.PlayerSkillRuntimeDtny.ContainsKey(skillId))
        {
            Debug.LogWarning($"_playerSkillRuntimeDtny���]�t ID:{skillId}");
            return;
        }
        if (rt.UnlockedSkillIdList.Contains(skillId))
        {
            Debug.LogWarning($"_unlockedSkillIdList�w���� ID:{skillId} {rt.PlayerSkillRuntimeDtny[skillId].SkillName}");
            return;
        }
        rt.AddUnlockSkillList(skillId);
        //�o�ƥ�
        GameEventSystem.Instance.Event_SkillUnlocked?.Invoke(playerId,  skillId);
    }
}
