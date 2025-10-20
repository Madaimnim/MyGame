using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class EnemyMoveDetector : MonoBehaviour
{
    [Header("敵人本體")]
    public Enemy enemy;


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
        foreach (var target in targetTransforms)
        {
            if (target == null) continue;
            break; // 一次只處理一個
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (((1 << col.gameObject.layer) & targetLayers.value) == 0) return;
        var playerClass = col.GetComponent<IInteractable>();
    }

    private void OnTriggerExit2D(Collider2D col) {
        if (((1 << col.gameObject.layer) & targetLayers.value) == 0) return;

        var playerClass = col.GetComponent<IInteractable>();
        if (playerClass != null)
            targetTransforms.Remove(((MonoBehaviour)playerClass).transform);
    }

    private void HandlePlayerDie(IInteractable player) {
        if (player != null)
            targetTransforms.Remove(((MonoBehaviour)player).transform);
    }
}
