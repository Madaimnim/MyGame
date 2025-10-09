using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline;
using UnityEngine;
public class FollowMoveStrategy : MoveStrategyBase
{

    public override Vector2? GetMoveDirection(AIComponent ai) {
        var target = ai.GetMoveDetector().TargetTransform;
        var self = ai.SelfTransform;

        if (target == null) return null;
        return ((Vector2)(target.position - self.position)).normalized;
    }

    public override Transform GetTargetTransform(AIComponent ai) => ai.GetMoveDetector().TargetTransform;
}
