using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;

//只用IntentMovePosition移動
//低階持續行為元件，透過IntentMovePosition持續移動到目標
public class MoveComponent
{
    public Vector2? IntentMovePosition { get; private set; }

    public Vector2 GetCurrentMoveVelocity()=> _currentMoveDirection* CurrentMoveSpeed;
    private Vector2 _currentMoveDirection;

    public float MoveSpeed { get; private set; }
    private float _pendingMoveSpeed;
    public float VerticalMoveSpeed { get; private set; }
    public float CurrentMoveSpeed { get; private set; }

    //移動相關
    private const float ARRIVAL_THRESHOLD = 0.05f;    //抵達判定距離
    private const float MOVEATTACK_SPEED = 0.3f;      //攻擊時的移動速度

    //敵人移動相關---------------------------------
    private float _moveDuration = 1f;
    private float _moveWindowRemainTime = 0f;
    private bool _useMoveWindow = false;

    //入場移動控制相關
    public bool IgnoreMoveBounds { get; private set; } = false;
    public void SetIgnoreMoveBounds(bool value) {IgnoreMoveBounds = value;}

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

    //技能發動移動相關-----------------------------------
    private Coroutine _skillDashMoveCoroutine;
    private Coroutine _skillPrepareMoveCoroutine;
    private Vector2 _skillDashDirection;
    public void SetSkillDashDirection(Vector2 dir) => _skillDashDirection = dir.normalized;

    public MoveComponent(Rigidbody2D rb,StatsData statsData ,MonoBehaviour runner,AnimationComponent animationComponent,HeightComponent heightComponent,StateComponent stateComponent) {
        _rb = rb ;
        MoveSpeed = statsData.MoveSpeed;
        VerticalMoveSpeed= statsData.VerticalMoveSpeed;
        CurrentMoveSpeed = MoveSpeed;
        _runner = runner;
        _animationComponent = animationComponent;
        _heightComponent = heightComponent;

        _stateComponent = stateComponent;
    }
    public void FixedTick() {
        bool isMoving = TryMove();
        _stateComponent.SetIsMoving(isMoving);
    }

    private bool TryMove() {
        if (!_stateComponent.CanMove) return false;
        if (IntentMovePosition == null) return false;

        _currentMoveDirection = (IntentMovePosition.Value - _rb.position).normalized;
        
        //到達位置，停止移動
        float dist = Vector2.Distance(_rb.position, IntentMovePosition.Value);
        if (dist <= ARRIVAL_THRESHOLD) ClearAllMoveIntent();


        Vector2 newPosition = _rb.position + _currentMoveDirection * CurrentMoveSpeed * Time.fixedDeltaTime;

        _animationComponent.PlayMove();
         OnMoveDirectionChanged?.Invoke(_currentMoveDirection);

        if (_useMoveWindow && _moveWindowRemainTime <= 0f) return false;
        if (_useMoveWindow) _moveWindowRemainTime -= Time.fixedDeltaTime;

        //處理移動邊界
        Vector2 resolvedPos = newPosition;
        if (!IgnoreMoveBounds && MoveBoundsManager.Instance != null)
            resolvedPos = MoveBoundsManager.Instance.Resolve(newPosition);
        _rb.MovePosition(resolvedPos);

        return true;
    }



    //技能衝刺
    public void SkillDashMove(ISkillRuntime skillRt) {
        StopSkillDashMoveCoroutine();
        _skillDashMoveCoroutine = _runner.StartCoroutine(SkillDashMoveCoroutine(_skillDashDirection, skillRt));
    }
    private IEnumerator SkillDashMoveCoroutine(Vector2 direction, ISkillRuntime skillRt) {
        _stateComponent.SetIsSkillDashing(true);
        float elapsed = 0f;
        float dashSpeed = MoveSpeed * skillRt.SkillDashMultiplier;
        Vector2 moveVelocity = direction * dashSpeed;
    
        while (elapsed < skillRt.SkillDashDuration) {
            Vector2 before = _rb.position;
            Vector2 expected = before + moveVelocity * Time.fixedDeltaTime;

            //處理移動邊界
            var resolvedPos = MoveBoundsManager.Instance != null ? MoveBoundsManager.Instance.Resolve(expected) : expected;
            _rb.MovePosition(resolvedPos);

            yield return new WaitForFixedUpdate();
            Vector2 after = _rb.position;
            //Debug.Log($"[DashCheck] frame={Time.frameCount} " +$"before={before} expected={expected} after={after}");
            elapsed += Time.fixedDeltaTime;
        }

        //衝刺結束後的緩衝時間
        float dashRecoverTime = skillRt.SkillDashDuration*0.2f;
        elapsed = 0f;
        while (elapsed < dashRecoverTime) {
            yield return new WaitForFixedUpdate();

            elapsed += Time.fixedDeltaTime;
        }

        _stateComponent.SetIsSkillDashing(false);
        _stateComponent.SetIsCastingSkill(false);

        _skillDashMoveCoroutine = null;
    }

    //技能準備
    public void SkillPrepareMove(ISkillRuntime skillRt) {
        StopSkillPrepareMoveCoroutine();
        _skillPrepareMoveCoroutine = _runner.StartCoroutine(SkillPrepareMoveCoroutine(_skillDashDirection, skillRt));
    }
    private IEnumerator SkillPrepareMoveCoroutine(Vector2 direction, ISkillRuntime skillRt) {
        _stateComponent.SetIsSkillDashing(true);
        float elapsed = 0f;
        float dashSpeed = MoveSpeed * skillRt.SkillDashMultiplier;
        Vector2 moveVelocity = -direction * dashSpeed*0.1f;

        while (elapsed < skillRt.SkillDashPrepareDuration) {
            Vector2 newPos = _rb.position + moveVelocity * Time.fixedDeltaTime;

            //處理移動邊界
            var resolvedPos = MoveBoundsManager.Instance != null ? MoveBoundsManager.Instance.Resolve(newPos) : newPos;
            _rb.MovePosition(resolvedPos);

            yield return new WaitForFixedUpdate(); // 關鍵

            elapsed += Time.fixedDeltaTime;
        }
        _animationComponent.SetParameterBool("IsPrepareReady", true);

        _skillPrepareMoveCoroutine = null;
    }

    //被擊退
    public void Knockbacked(InteractInfo info) {
        if (info.KnockbackPower == 0f) return;
        StopKnockbackedCoroutine();

        var knockbackDirection = Vector2.zero;
        if(info.MoveVelocity!=Vector2.zero)
            knockbackDirection = info.MoveVelocity.normalized;
        else knockbackDirection = ((Vector2)info.SourcePosition - _rb.position).normalized;

        var knockbackVector = info.KnockbackPower * knockbackDirection;
        //Debug.Log($"{_rb.gameObject.name}被擊退，方向力道{knockbackVector}");
        _knockbackCoroutine = _runner.StartCoroutine(KnockbackCoroutine(knockbackVector));
    }
    private IEnumerator KnockbackCoroutine(Vector2 knockbackVector)
    {
        _stateComponent.SetIsKnocked(true);
        while (true)
        {
            yield return new WaitForFixedUpdate();

            Vector2 current = _rb.position;
            Vector2 next = current + knockbackVector * Time.fixedDeltaTime;
            //  關鍵：擊退也必須 Resolve
            next = MoveBoundsManager.Instance != null ? MoveBoundsManager.Instance.Resolve(next) : next;

            _rb.MovePosition(next);

            if (_stateComponent.IsGrounded) break;
        }
        _stateComponent.SetIsKnocked(false);
    }
    public void StopKnockbackedCoroutine() {
        if (_knockbackCoroutine != null) {
            _runner.StopCoroutine(_knockbackCoroutine);
            _knockbackCoroutine = null;
        }
    }


    //重置速度
    public void ResetVelocity() {
        _currentMoveDirection = Vector2.zero;
        _rb.velocity = Vector2.zero;
    }

    //停止協程
    public void StopSkillDashMoveCoroutine() {
        if (_skillDashMoveCoroutine != null) {
            _runner.StopCoroutine(_skillDashMoveCoroutine);
            _skillDashMoveCoroutine = null;

            _stateComponent.SetIsSkillDashing(false);
        }
    }
    public void StopSkillPrepareMoveCoroutine() {
        if (_skillPrepareMoveCoroutine != null) {
            _runner.StopCoroutine(_skillPrepareMoveCoroutine);
            _skillPrepareMoveCoroutine = null;
        }
    }


    public void SetIntentMovePosition(Vector2? inputPosition = null, Transform inputTargetTransform = null) {
        if (inputTargetTransform != null)
            IntentMovePosition = inputTargetTransform.position;
        else if (inputPosition.HasValue)
            IntentMovePosition = inputPosition;
    }

    public void ClearAllMoveIntent() {
        IntentMovePosition = null;
        _currentMoveDirection = Vector2.zero;
    }

    public void MultiCurrentMoveSpeed(float multiple) {
        _pendingMoveSpeed= CurrentMoveSpeed;
        CurrentMoveSpeed *= multiple;
    }
    public void ResetCurrentMoveSpeed() {
        CurrentMoveSpeed = _pendingMoveSpeed;
    }
}
