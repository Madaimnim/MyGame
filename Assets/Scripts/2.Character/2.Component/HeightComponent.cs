using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using System.Collections;

public class HeightComponent
{
    private float _initialHeight;
    private Transform _sprTransform;
    private StateComponent _stateComponent;
    private AnimationComponent _animationComponent;

    private StatsData _statsData;
    private float _verticalVelocity = 0f;
    private float _gravity;

    private MonoBehaviour _runner ;
    Coroutine _skillDashMoveCoroutine;
    Coroutine _skillPrepareMoveCoroutine;

    Coroutine _recoverHeightCoroutine;
    Coroutine _hurtCoroutine;

    //地面微小偏差容許值
    const float GROUND_EPSILON = 0.001f;

    public HeightComponent(Transform sprTransform,StateComponent stateComponent, AnimationComponent animationComponent, MonoBehaviour runner,StatsData statsData )
    {
        _sprTransform = sprTransform;
        _stateComponent = stateComponent;
        _animationComponent = animationComponent;
        _runner = runner;
        _statsData= statsData;

        _initialHeight = sprTransform.localPosition.y;
        _gravity = PhysicManager.Instance.PhysicConfig.GravityScale;


    }

    public void FixedTick() {
        if (_stateComponent.IsInGravityFall) GravityFall();
        else if (!_stateComponent.IsInitialHeight) RecoveryHeight();

        if (_stateComponent.IsGrounded) _stateComponent.SetIsInGravityFall(false);
    }

    //受傷，啟、閉重力
    public void Hurt(float duration=0f) {
        if (_hurtCoroutine != null) {
            _runner.StopCoroutine(_hurtCoroutine);
            _hurtCoroutine = null;
        }
        _hurtCoroutine = _runner.StartCoroutine(HurtCoroutine(duration));    
    }
    private IEnumerator HurtCoroutine(float duration) {
        int steps = Mathf.CeilToInt(duration / Time.fixedDeltaTime);


        _stateComponent.SetIsPlayingAttackAnimation(false);
        _stateComponent.SetIsGrounded(false);
        _stateComponent.SetIsInitialHeight(false);
        _stateComponent.SetIsInGravityFall(true);

        for (int i = 0; i < steps; i++) {
            yield return new WaitForFixedUpdate();
        }
        _animationComponent.PlayIdle();
        yield return null;
    }

    //浮空
    public void AddUpVelocity(float upVelocity) {
        _verticalVelocity= upVelocity;
        _stateComponent.SetIsGrounded(false);
        _stateComponent.SetIsInitialHeight(false);
        _stateComponent.SetIsInGravityFall(true);
    }

    //重力控制
    public void GravityFall() {
        float dt = Time.fixedDeltaTime;
        float currentHeight = _sprTransform.localPosition.y + _verticalVelocity * dt;
        if (currentHeight <= 0f) {
            UpdateHeight(0f);
            _stateComponent.SetIsGrounded(true);
        }else UpdateHeight(currentHeight);

        _verticalVelocity -= _gravity * dt;
    }


    //技能準備
    public void SkillPrepareMove(ISkillRuntime skillRt) {
        StopSkillPrepareMoveCoroutine();
        _skillPrepareMoveCoroutine = _runner.StartCoroutine(SkillPrepareMoveCoroutine(skillRt.SkillDashVerticalVelocity, skillRt));
    }
    private IEnumerator SkillPrepareMoveCoroutine(float skillDashVerticalVelocity, ISkillRuntime skillRt) {
        int steps = Mathf.CeilToInt(skillRt.SkillDashPrepareDuration / Time.fixedDeltaTime);

        for (int i = 0; i < steps; i++) {
            float currentHeight = _sprTransform.localPosition.y;
            float nextHeight = currentHeight - skillDashVerticalVelocity * Time.fixedDeltaTime;

            //高度下限保護
            if (nextHeight <= 0f)UpdateHeight(0f);

            UpdateHeight(nextHeight);
            yield return new WaitForFixedUpdate();
        }
        _animationComponent.SetParameterBool("IsPrepareReady", true);

        _skillPrepareMoveCoroutine = null;
    }

    //技能衝刺
    public void SkillDashMove(ISkillRuntime skillRt) {
        StopSkillDashMoveCoroutine();
        _skillDashMoveCoroutine = _runner.StartCoroutine(SkillDashMoveCoroutine(skillRt.SkillDashVerticalVelocity, skillRt));
    }
    private IEnumerator SkillDashMoveCoroutine(float skillDashVerticalVelocity, ISkillRuntime skillRt) {
        int steps = Mathf.CeilToInt(skillRt.SkillDashDuration / Time.fixedDeltaTime);
        _stateComponent.SetIsInitialHeight(false);
        for (int i = 0; i < steps; i++) {
            float currentHeight = _sprTransform.localPosition.y;
            float nextHeight = currentHeight + skillDashVerticalVelocity * Time.fixedDeltaTime;

            //高度下限保護
            if (nextHeight <= 0f) {
                UpdateHeight(0f);
                break; //只跳出 for，不中斷協程
            }

            UpdateHeight(nextHeight);
            yield return new WaitForFixedUpdate();
        }
        _skillDashMoveCoroutine = null;
    }


    //恢復高度
    public void RecoveryHeight() {
        float currentHeight = _sprTransform.localPosition.y;
        float nextHeight = Mathf.MoveTowards(currentHeight, _initialHeight, _statsData.VerticalMoveSpeed * Time.fixedDeltaTime);
        
        UpdateHeight(nextHeight);

        if (Mathf.Approximately(nextHeight, _initialHeight)) {
            _sprTransform.localPosition = new Vector3(_sprTransform.localPosition.x, _initialHeight, _sprTransform.localPosition.z);
            _stateComponent.SetIsInitialHeight(true);
        }
    }

    //暫停協程
    public void StopSkillDashMoveCoroutine() {
        if (_skillDashMoveCoroutine != null) {
            _runner.StopCoroutine(_skillDashMoveCoroutine);
            _skillDashMoveCoroutine = null;
        }
    }
    public void StopRecoverHeightCoroutine() {
        if (_recoverHeightCoroutine != null) {
            _runner.StopCoroutine(_recoverHeightCoroutine);
            _recoverHeightCoroutine = null;
        }
    }
    public void StopSkillPrepareMoveCoroutine() {
        if (_skillPrepareMoveCoroutine != null) {
            _runner.StopCoroutine(_skillPrepareMoveCoroutine);
            _skillPrepareMoveCoroutine = null;
        }
    }

    //更新高度
    private void UpdateHeight(float y) {
        _sprTransform.localPosition = new Vector3(_sprTransform.localPosition.x, y, _sprTransform.localPosition.z);
    }
}
