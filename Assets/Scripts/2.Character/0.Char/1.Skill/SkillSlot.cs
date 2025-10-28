using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillSlot
{
    public int SkillId { get; private set; } = -1;
    public float CooldownTimer { get; private set; }
    public SkillDetectStrategyBase DetectStrategy { get; private set; }

    public bool HasSkill => SkillId  != -1;   //-1 代表無技能;
    public bool IsReady => SkillId != -1 && CooldownTimer <= 0f;
    
    private Transform _owner;
    private IReadOnlyList<IInteractable> _targetList;

    public SkillSlot(Transform owner, IReadOnlyList<IInteractable> targetList) {
        SkillId = -1;
        CooldownTimer = 0f;
        DetectStrategy = null;
        _owner= owner;
        _targetList= targetList;
    }

    public void Tick() {
        if(DetectStrategy!=null) DetectStrategy.DetectTargetsTick(_targetList);
        TickCooldown();
    }

    public void SetSlot(int skillId,SkillDetectStrategyBase detectStrategy = null) {
        SkillId = skillId;
        DetectStrategy = detectStrategy;
        DetectStrategy.Initialize(_owner);
        CooldownTimer = 0f;
    }
    public void TriggerCooldown(float cd) {
        if (SkillId == -1) return;
        CooldownTimer = cd;
    }

    private void TickCooldown()=>CooldownTimer = Mathf.Max(0, CooldownTimer - Time.deltaTime);  

    public void Uninstall() {
        SkillId = -1;
        CooldownTimer = 0f;
        DetectStrategy = null;
    }

}