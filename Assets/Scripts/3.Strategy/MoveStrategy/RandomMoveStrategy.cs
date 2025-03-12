using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMoveStrategy : MoveStrategyBase
{
    public override void MoveMethod() {

    }
    public override Vector2 MoveDirection() {
        float rand = Random.value; // 取得 0~1 之間的隨機數
        Vector2 direction;

        if (rand < 0.5f) // 50% 機率
        {
            direction = new Vector2(-1, 0).normalized;
        }
        else if (rand < 0.75f) // 25% 機率
        {
            direction = new Vector2(-1, 1).normalized;
        }
        else // 25% 機率
        {
            direction = new Vector2(-1, -1).normalized;
        }
        return direction;
    }
}
