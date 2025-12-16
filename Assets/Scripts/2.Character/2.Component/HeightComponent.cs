using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using System.Collections;

public class HeightComponent
{
    private float _initialHeightY;
    private float _recoverSpeed ;
    private Transform _sprTransform;
    private MonoBehaviour _runner;
    private StateComponent _stateComponent;

    Coroutine _floatCoroutine;
    Coroutine _recoverHeightCoroutine;

    public HeightComponent(Transform transform, MonoBehaviour runner,float initialHeightY,float recoverSpeed,StateComponent stateComponent)
    {
        _sprTransform = transform;
        _runner = runner;
        _initialHeightY= initialHeightY;
        _recoverSpeed = recoverSpeed;
        _stateComponent = stateComponent; 
    }


    public void FloatUp(float floatPower)
    {
        if (_floatCoroutine != null)
        {
            _runner.StopCoroutine(_floatCoroutine);
            _floatCoroutine = null;
        }
        _floatCoroutine = _runner.StartCoroutine(FloatCoroutine(floatPower));
    }
    private IEnumerator FloatCoroutine(float floatPower)
    {
        _stateComponent.SetIsGrounded (false);

        // ======= 物理參數 =======
        float gravity = PhysicManager.Instance.PhysicConfig.GravityScale;                 //重力加速度
        float verticalVelocity = floatPower;                                              //初始向上速度

        // ======= 迴圈：持續模擬直到角色落回地面 =======
        while (true)
        {
            var deltaTime = Time.deltaTime;
            verticalVelocity -= gravity * deltaTime;
            _sprTransform.localPosition += new Vector3(0,verticalVelocity * deltaTime);

            if(_sprTransform.localPosition.y <= 0 ) break;
            yield return null;
        }

        RecoverHeight(_recoverSpeed);
        _stateComponent.SetIsGrounded(true);
    }

    public void RecoverHeight(float speed) {
        if (_recoverHeightCoroutine != null) {
            _runner.StopCoroutine(_recoverHeightCoroutine);
            _recoverHeightCoroutine = null;
        }

        _recoverHeightCoroutine = _runner.StartCoroutine(RecoverHeightCoroutine(speed)
        );
    }
    private IEnumerator RecoverHeightCoroutine(float speed) {
        while (true) {
            float currentY = _sprTransform.localPosition.y;

            if (Mathf.Approximately(currentY, _initialHeightY))break;

            float newY = Mathf.MoveTowards(currentY,_initialHeightY,speed * Time.deltaTime);
            _sprTransform.localPosition = new Vector3(_sprTransform.localPosition.x,newY,_sprTransform.localPosition.z);

            yield return null;
        }

        _recoverHeightCoroutine = null;
        _stateComponent.SetIsGrounded(true);
    }

}
