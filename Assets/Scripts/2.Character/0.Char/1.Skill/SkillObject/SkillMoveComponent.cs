using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMoveComponent {
    private Transform _transform;
    private Collider2D _sprCol;
    private SkillObject _skillObject;

    private ISkillRuntime _skillRt;
    private SkillMoveType _skillMoveType;
    private Transform _targetTransform;
    private Vector3 _targetPosition;
    public Vector2 MoveDirection { get; private set; }
    private Vector2 _initialDirection;
    public float MoveSpeed { get; private set; }
    public float VerticalSpeed;


    public SkillMoveComponent(SkillObject skillObject) {
        _skillObject = skillObject;
        _transform = _skillObject.transform;
        _sprCol = skillObject.SprCol;
    }

    public void Initialize(ISkillRuntime skillRt,Transform charTransform,Transform charSprTransform, Vector3 targetPos, Transform targetTransform = null) {
        _skillRt = skillRt;
        _skillMoveType = _skillRt.SkillMoveType;
        MoveSpeed = _skillRt.StatsData.MoveSpeed;
        _targetTransform = targetTransform;
        _targetPosition = targetPos;

        Vector3 referencePos = targetTransform ? targetTransform.position : _targetPosition;
        _initialDirection = (referencePos - _transform.position).normalized;
        MoveDirection = _initialDirection;
        InitailPosition(charTransform, charSprTransform);

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

    }


    public void Tick() {
        switch (_skillMoveType) {
            case SkillMoveType.AttackToOwner:
                break;
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

    private void InitailPosition(Transform charTransform,Transform charSprTransform) {
        _transform.position= charTransform.position;
        switch (_skillMoveType) {
            case SkillMoveType.AttackToOwner:
                _transform.SetParent(charTransform);
                _transform.localPosition = Vector3.zero;
                _transform.localScale= Vector3.one;
                break;
            case SkillMoveType.SpawnAtTarget:
                _transform.position = _targetPosition;
                break;
        }
        SetFacingRight(MoveDirection);
    }
    //移動方法
    private void TowardTick() => _transform.position += (Vector3)(MoveDirection * MoveSpeed * Time.deltaTime);
    private void StraightTick() => _transform.position += (Vector3)(MoveDirection * MoveSpeed * Time.deltaTime);
    private void HomingTick() {
        if (_targetTransform != null) {
            MoveDirection = (_targetTransform.position - _transform.position).normalized;
            //Debug.Log($"改變方向{MoveDirection}");
        }
        else {
            MoveDirection = _initialDirection;
            //Debug.Log($"原方向{MoveDirection}");
        }


        SetFacingRight(MoveDirection);
        _transform.position += (Vector3)(MoveDirection * MoveSpeed * Time.deltaTime);
    }
    private void ParabolaTowardTick() {
        if (_sprCol == null) {
            Debug.LogError("[SkillMove] sprCol destroyed during Tick");
            return;
        }

        _transform.position += (Vector3)(MoveDirection * MoveSpeed * Time.deltaTime);

        if (_sprCol.transform.localPosition.y < 0) {
            _sprCol.transform.localPosition = new Vector3(0, 0, 0);
            _skillObject.StartDestroyTimer(_skillRt.OnHitDestroyDelay);
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

    public void SetFacingRight(Vector2 direction) {
        if (direction.sqrMagnitude < 0.01f) return;     //避免靜止時頻繁執行

        if (Mathf.Abs(direction.x) > 0.01f) {
            var scale = _transform.localScale;
            float mag = Mathf.Abs(scale.x);

            switch (_skillRt.FacingDirection) {
                case FacingDirection.Left:
                    scale.x = (direction.x < 0f) ? mag : -mag;
                    break;
                case FacingDirection.Right:
                    scale.x = (direction.x < 0f) ? -mag : mag;
                    break;
            }


            _transform.localScale = scale;
        }
    }
}
