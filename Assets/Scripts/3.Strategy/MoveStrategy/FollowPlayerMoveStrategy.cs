using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FollowPlayerMoveStrategy : MoveStrategyBase
{
    public override void MoveMethod() {
  
    }
    public override Vector2 MoveDirection() {
        return new Vector2(1, 1);
    }
}
