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
    private float _groundY = float.NaN;
    private Coroutine _knockbackCoroutine;
    private Collider2D _bottomCollider;

    //組件
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
    public void Knockbacked(Vector2 force, Transform source) {
        var dir = source.position.x-_rb.position.x >= 0 ? new Vector2(-1f,0) : new Vector2(1f, 0);

        if (_knockbackCoroutine != null)
        {
            Debug.Log("暫停了協程");
            _runner.StopCoroutine(_knockbackCoroutine);
            _knockbackCoroutine = null;
        }
        Debug.Log("啟動協程");
        _knockbackCoroutine = _runner.StartCoroutine(KnockbackCoroutine(force, dir));
    }

    private IEnumerator KnockbackCoroutine(Vector2 force,Vector2 direction)
    {
        // 若是第一次被擊飛，記錄地面高度
        if (float.IsNaN(_groundY)) _groundY = _rb.position.y;

        IsKnocked = true;

        float baseY = _groundY;
        Vector3 bottomGroundPos = _bottomCollider.transform.position;

        // ======= 物理參數 =======
        float gravity = PhysicManager.Instance.PhysicConfig.gravityScale;                 // 重力加速度
        float verticalVelocity = force.y;     // 初始向上速度
        float horizontalVelocity = force.x * direction.x; // 水平速度（不受重力影響）

        // ======= 迴圈：持續模擬直到角色落回地面 =======
        while (true)
        {
            float deltaTime = Time.deltaTime;

            // 垂直速度逐漸被重力拉低
            verticalVelocity -= gravity * deltaTime;

            // 位移應用
            _rb.position += new Vector2(
                horizontalVelocity * deltaTime,
                verticalVelocity * deltaTime
            );

            // 底部保持貼地（Y 固定，但 X 跟著走）
            _bottomCollider.transform.position = new Vector3(
                _rb.position.x,
                bottomGroundPos.y,
                _bottomCollider.transform.position.z
            );

            // 偵測是否回到地面以下（表示落地）
            if (_rb.position.y <= baseY && verticalVelocity < 0)
                break;

            yield return null;
        }

        // ======= 落地校正 =======
        _rb.position = new Vector3(_rb.position.x, baseY);
        _bottomCollider.transform.localPosition = Vector3.zero;

        IsKnocked = false;
        _groundY = float.NaN;
    }


    //protected virtual IEnumerator KnockbackCoroutine(float force, Vector2 knockbackDirection) {
    //    CanMove = false;
    //
    //    _rb.velocity = Vector2.zero; // 先清除當前速度，避免擊退力疊加
    //    _rb.AddForce(force * knockbackDirection, ForceMode2D.Impulse); // 添加瞬間衝擊力
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
