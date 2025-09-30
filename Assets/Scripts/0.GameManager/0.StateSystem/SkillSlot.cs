using UnityEngine;

[System.Serializable]
public class SkillSlot
{
    public int SkillId { get; private set; } = -1;
    public float CooldownTimer { get; private set; }
    public bool IsReady => SkillId != -1 && CooldownTimer <= 0f;

    public SkillSlot() {
        SkillId = -1;
        CooldownTimer = 0f;
    }

    public void SetId(int skillId) {
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
}