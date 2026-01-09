using UnityEngine;
using System;

public class EnemyCounter : MonoBehaviour {
    //事件敵人清空
    public event Action OnEnemyClear;
    private int _remainEnemy;
    private int _totalEnemy;


    private void Start() {
        _totalEnemy = CalculateTotalEnemyCount();
        _remainEnemy = _totalEnemy;
    }

    private int CalculateTotalEnemyCount() {
        int total = 0;
        foreach (var wave in GameManager.Instance.GameStageSystem.CurrentStageData.Waves)
            total += wave.SpawnCount;
        return total;
    }

    public void EnemyDefeated(int exp) {
        _remainEnemy--;
        ExperienceManager.Instance.AddExp(exp);

        if (_remainEnemy <= 0)
            OnEnemyClear?.Invoke();
    }
}
