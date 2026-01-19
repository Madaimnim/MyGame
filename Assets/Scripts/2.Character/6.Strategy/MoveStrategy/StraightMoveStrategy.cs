using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightMoveStrategy : MoveStrategyBase
{
    public override Vector2? GetMovePosition(AIComponent ai) {
        return new Vector2(ai.Transform.position.x-1, ai.Transform.position.y);
    }
}