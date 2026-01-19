using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAttackSlot {
    public float CooldownTimer { get; private set; }
    public SkillDetectorBase Detector { get; private set; }
    public GameObject DetectRangeObject;
    public bool IsReady => CooldownTimer <= 0f;
    private Transform _backSpriteTransform;

    public BaseAttackSlot(Transform backSpriteTransform) {
        CooldownTimer = 0f;
        Detector = null;

        _backSpriteTransform = backSpriteTransform;
    }

    public void DetectorTick(IReadOnlyList<IInteractable> targetList) {
        if (Detector != null) Detector.DetectTargetsTick(targetList);
    }
    public void CooldownTick() => CooldownTimer = Mathf.Max(0, CooldownTimer - Time.deltaTime);

    public void SetSlot(ISkillRuntime rt) {
        CooldownTimer = 0f;
        Detector = rt.Detector;
        Detector.Initialize(_backSpriteTransform);

        DetectRangeObject = Detector.SpawnRangeObject(_backSpriteTransform);
        DetectRangeObject.transform.SetParent(_backSpriteTransform);
        DetectRangeObject.transform.localPosition = Vector3.zero;
        DetectRangeObject.transform.localPosition = Vector3.zero;
        DetectRangeObject.SetActive(false);
    }
    public void TriggerCooldown(float cd) {
        CooldownTimer = cd;
    }
    public void Uninstall() {
        GameObject.Destroy(DetectRangeObject);
        CooldownTimer = 0f;
        Detector = null;
    }

}