using System;
using System.Collections;
using UnityEngine;
using System.Linq;

public class CharSkillComponent 
{
    private PlayerStatsRuntime _rt;
    private GameObject _playerObj;

    public event Action<int, int, PlayerSkillRuntime> OnSkillEquipped; // (slot, skillId, runtime)
    public event Action<int, PlayerSkillRuntime> OnSkillUsed;         // (slot, runtime)

    public CharSkillComponent(PlayerStatsRuntime rt,GameObject playerObj) {
        _rt = rt ?? throw new ArgumentNullException(nameof(rt));
        _playerObj = playerObj ?? throw new ArgumentNullException(nameof(playerObj));
    }

    public void EquipSkill(int slotIndex, int skillId) {
        _rt.SkillSlots[slotIndex].SetId(skillId);
        //Todo 實作安裝技能
        
        //發事件
        OnSkillEquipped?.Invoke(slotIndex, skillId, _rt.GetSkillRuntime(skillId));
    }

    public void UseSkill(int slotIndex, Transform ownerTransform) {
        var slot = _rt.SkillSlots[slotIndex];
        if (!slot.IsReady) return;

        var skill = _rt.GetSkillRuntime(slot.SkillId);
        if (skill == null) return;

        // 冷卻
        slot.TriggerCooldown(skill.Cooldown);

        // 使用次數加一
        bool leveledUp = AddSkillUsageCount(skill);

        if (leveledUp)
        {
            GameEventSystem.Instance.Event_SkillLevelUp?.Invoke(skill, ownerTransform);
        }
    }


    public bool AddSkillUsageCount(PlayerSkillRuntime skill) {
        skill.SkillUsageCount++;
        if (skill.SkillUsageCount >= skill.NextLevelCount)
        {
            SkillLevelUp(skill);
            return true; // 告訴呼叫端「升級了」
        }
        return false;
    }
    private void SkillLevelUp(PlayerSkillRuntime skill) {
        skill.StatsData.Power++;
        skill.SkillLevel++;
        skill.NextLevelCount += skill.SkillLevel * 10;
    }

    public void Tick(float deltaTime) {
        foreach (var slot in _rt.SkillSlots)
            slot.TickCooldown(deltaTime);
    }


    public void OnSkillUsed(int slotIndex, Transform ownerTransform) {
        var playerSkillRuntime = GetSkillAtSlot(slotIndex);
        if (playerSkillRuntime == null) return;

        bool leveledUp = playerSkillRuntime.AddSkillUsageCount();
        if (leveledUp)
        {
            GameEventSystem.Instance.Event_SkillLevelUp?.Invoke(playerSkillRuntime, ownerTransform);
            GameEventSystem.Instance.Event_SkillInfoChanged?.Invoke(slotIndex, ownerTransform.GetComponent<Player>());
        }
    }


}
