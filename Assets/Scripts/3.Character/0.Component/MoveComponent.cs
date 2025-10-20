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
    private float _groundY = float.NaN;
    private Coroutine _knockbackCoroutine;
    private Collider2D _bottomCollider;

    //�ե�
    private Rigidbody2D _rb;
    private AnimationComponent _animationComponent;
    private ICoroutineRunner _runner;

    public MoveComponent(Rigidbody2D rb,
        float moveSpeed , 
        ICoroutineRunner runner, 
        TargetDetector moveDetector,
        AnimationComponent animationComponent,
        Collider2D bottomCollider) {
        _rb = rb ;
        MoveSpeed = moveSpeed;
        _runner = runner;
        MoveDetector = moveDetector;
        _animationComponent = animationComponent;
        _bottomCollider = bottomCollider;
    }
    public void Tick() {
        TryMove();
    }
    private void TryMove() {
        if (!CanMove) return;

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
    public void Knockbacked(Vector2 force, Transform source) {
        var dir = source.position.x-_rb.position.x >= 0 ? new Vector2(-1f,0) : new Vector2(1f, 0);

        if (_knockbackCoroutine != null)
        {
            Debug.Log("�Ȱ��F��{");
            _runner.StopCoroutine(_knockbackCoroutine);
            _knockbackCoroutine = null;
        }
        Debug.Log("�Ұʨ�{");
        _knockbackCoroutine = _runner.StartCoroutine(KnockbackCoroutine(force, dir));
    }

    private IEnumerator KnockbackCoroutine(Vector2 force,Vector2 direction)
    {
        // �Y�O�Ĥ@���Q�����A�O���a������
        if (float.IsNaN(_groundY)) _groundY = _rb.position.y;

        IsKnocked = true;

        float baseY = _groundY;
        Vector3 bottomGroundPos = _bottomCollider.transform.position;

        // ======= ���z�Ѽ� =======
        float gravity = PhysicManager.Instance.PhysicConfig.gravityScale;                 // ���O�[�t��
        float verticalVelocity = force.y;     // ��l�V�W�t��
        float horizontalVelocity = force.x * direction.x; // �����t�ס]�������O�v�T�^

        // ======= �j��G����������쨤�⸨�^�a�� =======
        while (true)
        {
            float deltaTime = Time.deltaTime;

            // �����t�׳v���Q���O�ԧC
            verticalVelocity -= gravity * deltaTime;

            // �첾����
            _rb.position += new Vector2(
                horizontalVelocity * deltaTime,
                verticalVelocity * deltaTime
            );

            // �����O���K�a�]Y �T�w�A�� X ��ۨ��^
            _bottomCollider.transform.position = new Vector3(
                _rb.position.x,
                bottomGroundPos.y,
                _bottomCollider.transform.position.z
            );

            // �����O�_�^��a���H�U�]��ܸ��a�^
            if (_rb.position.y <= baseY && verticalVelocity < 0)
                break;

            yield return null;
        }

        // ======= ���a�ե� =======
        _rb.position = new Vector3(_rb.position.x, baseY);
        _bottomCollider.transform.localPosition = Vector3.zero;

        IsKnocked = false;
        _groundY = float.NaN;
    }


    //protected virtual IEnumerator KnockbackCoroutine(float force, Vector2 knockbackDirection) {
    //    CanMove = false;
    //
    //    _rb.velocity = Vector2.zero; // ���M����e�t�סA�קK���h�O�|�[
    //    _rb.AddForce(force * knockbackDirection, ForceMode2D.Impulse); // �K�[���������O
    //    yield return new WaitForSeconds(0.2f);
    //    _rb.velocity = Vector2.zero;
    //
    //    CanMove = true;
    //}




    public void DisableMove() => CanMove = false;
    public void Reset() {
        CanMove = true;
        IntentDirection = Vector2.zero;
        _rb.velocity = Vector2.zero;
    }
}
