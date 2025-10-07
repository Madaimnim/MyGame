using System;
using UnityEngine;
using System.Collections;

public class MoveComponent
{
    public Vector2 IntentDirection;
    public Vector2? IntentTargetPosition;           // 動目標位置
    private const float ARRIVAL_THRESHOLD = 0.2f;   // 抵達判定距離

    public bool IsMoving => _rb.velocity != Vector2.zero;
    public bool CanMove { get; private set; } = true;

    private Rigidbody2D _rb;
    private float _moveSpeed;
    private ICoroutineRunner _runner;



    public MoveComponent(Rigidbody2D rb,float moveSpeed, ICoroutineRunner runner) {
        _rb = rb ;
        _moveSpeed = moveSpeed;
        _runner = runner;
    }

    public void Move() {
        if (!CanMove)  return;

        if (IntentTargetPosition.HasValue)
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

        if (IntentDirection == Vector2.zero) return;
        Vector2 newPosition = _rb.position + IntentDirection * _moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(newPosition);                // 用 Rigidbody2D 物理方式移動，會自動和其他 Collider2D 發生碰撞
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
