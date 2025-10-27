using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class EnemyMoveDetector : MonoBehaviour
{
    [Header("�ĤH����")]
    public Enemy enemy;


    [Header("�����ؼ�")]
    public LayerMask targetLayers;

    // �x�s�����쪺�ؼ�
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
            break; // �@���u�B�z�@��
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
