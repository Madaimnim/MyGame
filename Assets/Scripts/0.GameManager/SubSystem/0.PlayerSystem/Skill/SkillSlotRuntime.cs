using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


[System.Serializable]
public class SkillSlotRuntime
{
    public int SlotIndex { get; private set; }
    public float CooldownTimer { get; private set; }
    public SkillBase SkillData { get; private set; }


    public SkillSlotRuntime(int slotIndex) {
        slotIndex = slotIndex;
        CooldownTimer = 0f;
    }

    public void BindSkill(SkillBase data) {
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
