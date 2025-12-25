using System;
using System.Collections;
using UnityEngine;


public class MoveComponent
{
    public Transform IntentTargetTransform;
    public Vector2? IntentTargetPosition;
    public Vector2 IntentDirection;
    public TargetDetector MoveDetector { get; private set; }
    public float MoveSpeed { get; private set; }
    public float CurrentMoveSpeed { get; private set; }

    //移動相關
    private const float ARRIVAL_THRESHOLD = 0.05f;    //抵達判定距離
    private const float MOVEATTACK_SPEED = 0.3f;      //攻擊時的移動速度

    //敵人移動相關
    private float _moveDuration = 1f;
    private float _moveWindowRemainTime = 0f;
    private bool _useMoveWindow = false;
    public void MoveDuration(float duration) {
        CurrentMoveSpeed = MoveSpeed / duration;
        _moveWindowRemainTime = duration;
        _useMoveWindow= true;
    } 
    //事件
    public event Action<Vector2> OnMoveDirectionChanged;

    //擊退相關
    private Coroutine _knockbackCoroutine;
    //組件
    private Rigidbody2D _rb;
    private AnimationComponent _animationComponent;
    private MonoBehaviour _runner;
    private HeightComponent _heightComponent;
    private StateComponent _stateComponent;
    public MoveComponent(Rigidbody2D rb,float moveSpeed ,MonoBehaviour runner,TargetDetector moveDetector,AnimationComponent animationComponent,HeightComponent heightComponent,StateComponent stateComponent) {
        _rb = rb ;
        MoveSpeed = moveSpeed;
        CurrentMoveSpeed = MoveSpeed;
        _runner = runner;
        MoveDetector = moveDetector;
        _animationComponent = animationComponent;
        _heightComponent = heightComponent;

        _stateComponent = stateComponent;
    }
    public void Tick() {
        if(TryMove()) _stateComponent.SetIsMoving(true);
        else _stateComponent.SetIsMoving(false);
    }
    private bool TryMove() {
        if (!_stateComponent.CanMove) return false;

        // 若指定追蹤 Transform（AI 用）
        if (IntentTargetTransform != null) {
            Vector2 targetPos = IntentTargetTransform.position;
            Vector2 current = _rb.position;
            Vector2 dir = (targetPos - current).normalized;
            IntentDirection = dir;

            float dist = Vector2.Distance(current, targetPos);
            if (dist <= ARRIVAL_THRESHOLD) {
                IntentTargetTransform = null;  // 若你希望AI靠近就停，保留此行
                IntentTargetPosition = null;
                IntentDirection = Vector2.zero;
                return false;
            }
        }
        // 若有固定目標位置（玩家點擊地板）
        else if (IntentTargetPosition.HasValue) {
            Vector2 target = IntentTargetPosition.Value;
            Vector2 current = _rb.position;
            Vector2 dir = (target - current).normalized;
            IntentDirection = dir;

            float dist = Vector2.Distance(current, target);
            if (dist <= ARRIVAL_THRESHOLD) {
                IntentTargetPosition = null;
                IntentDirection = Vector2.zero;
                return false;
            }
        }
        // 若只有方向輸入（玩家 WASD）
        else if (IntentDirection == Vector2.zero)
            return false;// 沒有方向時不移動

        // 執行移動
        Vector2 newPosition = _rb.position + IntentDirection * CurrentMoveSpeed * Time.fixedDeltaTime;

        if (_stateComponent.IsPlayingAttackAnimation)
            newPosition = _rb.position + IntentDirection * CurrentMoveSpeed*MOVEATTACK_SPEED * Time.fixedDeltaTime;

        //如果沒在攻擊，則播放移動動畫，否則交由攻擊動畫控制
        if (!_stateComponent.IsPlayingAttackAnimation) {
            _animationComponent.PlayMove();
            OnMoveDirectionChanged?.Invoke(IntentDirection);
        }


        if (_useMoveWindow && _moveWindowRemainTime <= 0f) return false;
        if (_useMoveWindow) _moveWindowRemainTime -= Time.fixedDeltaTime;
        _rb.MovePosition(newPosition);


        return true;
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
        _stateComponent.SetIsKnocked(true);
        // ======= 迴圈：持續模擬直到角色落回地面 =======
        while (true)
        {
            yield return null;
            //Debug.Log($"{_isGrounded}");
            _rb.position += knockbackForce * Time.deltaTime;
            if (_stateComponent.IsGrounded) break;
        }
        _stateComponent.SetIsKnocked(false);
    }

    public void ResetVelocity() {
        IntentDirection = Vector2.zero;
        _rb.velocity = Vector2.zero;
    }
}
