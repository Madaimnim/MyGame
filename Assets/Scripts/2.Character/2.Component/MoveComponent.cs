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

    //移動相關
    private const float ARRIVAL_THRESHOLD = 0.05f;    //抵達判定距離
    private const float MOVEATTACK_SPEED = 0.3f;      //攻擊時的移動速度

    //擊退相關
    private Coroutine _knockbackCoroutine;
    private bool _isGrounded => _heightComponent.IsGrounded;

    //組件
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
        // 若指定追蹤 Transform（AI 用）
        if (IntentTargetTransform != null)
        {
            Vector2 targetPos = IntentTargetTransform.position;
            Vector2 current = _rb.position;
            Vector2 dir = (targetPos - current).normalized;
            IntentDirection = dir;

            float dist = Vector2.Distance(current, targetPos);
            if (dist <= ARRIVAL_THRESHOLD)
            {
                IntentTargetTransform = null;  // 若你希望AI靠近就停，保留此行
                IntentTargetPosition = null;
                IntentDirection = Vector2.zero;
                return;
            }
        }
        // 若有固定目標位置（玩家點擊地板）
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
        // 若只有方向輸入（玩家 WASD）
        else if (IntentDirection == Vector2.zero)
            return; // 沒有方向時不移動

        // 執行移動
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
        // ======= 迴圈：持續模擬直到角色落回地面 =======
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
