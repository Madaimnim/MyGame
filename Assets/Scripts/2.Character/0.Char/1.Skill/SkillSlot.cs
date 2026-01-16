using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillSlot
{
    public int SkillId { get; private set; } = -1;
    public float CooldownTimer { get; private set; }
    public SkillDetectorBase Detector { get; private set; }
    public GameObject DetectRangeObject;
    public ISkillRuntime Rt;

    public bool HasSkill => SkillId  != -1;   //-1 代表無技能;
    public bool IsReady => HasSkill && CooldownTimer <= 0f;
    
    private Transform _backSpriteTransform;

    public SkillSlot(Transform backSpriteTransform ) {
        SkillId = -1;
        CooldownTimer = 0f;
        Detector = null;

        _backSpriteTransform = backSpriteTransform;
    }

    public void DetectorTick(IReadOnlyList<IInteractable> targetList) {
        if(Detector != null) Detector.DetectTargetsTick(targetList);
    }
    public void CooldownTick() => CooldownTimer = Mathf.Max(0, CooldownTimer - Time.deltaTime);

    public void SetSlot(ISkillRuntime rt) {
        Rt = rt;
        SkillId = Rt.Id;
        Detector = Rt.Detector;
        CooldownTimer = 0f;
        Detector.Initialize(_backSpriteTransform);

        DetectRangeObject = Detector.SpawnRangeObject(_backSpriteTransform);
        DetectRangeObject.transform.SetParent(_backSpriteTransform);
        DetectRangeObject.transform.localPosition = Vector3.zero;
        DetectRangeObject.SetActive(false);
    }
    public void TriggerCooldown(float cd) {
        if (SkillId == -1) return;
        CooldownTimer = cd;
    }


    public void Uninstall() {
        GameObject.Destroy(DetectRangeObject);
        SkillId = -1;
        CooldownTimer = 0f;
        Detector = null;
    }

}