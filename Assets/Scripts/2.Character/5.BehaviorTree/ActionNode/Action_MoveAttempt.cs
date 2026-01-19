using System.Threading;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

//Todo待重構!
public class Action_MoveAttempt : Node {
    private readonly AIComponent _ai;
    private readonly MoveComponent _move;
    private readonly CombatComponent _combat;
    private readonly MoveStrategyBase _strategy;


    // 移動控制
    //private readonly float _maxMoveTime;
    //private float _timer;

    // Intent
    private Vector2 _currentPos => _ai.Transform.position;
    private Vector2? _direction;
    private Vector2? _targetPos;
    private Transform _targetTransform;

    // 抵達
    private const float ARRIVAL_THRESHOLD = 0.01f;

    // 卡住偵測
    private Vector2 _lastPosition;
    private float _stuckTimer;
    private const float STUCK_INTERVAL = 0.2f;
    private const float STUCK_RATIO_THRESHOLD = 0.5f;

    public Action_MoveAttempt(AIComponent ai) {
        _ai = ai;
        _move = ai.MoveComponent;
        _combat = ai.CombatComponent;
        _strategy = ai.MoveStrategy;

        _stuckTimer = 0f;
    }

    public override NodeState Evaluate(float updateInterval) {
        // 1. 有攻擊目標 → 提前結束
        if (_combat.HasAnySkillTarget) return SuccessFinish();

        // 2.設定移動 Intent
        SetupMoveIntent();

        // 3. 更新計時、卡住檢測
        _stuckTimer += updateInterval;
        if (IsStuck()) return SuccessFinish();

        // 4. 抵達檢測
        if (HasArrived()) return SuccessFinish();

        return NodeState.RUNNING;
    }

    // ================= 狀態判斷 =================

    private bool HasArrived() {
        if (!_targetPos.HasValue) return false;
        return Vector2.Distance(_currentPos, _targetPos.Value) <= ARRIVAL_THRESHOLD;
    }

    private bool IsStuck() {
        if (_stuckTimer < STUCK_INTERVAL)
            return false;

        float moved = Vector2.Distance(_currentPos, _lastPosition);
        float expected = _move.MoveSpeed * _stuckTimer;
        float ratio = expected <= 0 ? 1f : moved / expected;

        _lastPosition = _currentPos;
        _stuckTimer = 0f;

        return ratio < STUCK_RATIO_THRESHOLD;
    }

    // ================= 狀態更新 =================

    private void SetupMoveIntent() {
        _lastPosition = _currentPos;

        _targetPos = _strategy.GetMovePosition(_ai);
        _targetTransform = _strategy.GetMoveTransform(_ai);

        _move.SetIntentMovePosition(_targetPos, _targetTransform);
    }


    // ================= 結束處理 =================

    private NodeState SuccessFinish() {
        _stuckTimer = 0f;
        _move.ClearAllMoveIntent();
        return NodeState.SUCCESS;
    }
}
