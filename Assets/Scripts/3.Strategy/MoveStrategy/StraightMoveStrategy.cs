using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightMoveStrategy : MoveStrategyBase
{
    public override void MoveMethod() {

    }
    public override Vector2 MoveDirection(EnemyAI enemyAI) {

        return new Vector2(-1,0);
    }
}