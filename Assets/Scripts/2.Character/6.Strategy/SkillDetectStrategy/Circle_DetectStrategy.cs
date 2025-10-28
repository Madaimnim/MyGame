using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle_DetectStrategy : SkillDetectStrategyBase
{
    private float _detectRadius;

    public Circle_DetectStrategy(float detectRadius) {
        _detectRadius = detectRadius;
    } 

    public override void Initialize(Transform self) =>_self = self;
    public override void DetectTargetsTick(IReadOnlyList<IInteractable> targetList) {
        float minDist = float.MaxValue;
        foreach (var t in targetList) {
            if (t == null) continue;

            float dist = Vector2.Distance(_self.position, t.BottomTransform.position);
            if (dist <= _detectRadius && dist < minDist) {
                minDist = dist;
                _targetTransform = t.BottomTransform;
            }
        }
    }

    public override bool IsInRange(Vector2 mouseWorldPos) {
        return Vector2.Distance(_self.position, mouseWorldPos) <= _detectRadius;
    }

    public override Vector2 GetClosestPoint(Vector2 mouseWorldPos) {
        Vector2 selfPos = _self.position;
        Vector2 dir = (mouseWorldPos - selfPos).normalized;
        return selfPos + dir * _detectRadius;
    }

#if UNITY_EDITOR
    public void DrawDebugGizmo() {
        if (_self == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_self.position, _detectRadius);
    }
#endif
}
