using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


[System.Serializable]
public class SkillSlotRuntime<TSkill> where TSkill : SkillBase
{
    public int SlotIndex { get; private set; }
    public float CooldownTimer { get; private set; }
    public TSkill SkillData { get; private set; }


    public SkillSlotRuntime(int slotIndex) {
        SlotIndex = slotIndex;
        CooldownTimer = 0f;
    }

    public void BindSkill(TSkill data) {
        SkillData = data;
        CooldownTimer = 0f;
    }

    public void TriggerCooldown() {
        if (SkillData == null) return;
        CooldownTimer = SkillData.SkillCooldown;
    }

    public void TickCooldown(float deltaTime) {
        if (SkillData == null) return;
        CooldownTimer = Mathf.Max(0, CooldownTimer - deltaTime);
    }
    public bool IsSkillReady => CooldownTimer <= 0f;
}
