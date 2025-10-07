using UnityEngine;

[System.Serializable]
public class SkillSlot
{
    public int SkillId { get; private set; } = -1;
    public float CooldownTimer { get; private set; }
    public Detector Detector { get; private set; }

    public bool HasSkill => SkillId  != -1;   //-1 代表無技能;
    public bool IsReady => SkillId != -1 && CooldownTimer <= 0f;

    public SkillSlot() {
        SkillId = -1;
        CooldownTimer = 0f;
        Detector = null;
    }

    public void SetSlot(int skillId,Detector dector = null) {
        SkillId = skillId;
        Detector = dector;
        CooldownTimer = 0f;
    }
    public void TriggerCooldown(float cd) {
        if (SkillId == -1) return;
        CooldownTimer = cd;
    }
    public void TickCooldown(float deltaTime) {
        CooldownTimer = Mathf.Max(0, CooldownTimer - deltaTime);
    }

    public void Uninstall() {
        SkillId = -1;
        CooldownTimer = 0f;
        Detector?.Destroy();
        Detector = null;
    }

}