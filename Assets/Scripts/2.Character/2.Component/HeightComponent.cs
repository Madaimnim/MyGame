using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using System.Collections;

public class HeightComponent
{
    public bool IsGrounded { get; private set; }

    Transform _sprTransform;
    MonoBehaviour _runner;
    Coroutine _floatCoroutine;
    float _groundY = float.NaN;

    public HeightComponent(Transform transform, MonoBehaviour runner)
    {
        _sprTransform = transform;
        _runner = runner;
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
        IsGrounded = false;
        // 若是第一次被擊飛，記錄地面高度
        if (float.IsNaN(_groundY)) _groundY = _sprTransform.localPosition.y;

        float baseY = _groundY;

        // ======= 物理參數 =======
        float gravity = PhysicManager.Instance.PhysicConfig.GravityScale;                 //重力加速度
        float verticalVelocity = floatPower;                                              //初始向上速度

        // ======= 迴圈：持續模擬直到角色落回地面 =======
        while (true)
        {
            var deltaTime = Time.deltaTime;
            // 垂直速度逐漸被重力拉低
            verticalVelocity -= gravity * deltaTime;

            _sprTransform.localPosition += new Vector3(0,verticalVelocity * deltaTime);

            // 偵測是否回到地面以下（表示落地）
            if (_sprTransform.localPosition.y <= baseY ) break;
           
            yield return null;
        }

        // ======= 落地校正 =======
        _sprTransform.localPosition = new Vector3(_sprTransform.localPosition.x, _groundY);

        IsGrounded = true;
        _groundY = float.NaN;
    }
}
