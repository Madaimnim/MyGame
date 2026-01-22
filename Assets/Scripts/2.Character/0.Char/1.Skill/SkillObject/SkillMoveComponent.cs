using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMoveComponent {
    private SkillObject _skillObject;
    private SkillHeightComponent _skillheightComponent;

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

    public void Initialize(SkillHeightComponent skillheightComponent,ISkillRuntime skillRt,Transform sourceCharTransform,
        Transform sourceCharHeightTransform, Vector3 initialHorizontalDirection, Vector3 targetPos, Transform targetTransform = null) {
        _skillheightComponent = skillheightComponent;

        _skillRt = skillRt;
        _skillMoveType = _skillRt.SkillMoveType;
        MoveSpeed = _skillRt.StatsData.MoveSpeed;
        _targetTransform = targetTransform;
        _targetPosition = targetPos;

        _initialHorizontalDirection = initialHorizontalDirection;
        HorizontalMoveDirection = initialHorizontalDirection;

        InitailPosition(sourceCharTransform, sourceCharHeightTransform);

        var target = targetTransform ? targetTransform.GetComponentInParent<IInteractable>():null;
        float targetHeight = target!=null ? target.HeightInfo.Min : 0f;
        Vector2 targetVelocity= target != null ? target.MoveVelocity: Vector2.zero;

        ParabolaHelper.TryGetVerticalSpeed(
            GameSettingManager.Instance.PhysicConfig.GravityScale,
            _skillObject.transform.position,
            _skillObject.HeightInfo.Min,
             MoveSpeed,
            _targetPosition,
            targetHeight,
            targetVelocity,

            out VerticalSpeed);
    }


    public void Tick() {
        switch (_skillMoveType) {
            case SkillMoveType.AttachToOwner:
                AttachToOwnerTick();
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

    private void InitailPosition(Transform charTransform,Transform charHeightTransform) {
        _skillObject.transform.position= charTransform.position;
        _skillObject.ScaleTransform.localPosition = Vector3.zero;

        switch (_skillMoveType) {
            case SkillMoveType.AttachToOwner:
                _skillObject.transform.SetParent(charTransform);
                _skillObject.transform.position = charTransform.position;
                _skillObject.ScaleTransform.localPosition = Vector3.zero;
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
    private void AttachToOwnerTick() {
        if (_skillObject.SourceCharHeightTransform == null)
            return;

        //高度：直接同步角色高度（不是加、不是 lerp）
        float ownerHeight = _skillObject.SourceCharHeightTransform.localPosition.y;
        _skillheightComponent.UpdateHeight(ownerHeight);
    }
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

        if (_skillObject.HeightInfo.Min < 0) {
            _skillheightComponent.UpdateHeight( 0);
            _skillObject.StartDestroyTimer(_skillRt.OnHitDestroyDelay);
            return;
        }

        _skillObject.UpdateArrowRotation(VerticalSpeed, MoveSpeed);
        _skillheightComponent.AddHeight(VerticalSpeed * Time.deltaTime);

        VerticalSpeed -= GameSettingManager.Instance.PhysicConfig.GravityScale * Time.deltaTime;
    }


    public void SetFacingRight(Vector2 direction) {
        if (direction.sqrMagnitude < 0.01f) return;     //避免靜止時頻繁執行

        if (Mathf.Abs(direction.x) > 0.01f) {
            var scale = _skillObject.ScaleTransform.localScale;
            float mag = Mathf.Abs(scale.x);

            switch (_skillRt.FacingDirection) {
                case FacingDirection.Left:
                    scale.x = (direction.x < 0f) ? mag : -mag;
                    break;
                case FacingDirection.Right:
                    scale.x = (direction.x < 0f) ? -mag : mag;
                    break;
            }


            _skillObject.ScaleTransform.localScale = scale;
        }
    }
}
