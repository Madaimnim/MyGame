using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillSlot
{
    public int SkillId { get; private set; } = -1;
    public float CooldownTimer { get; private set; }
    public SkillDetectorBase Detector { get; private set; }
    public GameObject RangeObject;

    public bool HasSkill => SkillId  != -1;   //-1 代表無技能;
    public bool IsReady => SkillId != -1 && CooldownTimer <= 0f;
    
    private Transform _backSpriteTransform;
    private IReadOnlyList<IInteractable> _targetList;

    public SkillSlot(Transform backSpriteTransform, IReadOnlyList<IInteractable> targetList) {
        SkillId = -1;
        CooldownTimer = 0f;
        Detector = null;

        _backSpriteTransform = backSpriteTransform;
        _targetList= targetList;
    }

    public void Tick() {
        if(Detector != null) Detector.DetectTargetsTick(_targetList);
        TickCooldown();
    }

    public void SetSlot(int skillId,SkillDetectorBase detectStrategy = null) {
        SkillId = skillId;
        Detector = detectStrategy;
        CooldownTimer = 0f;
        Detector.Initialize(_backSpriteTransform);

        if (RangeObject != null) GameObject.Destroy(RangeObject);
        RangeObject = Detector.SpawnRangeObject(_backSpriteTransform);
        RangeObject.transform.SetParent(_backSpriteTransform);
        RangeObject.transform.localPosition = Vector3.zero;
    }
    public void TriggerCooldown(float cd) {
        if (SkillId == -1) return;
        CooldownTimer = cd;
    }

    private void TickCooldown()=>CooldownTimer = Mathf.Max(0, CooldownTimer - Time.deltaTime);  

    public void Uninstall() {
        SkillId = -1;
        CooldownTimer = 0f;
        Detector = null;
    }

}