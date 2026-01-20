using System.Collections;
using UnityEditor.VersionControl;
using UnityEngine;

//生成的敵人將訂閱Counter的計數
public class EnemySpawner : MonoBehaviour {
    
    [Header("敵人生成位置")]
    private DefaultEnemyFactory _enemyFactory;
    private EnemyCounter _enemyCounter;

    private void Awake() {
        _enemyCounter = GetComponent<EnemyCounter>();
        _enemyFactory = new DefaultEnemyFactory();
    }
    private void OnEnable() { }
    private void Start() {}

    public Enemy Spawn(int enemyId) {
        var enemy = _enemyFactory.CreateEnemy(enemyId);
        enemy.HealthComponent.OnDie += () => _enemyCounter.EnemyDefeated(enemy.Rt.Exp);
        return enemy;
    }
}
