using System.Threading;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class Action_MoveToPoint : Node {
    private AIComponent _ai;
    private MoveComponent _moveComponent;
    private MoveStrategyBase _moveStrategy;

    // 抵達判定（你可自己調）
    private const float ARRIVAL_THRESHOLD = 0.05f;

    public Action_MoveToPoint(AIComponent ai) {
        _ai = ai;
        _moveComponent = ai.MoveComponent;
        _moveStrategy = ai.MoveStrategy;
    }

    public override NodeState Evaluate(float updateInterval) {
        Vector2 currentPos = _ai.Transform.position;

        // === 已經在移動途中：只檢查是否到達 ===
        float dist = Vector2.Distance(currentPos, _ai.AutoMoveLastIntentPosition);
        if (dist <= ARRIVAL_THRESHOLD) {
            _ai.StopAutoMoveAttack();
            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;       // 尚未抵達 → 持續進行
    }
}
