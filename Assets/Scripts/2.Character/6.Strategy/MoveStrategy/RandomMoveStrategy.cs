using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMoveStrategy : MoveStrategyBase
{
    private float _minDistance = 3f;
    private float _maxDistance = 3f;

    public override Vector2? GetMovePosition(AIComponent ai) {
        Vector2 origin = ai.Transform.position;

        // 隨機方向（單位向量）
        Vector2 dir = Random.insideUnitCircle.normalized;

        // 隨機距離（在 min-max 範圍）
        float distance = Random.Range(_minDistance, _maxDistance);

        // 最終目標
        Vector2 target = origin + dir * distance;

        return target;
    }
}
