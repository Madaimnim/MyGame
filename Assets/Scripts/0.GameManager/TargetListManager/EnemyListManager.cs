using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-50)]
public class EnemyListManager : TargetListManager {
    public static EnemyListManager Instance { get; private set; }

    private readonly List<Enemy> _enemyList = new();
    public IReadOnlyList<Enemy> EnemyList => _enemyList;

    protected override TextAnchor GUIAnchor => TextAnchor.UpperRight;
    protected override Vector2 GUICornerOffset => new Vector2(10, 10);

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void RegisterEnemy(Enemy enemy) {
        if (!_enemyList.Contains(enemy)) {
            _enemyList.Add(enemy);

            base.Register(enemy); // Also register in the base TargetListManager
        }
    }

    public void UnregisterEnemy(Enemy enemy) {
        if (_enemyList.Contains(enemy)) {
            _enemyList.Remove(enemy);

            base.Unregister(enemy); // Also unregister in the base TargetListManager
        }
    }
}
