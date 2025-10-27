using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightMoveStrategy : MoveStrategyBase
{
    public override Vector2? GetMoveDirection(AIComponent ai) {
        return new Vector2(-1, 0);
    }
}