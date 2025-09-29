using UnityEngine;

[System.Serializable]
public class EquippedSkillSlot
{
    public int SkillId { get; private set; } = -1;
    public float CooldownTimer { get; private set; }
    public EquippedSkillSlot() {
        SkillId = -1;
        CooldownTimer = 0f;
    }

    public void Equip(int skillId) {
        SkillId = skillId;
        CooldownTimer = 0f;
    }

    public void TriggerCooldown(float cd) {
        if (SkillId == -1) return;
        CooldownTimer = cd;
    }

    public void TickCooldown(float deltaTime) {
        CooldownTimer = Mathf.Max(0, CooldownTimer - deltaTime);
    }

    public bool IsReady => SkillId != -1 && CooldownTimer <= 0f;
}