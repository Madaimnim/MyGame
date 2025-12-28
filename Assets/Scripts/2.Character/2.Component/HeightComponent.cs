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
    private bool _gravityEnabled = false;
    private float _verticalVelocity = 0f;
    private float _gravity;

    private MonoBehaviour _runner ;
    Coroutine _recoverHeightCoroutine;
    Coroutine _skillVerticalMoveCoroutine;
    Coroutine _hurtCoroutine;

    public HeightComponent(Transform sprTransform,StateComponent stateComponent, AnimationComponent animationComponent, MonoBehaviour runner,StatsData statsData )
    {
        _sprTransform = sprTransform;
        _stateComponent = stateComponent;
        _animationComponent = animationComponent;
        _runner = runner;
        _statsData= statsData;

        _initialHeight = sprTransform.localPosition.y;
        _gravity = PhysicManager.Instance.PhysicConfig.GravityScale;

        if (_sprTransform.localPosition.y >0) _stateComponent.SetIsGrounded(false);
        else _stateComponent.SetIsGrounded(true);
    }

    public void FixedTick() {

        if (!_gravityEnabled) return;

        float dt = Time.fixedDeltaTime;
        float currentHeight = _sprTransform.localPosition.y + _verticalVelocity * dt;

        if (currentHeight <= 0f) {
            currentHeight = 0f;
            _gravityEnabled = false;
            _verticalVelocity = 0f;
            _stateComponent.SetIsGrounded(true);
        }

        UpdateHeight(currentHeight);

        _verticalVelocity -= _gravity * dt;
    }

    public void Hurt(float duration=0f) {
        if (_hurtCoroutine != null) {
            _runner.StopCoroutine(_hurtCoroutine);
            _hurtCoroutine = null;
        }
        _hurtCoroutine = _runner.StartCoroutine(HurtCoroutine(duration));    
    }

    private IEnumerator HurtCoroutine(float duration) {
        Debug.Log($"受傷持續時間:{duration}");
        int steps = Mathf.CeilToInt(duration / Time.fixedDeltaTime);
        EnableGravity();

        for (int i = 0; i < steps; i++) {
            yield return new WaitForFixedUpdate();
        }

        DisableGravity();
        RecoveryHeight(_statsData.VerticalMoveSpeed);
        yield return null;
    }

    public void FloatUp(float floatPower) {
        _verticalVelocity= floatPower;
    }

    public void EnableGravity() {
        _stateComponent.SetIsGrounded(false);
        _stateComponent.SetIsInitialHeight(false);

        _gravityEnabled = true;
    }
    public void DisableGravity(bool snapToGround = false) {
        _gravityEnabled = false;
        _verticalVelocity = 0f;
    }
    private void UpdateHeight(float y) {
        _sprTransform.localPosition = new Vector3(_sprTransform.localPosition.x,y,_sprTransform.localPosition.z);
    }


    public void SkillVerticalMove(ISkillRuntime skillRt) {
        _gravityEnabled = false;

        StopSkillVerticalMoveCoroutine();

        _recoverHeightCoroutine = _runner.StartCoroutine(SkillVerticalMoveCoroutine(skillRt.SkillDashVerticalVelocity, skillRt.SkillDashDuration));
    }
    public void RecoveryHeight(float verticalMovespeed) {
        _animationComponent.PlayImmediate("Idle");
        _gravityEnabled = false;

        StopRecoverHeightCoroutine();

        _recoverHeightCoroutine = _runner.StartCoroutine(RecoveryHeightCoroutine(verticalMovespeed));
    }

    private IEnumerator SkillVerticalMoveCoroutine(float skillDashVerticalVelocity, float skillDashDuration) {
        int steps = Mathf.CeilToInt(skillDashDuration / Time.fixedDeltaTime);

        //Debug.Log($"技能垂直移動 啟動 高度速度:{skillDashVerticalVelocity} 持續時間:{skillDashDuration}");

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
    }
    private IEnumerator RecoveryHeightCoroutine(float verticalMovespeed) {
        while (true) {
            float currentHeight = _sprTransform.localPosition.y;
            float nextHeight = Mathf.MoveTowards(currentHeight, _initialHeight, verticalMovespeed * Time.fixedDeltaTime);

            UpdateHeight(nextHeight);

            if (Mathf.Approximately(nextHeight, _initialHeight)) {
                _sprTransform.localPosition = new Vector3(_sprTransform.localPosition.x, _initialHeight, _sprTransform.localPosition.z);
                _stateComponent.SetIsInitialHeight(true);
                //Debug.Log($"高度恢復完成:{_stateComponent.IsInitialHeight}");
                break;
            }


            yield return new WaitForFixedUpdate();//關鍵
        }

        _recoverHeightCoroutine = null;
        _stateComponent.SetIsGrounded(true);
    }

    public void StopSkillVerticalMoveCoroutine() {
        if (_skillVerticalMoveCoroutine != null) {
            _runner.StopCoroutine(_skillVerticalMoveCoroutine);
            _skillVerticalMoveCoroutine = null;
        }
    }
    public void StopRecoverHeightCoroutine() {
        if (_recoverHeightCoroutine != null) {
            _runner.StopCoroutine(_recoverHeightCoroutine);
            _recoverHeightCoroutine = null;
        }
    }
}
