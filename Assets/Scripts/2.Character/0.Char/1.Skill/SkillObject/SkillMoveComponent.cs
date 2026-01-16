using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMoveComponent {
    private SkillObject _skillObject;

    private ISkillRuntime _skillRt;
    private SkillMoveType _skillMoveType;
    private Transform _targetTransform;
    private Vector3 _targetPosition;
    public Vector2 HorizontalMoveDirection { get; private set; }
    private Vector2 _initialHorizontalDirection;
    public float MoveSpeed { get; private set; }
    public float VerticalSpeed;


    public SkillMoveComponent(SkillObject skillObject) {
        _skillObject = skillObject;
    }

    public void Initialize(ISkillRuntime skillRt,Transform charTransform,Transform charSprTransform,
                            Vector3 initialHorizontalDirection, Vector3 targetPos, Transform targetTransform = null) {
        _skillRt = skillRt;
        _skillMoveType = _skillRt.SkillMoveType;
        MoveSpeed = _skillRt.StatsData.MoveSpeed;
        _targetTransform = targetTransform;
        _targetPosition = targetPos;

        _initialHorizontalDirection = initialHorizontalDirection;
        HorizontalMoveDirection = initialHorizontalDirection;

        InitailPosition(charTransform, charSprTransform);

        var target = targetTransform ? targetTransform.GetComponentInParent<IInteractable>():null;
        float targetHeight = target!=null ? target.RootSpriteCollider.transform.localPosition.y : 0f;
        Vector2 targetVelocity= target != null ? target.MoveVelocity: Vector2.zero;

        ParabolaHelper.TryGetVerticalSpeed(
            GameSettingManager.Instance.PhysicConfig.GravityScale,
            _skillObject.transform.position,
            _skillObject.RootSpriteCollider.transform.localPosition.y,
             MoveSpeed,
            _targetPosition,
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
        _skillObject.transform.position= charTransform.position;
        _skillObject.VisualRootTransform.localPosition = Vector3.zero;

        switch (_skillMoveType) {
            case SkillMoveType.AttackToOwner:
                _skillObject.transform.SetParent(charTransform);
                _skillObject.transform.position = charTransform.position;
                _skillObject.VisualRootTransform.localPosition = Vector3.zero;
                break;
            case SkillMoveType.SpawnAtTarget:
                _skillObject.transform.position = _targetPosition;
                break;
            default:
                break;
        }
        SetFacingRight(_initialHorizontalDirection);
    }
    //移動方法
    private void TowardTick() => _skillObject.transform.position += (Vector3)(_initialHorizontalDirection * MoveSpeed * Time.deltaTime);
    //Todo Fix
    private void StraightTick() => _skillObject.transform.position += (Vector3)(_initialHorizontalDirection * MoveSpeed * Time.deltaTime);
    private void HomingTick() {
        if (_targetTransform != null) {
            HorizontalMoveDirection = (_targetTransform.position -_skillObject.transform.position).normalized;
            //Debug.Log($"改變方向{MoveDirection}");
        }

        SetFacingRight(HorizontalMoveDirection);
        _skillObject.transform.position += (Vector3)(HorizontalMoveDirection * MoveSpeed * Time.deltaTime);
    }
    private void ParabolaTowardTick() {
        _skillObject.transform.position += (Vector3)(HorizontalMoveDirection * MoveSpeed * Time.deltaTime);

        if (_skillObject.RootSpriteCollider.transform.localPosition.y < 0) {
            _skillObject.RootSpriteCollider.transform.localPosition = new Vector3(0, 0);
            _skillObject.StartDestroyTimer(_skillRt.OnHitDestroyDelay);
            return;
        }

        UpdateArrowRotation();
        _skillObject.RootSpriteCollider.transform.localPosition += new Vector3(0f, VerticalSpeed * Time.deltaTime, 0f);

        VerticalSpeed -= GameSettingManager.Instance.PhysicConfig.GravityScale * Time.deltaTime;
    }
    private void UpdateArrowRotation() {
        float angle = Mathf.Atan2(VerticalSpeed,MoveSpeed) * Mathf.Rad2Deg;
        _skillObject.RootSpriteCollider.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void SetFacingRight(Vector2 direction) {
        if (direction.sqrMagnitude < 0.01f) return;     //避免靜止時頻繁執行

        if (Mathf.Abs(direction.x) > 0.01f) {
            var scale = _skillObject.VisualRootTransform.localScale;
            float mag = Mathf.Abs(scale.x);

            switch (_skillRt.FacingDirection) {
                case FacingDirection.Left:
                    scale.x = (direction.x < 0f) ? mag : -mag;
                    break;
                case FacingDirection.Right:
                    scale.x = (direction.x < 0f) ? -mag : mag;
                    break;
            }


            _skillObject.VisualRootTransform.localScale = scale;
        }
    }
}
