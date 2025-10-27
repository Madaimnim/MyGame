using System;
using System.Collections;
using UnityEngine;


public class MoveComponent
{
    public Transform IntentTargetTransform;
    public Vector2? IntentTargetPosition;
    public Vector2 IntentDirection;
    public TargetDetector MoveDetector { get; private set; }
    public bool IsMoving => IntentDirection != Vector2.zero;
    public bool IsKnocked { get; private set; } = false;

    public bool CanMove { get; private set; } = true;
    public float MoveSpeed { get; private set; }

    //���ʬ���
    private const float ARRIVAL_THRESHOLD = 0.05f;    //��F�P�w�Z��
    private const float MOVEATTACK_SPEED = 0.3f;      //�����ɪ����ʳt��

    //���h����
    private Coroutine _knockbackCoroutine;
    private bool _isGrounded => _heightComponent.IsGrounded;

    //�ե�
    private Rigidbody2D _rb;
    private AnimationComponent _animationComponent;
    private MonoBehaviour _runner;
    private HeightComponent _heightComponent;

    public MoveComponent(Rigidbody2D rb,
        float moveSpeed ,
        MonoBehaviour runner, 
        TargetDetector moveDetector,
        AnimationComponent animationComponent,
        HeightComponent heightComponent
        ) {
        _rb = rb ;
        MoveSpeed = moveSpeed;
        _runner = runner;
        MoveDetector = moveDetector;
        _animationComponent = animationComponent;
        _heightComponent = heightComponent;
    }
    public void Tick() {
        TryMove();
    }
    private void TryMove() {
        if (!CanMove) return;
        if (IsKnocked) return;
        // �Y���w�l�� Transform�]AI �Ρ^
        if (IntentTargetTransform != null)
        {
            Vector2 targetPos = IntentTargetTransform.position;
            Vector2 current = _rb.position;
            Vector2 dir = (targetPos - current).normalized;
            IntentDirection = dir;

            float dist = Vector2.Distance(current, targetPos);
            if (dist <= ARRIVAL_THRESHOLD)
            {
                IntentTargetTransform = null;  // �Y�A�Ʊ�AI�a��N���A�O�d����
                IntentTargetPosition = null;
                IntentDirection = Vector2.zero;
                return;
            }
        }
        // �Y���T�w�ؼЦ�m�]���a�I���a�O�^
        else if (IntentTargetPosition.HasValue)
        {
            Vector2 target = IntentTargetPosition.Value;
            Vector2 current = _rb.position;
            Vector2 dir = (target - current).normalized;
            IntentDirection = dir;

            float dist = Vector2.Distance(current, target);
            if (dist <= ARRIVAL_THRESHOLD)
            {
                IntentTargetPosition = null;
                IntentDirection = Vector2.zero;
                return;
            }
        }
        // �Y�u����V��J�]���a WASD�^
        else if (IntentDirection == Vector2.zero)
            return; // �S����V�ɤ�����

        // ���沾��
        Vector2 newPosition = _rb.position + IntentDirection * MoveSpeed * Time.fixedDeltaTime;
        if (_animationComponent.IsPlayingAttackAnimation)
            newPosition= _rb.position + IntentDirection * MOVEATTACK_SPEED * Time.fixedDeltaTime;
        _rb.MovePosition(newPosition);
    }
    public void Knockbacked(Vector2 knockbackForce, Transform source) {

        if (_knockbackCoroutine != null)
        {
            _runner.StopCoroutine(_knockbackCoroutine);
            _knockbackCoroutine = null;
        }
        _knockbackCoroutine = _runner.StartCoroutine(KnockbackCoroutine(knockbackForce));
    }

    private IEnumerator KnockbackCoroutine(Vector2 knockbackForce)
    {
        IsKnocked = true;
        // ======= �j��G����������쨤�⸨�^�a�� =======
        while (true)
        {
            yield return null;
            //Debug.Log($"{_isGrounded}");
            _rb.position += knockbackForce * Time.deltaTime;
            if (_isGrounded) break;
        }
        IsKnocked = false;
    }


    public void DisableMove() => CanMove = false;
    public void Reset() {
        CanMove = true;
        IntentDirection = Vector2.zero;
        _rb.velocity = Vector2.zero;
    }
}
