using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultEnemyFactory 
{
    public Enemy CreateEnemy(int id) {
        var tp = EnemyUtility.Get(id);
        var ob = Object.Instantiate(tp.VisualData.Prefab, GameManager.Instance.PlayerBattleParent);
        var enemy = ob.GetComponent<Enemy>();
        ob.transform.localPosition = Vector3.zero;

        enemy.Initialize(new EnemyStatsRuntime(tp));
        return enemy;
    }
}
