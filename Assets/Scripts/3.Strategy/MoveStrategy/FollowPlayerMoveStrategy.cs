using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FollowPlayerMoveStrategy : MoveStrategyBase
{
    public override void MoveMethod() {
  
    }
    public override Vector2 MoveDirection(EnemyAI enemyAI) {
        Vector2 direction = new Vector2(enemyAI.currentMoveTarget.position.x- enemyAI.transform.position.x, enemyAI.currentMoveTarget.position.y - enemyAI.transform.position.y).normalized;
        return direction;
    }
}
