using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private IEnumerator Start() {
        yield return StartCoroutine(GameSystem.Instance.WaitForDataReady());
        if (PlayerStateManager.Instance != null)
        {
            PlayerStateManager.Instance.stageSpawnPosition = gameObject.transform.position;
            PlayerStateManager.Instance.ActivateAllPlayer();
        }
        else
            Debug.LogWarning("PlayerStateManager不存在目前場景");
    }
}
