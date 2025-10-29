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
        _targetTransform = null; //每次 Tick 前清空舊目標

        float minDist = float.MaxValue;
        foreach (var t in targetList) {
            if (t == null || t.Equals(null)) continue;

            Vector2 targetPos = t.BottomTransform.position;
            if (IsInRange(targetPos)) {
                float dist = Vector2.Distance(_self.position, targetPos);

                if (dist < minDist) {
                    minDist = dist;
                    _targetTransform = t.BottomTransform;
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

#if UNITY_EDITOR
    public override void DrawDebugGizmo() {
        if (_self == null) return;
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(_self.position, _detectRadius);

        if (_targetTransform != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_self.position, _targetTransform.position);
        }
    }
#endif
}
