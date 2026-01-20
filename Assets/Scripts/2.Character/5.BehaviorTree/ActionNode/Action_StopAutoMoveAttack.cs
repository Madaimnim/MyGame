using System.Threading;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class Action_StopAutoMoveAttack : Node {
    private AIComponent _ai;
    private MoveComponent _moveComponent;
    private MoveStrategyBase _moveStrategy;

    // 抵達判定（你可自己調）
    private const float ARRIVAL_THRESHOLD = 0.05f;

    public Action_StopAutoMoveAttack(AIComponent ai) {
        _ai = ai;
        _moveComponent = ai.MoveComponent;
        _moveStrategy = ai.MoveStrategy;
    }

    public override NodeState Evaluate(float updateInterval) {
        if (_ai.AutoMoveTargetPosition == null) return NodeState.FAILURE;

        Vector2 currentPos = _ai.Transform.position;
        if (_moveComponent.IntentMovePosition != _ai.AutoMoveTargetPosition)
            _moveComponent.SetIntentMovePosition(_ai.AutoMoveTargetPosition);

        // === 已經在移動途中：只檢查是否到達 ===
        float dist = Vector2.Distance(currentPos, _ai.AutoMoveTargetPosition.Value);
        if (dist <= ARRIVAL_THRESHOLD) {
            _ai.StopAutoMoveAttack();
            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;       // 尚未抵達 → 持續進行
    }
}
