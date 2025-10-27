using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SkillHitComponent {
    private SkillObject _owner;
    private SkillMoveComponent _skillMoveComponent;
    private int _damage;
    private float _knockbackPower;
    private float _floatPower;

    private List<(IInteractable target, Collider2D col)> _targetList = new();

    public SkillHitComponent(SkillObject owner) {
        _owner = owner;
    }

    public void Initialize(SkillMoveComponent skillMoveComponent, StatsData pStatsData, StatsData sStatsData) {
        _skillMoveComponent = skillMoveComponent;
        _damage = pStatsData.Power + sStatsData.Power;
        _knockbackPower = pStatsData.KnockbackPower + sStatsData.KnockbackPower;
        _floatPower = pStatsData.FloatPower + sStatsData.FloatPower;
    }
    public void Tick() {
        for (int i = _targetList.Count - 1; i >= 0; i--) {
            var (target, col) = _targetList[i];
            if (target == null || col == null) {
                _targetList.RemoveAt(i);
                continue;
            }

            float bottomYDiff = Mathf.Abs(_owner.transform.position.y - target.BottomY);
            if (bottomYDiff > PhysicManager.Instance.PhysicConfig.BottomYThreshold) continue;

            Hit(target, col);
            _targetList.RemoveAt(i);
        }
    }
    public void TriggerEnter(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & _owner.TargetLayers) == 0) return;

        IInteractable target = collision.GetComponentInParent<IInteractable>();
        if (target == null) return;

        if (!_targetList.Exists(t => t.target == target))
            _targetList.Add((target, collision));
    }

    private void Hit(IInteractable target, Collider2D targetCol) {
        Vector2 hitPoint = GetHitEffectPosition(targetCol);
        VFXManager.Instance.Play("DamageEffect01", hitPoint, targetCol.GetComponent<SpriteRenderer>());
        target.Interact(new InteractInfo {
            Source = _owner.transform,
            Damage = _damage,
            KnockbackForce = _knockbackPower * _skillMoveComponent.MoveDirection,
            FloatPower = _floatPower

        });
        switch (_owner.OnHitType) {
            case OnHitType.Disappear:
                StartDestroyTimer(_owner.OnHitDestroyDelay);
                break;

            case OnHitType.Explode:
                // 可在這裡觸發爆炸特效、AOE 等
                StartDestroyTimer(_owner.OnHitDestroyDelay);
                break;

            case OnHitType.Nothing:
                // 不處理銷毀
                break;
        }
    }

    private Vector2 GetHitEffectPosition(Collider2D targetCol) {
        Bounds targetbounds = targetCol.bounds;
        Vector2 targetcenter = targetbounds.center;

        switch (_owner.HitEffectPositionType) {
            case HitEffectPositionType.ClosestPoint:
                Vector2 hitA = targetCol.ClosestPoint(_owner.transform.position);
                Vector2 hitB = _owner.SprCol.ClosestPoint(targetCol.transform.position);
                return (hitA + hitB) / 2f;

            case HitEffectPositionType.TargetCenter:
                return GetRandomPointNear(targetCol, targetcenter, targetbounds.extents);

            case HitEffectPositionType.TargetUpper:
                Vector2 upperCenter = new Vector2(
                    targetcenter.x,
                    targetbounds.min.y + targetbounds.size.y * 0.8f
                );
                return GetRandomPointNear(targetCol, upperCenter, targetbounds.extents);

            default:
                return targetcenter;
        }
    }
    private Vector2 GetRandomPointNear(Collider2D col, Vector2 refCenter, Vector2 extents, float sizeRatio = 0.3f) {
        Vector2 point;
        int safety = 20; // 最多嘗試20次，避免極端情況
        float rangeX = extents.x * sizeRatio;
        float rangeY = extents.y * sizeRatio;

        do {
            // 在正方形範圍內隨機取一點
            float x = UnityEngine.Random.Range(refCenter.x - rangeX, refCenter.x + rangeX);
            float y = UnityEngine.Random.Range(refCenter.y - rangeY, refCenter.y + rangeY);
            point = new Vector2(x, y);
            safety--;
        } while (!col.OverlapPoint(point) && safety > 0);

        return point;
    }

    private void StartDestroyTimer(float delay) => _owner.StartDestroyTimer(delay);

}
