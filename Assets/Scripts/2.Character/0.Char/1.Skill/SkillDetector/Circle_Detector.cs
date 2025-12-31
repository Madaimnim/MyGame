using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle_Detector : SkillDetectorBase
{
    private float _detectRadius;

    public Circle_Detector(float detectRadius) {  
        _detectRadius = Mathf.Round(detectRadius * 100f) / 100f;
    } 

    public override void Initialize(Transform self) =>_self = self;
    public override void DetectTargetsTick(IReadOnlyList<IInteractable> targetList) {
        _targetTransform = null; //每次 Tick 前清空舊目標

        float minDist = float.MaxValue;
        foreach (var interactable in targetList) {
            if (interactable == null || interactable.Equals(null)) continue;

            Vector2 targetPos = interactable.BottomTransform.position;
            if (IsInRange(targetPos)) {
                float dist = Vector2.Distance(_self.position, targetPos);

                if (dist < minDist) {
                    minDist = dist;
                    _targetTransform = interactable.BottomTransform;
                }
            }
        }
    }
    public override bool IsInRange(Vector2 targetPos) {
        var selfPos = _self.position;
        return Vector2.Distance(selfPos, targetPos) <= _detectRadius;
    }
    public override Vector2 GetClosestPoint(Vector2 mouseWorldPos) {
        Vector2 selfPos = _self.position;
        Vector2 dir = (mouseWorldPos - selfPos).normalized;
        if (dir.sqrMagnitude < 0.0001f) return selfPos; // 避免 NaN
        return selfPos + dir * _detectRadius;
    }
    public override GameObject SpawnRangeObject(Transform parent) {
        var detectorSpriteSpawner = new DetectorSpriteSpawner();
        return detectorSpriteSpawner.CreateCircleObject(_detectRadius);
    }
}
