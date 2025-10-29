using System.Collections.Generic;
using UnityEngine;

public class Box_DetectStrategy : SkillDetectStrategyBase {
    private float _rangeX;   // �e��Z���]���ס^
    private float _rangeY;   // �W�U�d��]���ת��@�b�^

    public Box_DetectStrategy(float rangeX, float rangeY) {
        _rangeX = rangeX;
        _rangeY = rangeY;
    }

    public override void Initialize(Transform self) => _self = self;

    public override void DetectTargetsTick(IReadOnlyList<IInteractable> targetList) {
        _targetTransform = null; //�C�� Tick �e�M���¥ؼ�

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
        Vector2 selfPos = _self.position;

        var diffX = Mathf.Abs(targetPos.x - selfPos.x);
        var diffY = Mathf.Abs(targetPos.y - selfPos.y);

        if (diffX <= _rangeX && diffY <= _rangeY) return true;
        return false;
    }

    // �^�ǳ̪��I�]Clamp �b�x����ɤ��^
    public override Vector2 GetClosestPoint(Vector2 worldPos) {
        Vector2 selfPos = _self.position;

        float clampedX = Mathf.Clamp(worldPos.x, selfPos.x - _rangeX, selfPos.x + _rangeX);
        float clampedY = Mathf.Clamp(worldPos.y, selfPos.y - _rangeY, selfPos.y + _rangeY);

        return new Vector2(clampedX, clampedY);
    }

#if UNITY_EDITOR
    // Gizmo�G�H�ۨ������ߵe�x��
    public override void DrawDebugGizmo() {
        if (_self == null) return;
        Gizmos.color = Color.yellow;

        Vector2 selfPos = _self.position;
        Vector2 size = new Vector2(_rangeX * 2f, _rangeY * 2f);


        Gizmos.DrawWireCube(selfPos, size);

        if (_targetTransform != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_self.position, _targetTransform.position);
        }
    }
#endif
}
