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
        // �Y�O�Ĥ@���Q�����A�O���a������
        if (float.IsNaN(_groundY)) _groundY = _sprTransform.localPosition.y;

        float baseY = _groundY;

        // ======= ���z�Ѽ� =======
        float gravity = PhysicManager.Instance.PhysicConfig.GravityScale;                 //���O�[�t��
        float verticalVelocity = floatPower;                                              //��l�V�W�t��

        // ======= �j��G����������쨤�⸨�^�a�� =======
        while (true)
        {
            var deltaTime = Time.deltaTime;
            // �����t�׳v���Q���O�ԧC
            verticalVelocity -= gravity * deltaTime;

            _sprTransform.localPosition += new Vector3(0,verticalVelocity * deltaTime);

            // �����O�_�^��a���H�U�]��ܸ��a�^
            if (_sprTransform.localPosition.y <= baseY ) break;
           
            yield return null;
        }

        // ======= ���a�ե� =======
        _sprTransform.localPosition = new Vector3(_sprTransform.localPosition.x, _groundY);

        IsGrounded = true;
        _groundY = float.NaN;
    }
}
