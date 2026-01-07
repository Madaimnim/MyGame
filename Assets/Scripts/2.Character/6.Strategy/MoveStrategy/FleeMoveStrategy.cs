using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeMoveStrategy : MoveStrategyBase
{
    public override Vector2? GetMoveDirection(AIComponent ai) {
        var target = ai.GetMoveDetector().TargetTransform;
        var self = ai.Transform;

        if (target == null) return null;
        return ((Vector2)(self.position- target.position)).normalized;
    }
}
