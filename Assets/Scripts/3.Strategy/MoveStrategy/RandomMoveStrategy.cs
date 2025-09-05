using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMoveStrategy : MoveStrategyBase
{
    public override void MoveMethod() {

    }
    public override Vector2 MoveDirection(EnemyAI enemyAI) {
        float rand = Random.value; // 取得 0~1 之間的隨機數
        Vector2 direction;

        if (rand < 0.25f) // 25% 機率
        {
            direction = new Vector2(-1, 1).normalized*0.5f;
        }
        else if (rand < 0.5f) // 25% 機率
        {
            direction = new Vector2(1, 1).normalized * 0.5f;
        }
        else if(rand<0.75f)// 25% 機率
        {
            direction = new Vector2(-1, -1).normalized * 0.5f;
        }
        else
            direction = new Vector2(1, -1).normalized * 0.5f;

        return direction;
    }
}
