using System;
using UnityEngine;
using System.Collections;

public class MoveComponent
{
    public Transform IntentTargetTransform;
    public Vector2? IntentTargetPosition;
    public Vector2 IntentDirection;

    public TargetDetector MoveDetector { get; private set; }

    private const float ARRIVAL_THRESHOLD = 0.05f;   // 抵達判定距離
    private const float MOVEATTACK_SPEED = 0.3f;      //攻擊時的移動速度
        
    public bool IsMoving => IntentDirection != Vector2.zero;
    public bool CanMove { get; private set; } = true;

    private Rigidbody2D _rb;
    private AnimationComponent _animationComponent;
    private ICoroutineRunner _runner;

    public float MoveSpeed { get; private set; }


    public MoveComponent(Rigidbody2D rb,float moveSpeed, ICoroutineRunner runner, TargetDetector moveDetector,AnimationComponent animationComponent) {
        _rb = rb ;
        MoveSpeed = moveSpeed;
        _runner = runner;
        MoveDetector = moveDetector;
        _animationComponent = animationComponent;
    }

    public void Tick() {
        TryMove();
    }

    private void TryMove() {
        if (!CanMove) return;

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

    public void Knockbacked(float force, Vector2 knockbackDirection) {
        _runner.StartCoroutine(KnockbackCoroutine(force, knockbackDirection));
    }
    protected virtual IEnumerator KnockbackCoroutine(float force, Vector2 knockbackDirection) {
        CanMove = false;

        _rb.velocity = Vector2.zero; // 先清除當前速度，避免擊退力疊加
        _rb.AddForce(force * knockbackDirection, ForceMode2D.Impulse); // 添加瞬間衝擊力
        yield return new WaitForSeconds(0.2f);
        _rb.velocity = Vector2.zero;

        CanMove = true;
    }
    public void DisableMove() => CanMove = false;
    public void Reset() {
        CanMove = true;
        IntentDirection = Vector2.zero;
        _rb.velocity = Vector2.zero;
    }
}
