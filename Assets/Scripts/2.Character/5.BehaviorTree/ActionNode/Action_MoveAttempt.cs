using System.Threading;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class Action_MoveAttempt : Node
{
    private AIComponent _ai;
    private MoveComponent _moveComp;
    private SkillComponent _skillComponent;
    private MoveStrategyBase _strategy;

    //攻擊目標檢測
    private const float ATK_INTERVAL = 0.2f;

    //最多持續移動時間
    private float _timer;
    private float _moveTime;

    //設定Intent
    private Vector2 _currentPos => _ai.SelfTransform.position;
    private Vector2? _direction;
    private Vector2? _targetPos;
    private Transform _targetTransform;
    
    // 抵達距離判定
    private const float ARRIVAL_THRESHOLD = 0.01f;
    
    //撞牆偵測
    private Vector2 _lastPosition;
    private float _stuckTimer;
    private const float STUCK_INTERVAL = 0.2f ;         // 每 0.2 秒檢查一次
    private const float STUCK_RATIO_THRESHOLD = 0.5f; // 實際移動量低於50%算卡住

    public Action_MoveAttempt(AIComponent ai, MoveStrategyBase strategy,MoveComponent moveComponent, SkillComponent skillComponent, float moveTime) {
        _ai = ai;
        _moveComp = moveComponent;
        _skillComponent = skillComponent;
        _strategy = strategy;

        _moveTime = moveTime;
        _timer = 0;
        _stuckTimer = 0f;
    }

    public override NodeState Evaluate(float updateInterval) {
        //有目標且冷卻完成，SUCCESS
        if (_timer >= ATK_INTERVAL && _skillComponent.HasAnyTarget)
        {
            ResetTimerAndIntent();
            return NodeState.SUCCESS;
        }

        if (_timer == 0)
        {
            _lastPosition = _ai.SelfTransform.position;

            _direction = _strategy.GetMoveDirection(_ai);
            _targetPos = _strategy.GetTargetPosition(_ai);
            _targetTransform = _strategy.GetTargetTransform(_ai);
            _ai.SetIntentMove(_moveComp, _direction, _targetPos, _targetTransform);
        }
        _timer += updateInterval;
        //卡住，SUCCESS
        _stuckTimer += updateInterval;
        if (_stuckTimer >= STUCK_INTERVAL)
        {
            float moved = Vector2.Distance(_currentPos, _lastPosition);
            float expected = _moveComp.MoveSpeed * _stuckTimer;  // 應該移動距離
            float ratio = moved / expected;

            if (ratio < STUCK_RATIO_THRESHOLD)
            {
                //Debug.Log($"Move偵測到卡住");
                ResetTimerAndIntent();
                return NodeState.SUCCESS;
            }

            _lastPosition = _currentPos;
            _stuckTimer = 0f;
        }

        //抵達，SUCCESS
        if (_targetPos.HasValue)
        {
            float dist = Vector2.Distance(_currentPos, _targetPos.Value);
            if (dist <= ARRIVAL_THRESHOLD)
            {
                ResetTimerAndIntent();
                return NodeState.SUCCESS;
            }
        }

        // 等待中
        if (_timer < _moveTime) return NodeState.RUNNING; 

        _timer = 0f;
        return NodeState.SUCCESS;
    }

    private void ResetTimerAndIntent() {
        _timer = 0f;
        _stuckTimer = 0f;
        _ai.SetIntentMove(_moveComp);
    }
}
