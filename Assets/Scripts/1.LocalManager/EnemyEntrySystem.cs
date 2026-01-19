using UnityEngine;
using System.Collections;

public class EnemyEntrySystem : MonoBehaviour {
    [SerializeField] private MonoBehaviour _enemySpawnPositionProvider;
    [SerializeField] private MonoBehaviour _enemyTargetPositionProvider;

    private IPositionProvider SpawnProvider => _enemySpawnPositionProvider as IPositionProvider;
    private IPositionProvider TargetProvider => _enemyTargetPositionProvider as IPositionProvider;

    public IEnumerator EntryEnemy(Enemy enemy) {
        Vector2 spawnPos = SpawnProvider.GetPosition();
        enemy.transform.position = spawnPos;

        enemy.AIComponent.DisableAI();
        enemy.Ani.speed = 3f;               //加速進場動畫
        enemy.MoveComponent.MultiCurrentMoveSpeed(3f); //加速移動速度

        var move = enemy.MoveComponent;
        move.SetIgnoreMoveBounds(true);


        Vector2 targetPos = TargetProvider.GetPosition();
        move.SetIntentMovePosition(inputPosition: targetPos);

        while (Vector2.Distance(enemy.transform.position, targetPos) > 0.05f)
            yield return null;

        move.ClearAllMoveIntent();
        move.SetIgnoreMoveBounds(false);

        enemy.Ani.speed = 1f;               //還原進場動畫
        enemy.MoveComponent.ResetCurrentMoveSpeed(); //還原移動速度
    }
}
