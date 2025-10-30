using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMoveComponent {
    private SkillObject _owner;
    private Transform _transform;
    private Collider2D _sprCol;

    private SkillMoveType _moveType;
    private Transform _targetTransform;
    private Vector3 _targetPosition;
    public Vector2 MoveDirection { get; private set; }

    private Vector2 _initialDirection;
    public float MoveSpeed { get; private set; }
    public float VerticalSpeed;

    public SkillMoveComponent(SkillObject owner) {
        _owner = owner;
        _transform = owner.transform;
        _sprCol = owner.SprCol;
    }

    public void Initialize(SkillMoveType moveType, float moveSpeed, Vector3 targetPos, Transform targetTransform = null) {
        _moveType = moveType;
        _targetTransform = targetTransform;
        _targetPosition = targetPos;
        MoveSpeed = moveSpeed;

        Vector3 referencePos = targetTransform ? targetTransform.position : targetPos;
        _initialDirection = (referencePos - _transform.position).normalized;
        MoveDirection = _initialDirection;
        InitailPosition(referencePos);

        var target = targetTransform ? targetTransform.GetComponentInParent<IInteractable>():null;
        float targetHeight = target!=null ? target.SprCol.transform.localPosition.y : 0f;
        Vector2 targetVelocity= target != null ? target.MoveVelocity: Vector2.zero;

        ParabolaHelper.TryGetVerticalSpeed(
            PhysicManager.Instance.PhysicConfig.GravityScale,
            _transform.position,
             _sprCol.transform.localPosition.y,
             MoveSpeed,
            referencePos,
            targetHeight,
            targetVelocity,

            out VerticalSpeed);
        //Debug.Log($"目標的 MoveSpeed:{targetVelocity}");
    }


    public void Tick() {
        switch (_moveType) {
            case SkillMoveType.Station:
                break;
            case SkillMoveType.Toward:
                TowardTick();
                break;
            case SkillMoveType.ParabolaToward:
                ParabolaTowardTick();
                break;
            case SkillMoveType.Homing:
                HomingTick();
                break;
            case SkillMoveType.Straight:
                StraightTick();
                break;
            case SkillMoveType.SpawnAtTarget:
                break;
        }
    }

    private void InitailPosition(Vector3 targetPos) {
        switch (_moveType) {
            case SkillMoveType.SpawnAtTarget:
                _owner.transform.position = targetPos;
                break;
        }
    }
    //移動方法
    private void TowardTick() => _transform.position += (Vector3)(MoveDirection * MoveSpeed * Time.deltaTime);
    private void StraightTick() => _transform.position += (Vector3)(MoveDirection * MoveSpeed * Time.deltaTime);
    private void HomingTick() {
        if (_targetTransform != null)
            MoveDirection = (_targetTransform.position - _transform.position).normalized;
        else
            MoveDirection = _initialDirection;

        _owner.SetFacingRight(MoveDirection);
        _transform.position += (Vector3)(MoveDirection * MoveSpeed * Time.deltaTime);
    }
    private void ParabolaTowardTick() {
        _transform.position += (Vector3)(MoveDirection * MoveSpeed * Time.deltaTime);

        if (_sprCol.transform.localPosition.y < 0) {
            //Debug.Log("localPosition 小於0, reset為 0");
            _sprCol.transform.localPosition = new Vector3(0, 0, 0);
            _owner.StartDestroyTimer(_owner.OnHitDestroyDelay);
            return;
        }

        UpdateRotation();
        _sprCol.transform.localPosition += new Vector3(0f, VerticalSpeed * Time.deltaTime, 0f);

        VerticalSpeed -= PhysicManager.Instance.PhysicConfig.GravityScale * Time.deltaTime;

    }
    private void UpdateRotation() {

        float angle = Mathf.Atan2(VerticalSpeed,MoveSpeed) * Mathf.Rad2Deg;
        _sprCol.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
