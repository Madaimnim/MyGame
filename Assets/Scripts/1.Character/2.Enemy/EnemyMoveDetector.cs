using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class EnemyMoveDetector : MonoBehaviour
{
    [Header("敵人本體")]
    public Enemy enemy;
    public EnemyAI enemyAI;

    [Header("偵測目標")]
    public LayerMask targetLayers;

    // 儲存偵測到的目標
    private readonly HashSet<Transform> targetTransforms = new();

    private void OnEnable() {
        GameEventSystem.Instance.Event_OnPlayerDie += HandlePlayerDie;
    }
    private void OnDisable() {
        GameEventSystem.Instance.Event_OnPlayerDie -= HandlePlayerDie;
    }

    private void Update() {
        if (targetTransforms.Count == 0) {
            if (enemy != null && enemy.attackDetector01 != null)
            {
                enemy.attackDetector01.SetActive(false);
            }
            return;
        }


        // 簡單示範：選擇 HashSet 中的第一個目標
        foreach (var target in targetTransforms)
        {
            if (target == null) continue;

            // 發送移動請求給 Enemy
            enemyAI.SetMoveTarget(target);

            break; // 一次只處理一個
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (((1 << col.gameObject.layer) & targetLayers.value) == 0) return;

        var playerClass = col.GetComponent<IDamageable>();
        if (playerClass != null)
        {
            enemy.attackDetector01.SetActive(true);
            targetTransforms.Add(((MonoBehaviour)playerClass).transform);
        }

    }

    private void OnTriggerExit2D(Collider2D col) {
        if (((1 << col.gameObject.layer) & targetLayers.value) == 0) return;

        var playerClass = col.GetComponent<IDamageable>();
        if (playerClass != null)
            targetTransforms.Remove(((MonoBehaviour)playerClass).transform);
    }

    private void HandlePlayerDie(IDamageable player) {
        if (player != null)
            targetTransforms.Remove(((MonoBehaviour)player).transform);
    }
}
