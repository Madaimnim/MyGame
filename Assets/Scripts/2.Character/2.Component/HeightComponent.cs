using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

// 注意：HeightComponent 影響 StateComponent
// 若未來要共用高度物理，請抽 HeightPhysics，不要直接合併

public struct HeightInfo {
    public float Min;
    public float Max;
    public HeightInfo(float min, float max) {
        Min = min;
        Max = max;
    }
    public bool Overlaps(HeightInfo other) {
        return Max >= other.Min && other.Max >= Min;
    }
}

public enum HurtType{
    Common,
    Hard
}

public class HeightComponent
{
    private float _initialHeight;
    private Transform _sprTransform;
    private StateComponent _stateComponent;
    private AnimationComponent _animationComponent;

    private StatsData _statsData;
    private float _currentVerticalVelocity = 0f;
    private float _gravity => _gravityProvider();
    private Func<float> _gravityProvider;


    private bool _isGrounded => _sprTransform.localPosition.y <= 0.001f;
    private bool _isCanAttackHeight => Mathf.Abs(_sprTransform.localPosition.y - _initialHeight)<= CANATTACKRANGE;
    private const float CANATTACKRANGE = 0.05f;

    private MonoBehaviour _runner ;
    Coroutine _skillDashMoveCoroutine;
    Coroutine _skillPrepareMoveCoroutine;
    Coroutine _hurtRecoverCoroutine;


    public HeightComponent(Transform sprTransform,StateComponent stateComponent,
        AnimationComponent animationComponent, MonoBehaviour runner,StatsData statsData, Func<float> gravityProvider)
    {
        _sprTransform = sprTransform;
        _stateComponent = stateComponent;
        _animationComponent = animationComponent;
        _runner = runner;
        _statsData= statsData;

        _initialHeight = sprTransform.localPosition.y;

        _gravityProvider = gravityProvider;
    }

    public void Tick() {
        UpdateHeightTick();
        if (_stateComponent.IsRecoveringHeight) 
            RecoveryHeightTick();
        if (_stateComponent.ShoudApplyGravity)
            AddGravity();

        _stateComponent.SetIsGrounded(_isGrounded);
        _stateComponent.SetIsCanAttackHeight(_isCanAttackHeight);
    }
    public void AddGravity()=> _currentVerticalVelocity -= _gravity * Time.deltaTime;
    private void UpdateHeightTick() {
        if (Mathf.Abs(_currentVerticalVelocity) < 0.0001f) return;

        float newHeight = _sprTransform.localPosition.y + _currentVerticalVelocity * Time.deltaTime;
        if (newHeight <= 0f) {
            newHeight = 0f;
            _currentVerticalVelocity = 0f;
        }
        _sprTransform.localPosition = new Vector3(_sprTransform.localPosition.x, newHeight, _sprTransform.localPosition.z);
    }

    public void RecoveryHeightTick() {                                  //恢復高度
        float currentHeight = _sprTransform.localPosition.y;
        float nextHeight = Mathf.MoveTowards(currentHeight, _initialHeight, _statsData.VerticalMoveSpeed * Time.deltaTime);//保證不超過

        _sprTransform.localPosition = new Vector3(_sprTransform.localPosition.x, nextHeight, _sprTransform.localPosition.z);
        
        if(_isCanAttackHeight) _stateComponent.SetIsRecoveringHeight(false);   
    }

    public void Hurt(float duration,float floatPower, HurtType hurtType) {
        _stateComponent.SetIsHurt(true);

        StopSkillDashMoveCoroutine();
        StopSkillPrepareMoveCoroutine();

        if (hurtType == HurtType.Hard) _stateComponent.SetIsAntiGravity(false); 

        AddUpVelocity(floatPower);

        StopHurtRecoverCoroutine();
        _hurtRecoverCoroutine = _runner.StartCoroutine(HurtRecoverCoroutinne(duration));
    }

    public void AddUpVelocity(float upVelocity) => _currentVerticalVelocity += upVelocity;

    public void SkillPrepareMove(ISkillRuntime skillRt) {
        StopSkillPrepareMoveCoroutine();
        _skillPrepareMoveCoroutine = _runner.StartCoroutine(SkillPrepareMoveCoroutine(skillRt.SkillDashVerticalVelocity, skillRt));
    }
    public void SkillDashMove(ISkillRuntime skillRt) {
        StopSkillDashMoveCoroutine();
        _skillDashMoveCoroutine = _runner.StartCoroutine(SkillDashMoveCoroutine(skillRt.SkillDashVerticalVelocity, skillRt));
    }


    private IEnumerator HurtRecoverCoroutinne(float duration) {
        yield return new WaitForSeconds(duration);
        _stateComponent.SetIsHurt(false);
    }

    private IEnumerator SkillPrepareMoveCoroutine(float skillDashVerticalVelocity, ISkillRuntime skillRt) {
        int steps = Mathf.CeilToInt(skillRt.SkillDashPrepareDuration / Time.deltaTime);

        for (int i = 0; i < steps; i++) {
            float currentHeight = _sprTransform.localPosition.y;
            float nextHeight = currentHeight - skillDashVerticalVelocity * Time.deltaTime;

            //高度下限保護
            if (nextHeight <= 0f) _sprTransform.localPosition = new Vector3(_sprTransform.localPosition.x, 0, _sprTransform.localPosition.z);

            _sprTransform.localPosition = new Vector3(_sprTransform.localPosition.x, nextHeight, _sprTransform.localPosition.z);
            yield return null;
        }
        _animationComponent.SetParameterBool("IsPrepareReady", true);

        _skillPrepareMoveCoroutine = null;
    }
    private IEnumerator SkillDashMoveCoroutine(float skillDashVerticalVelocity, ISkillRuntime skillRt) {
        int steps = Mathf.CeilToInt(skillRt.SkillDashDuration / Time.deltaTime);
        _stateComponent.SetIsCanAttackHeight(false);
        for (int i = 0; i < steps; i++) {
            float currentHeight = _sprTransform.localPosition.y;
            float nextHeight = currentHeight + skillDashVerticalVelocity * Time.deltaTime;

            //高度下限保護
            if (nextHeight <= 0f) {
                _sprTransform.localPosition = new Vector3(_sprTransform.localPosition.x, 0f, _sprTransform.localPosition.z);
                break; //只跳出 for，不中斷協程
            }

            _sprTransform.localPosition = new Vector3(_sprTransform.localPosition.x, nextHeight, _sprTransform.localPosition.z);
            yield return null;
        }
        _skillDashMoveCoroutine = null;
    }


    public void StopHurtRecoverCoroutine() {
        if (_hurtRecoverCoroutine != null) {
            _runner.StopCoroutine(_hurtRecoverCoroutine);
            _hurtRecoverCoroutine = null;
        }
    }
    public void StopSkillDashMoveCoroutine() {
        if (_skillDashMoveCoroutine != null) {
            _runner.StopCoroutine(_skillDashMoveCoroutine);
            _skillDashMoveCoroutine = null;
        }
    }
    public void StopSkillPrepareMoveCoroutine() {
        if (_skillPrepareMoveCoroutine != null) {
            _runner.StopCoroutine(_skillPrepareMoveCoroutine);
            _skillPrepareMoveCoroutine = null;
        }
    }

    public void ResetInitialHeight() {
        _sprTransform.localPosition=new Vector3(0,_initialHeight,0);

        var pos = _sprTransform.localPosition;
        pos.y = _initialHeight;
        _sprTransform.localPosition = pos;
    }
}
