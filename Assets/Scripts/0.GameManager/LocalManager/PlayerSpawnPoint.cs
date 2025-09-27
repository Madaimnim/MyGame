using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private IEnumerator Start() {
        yield return StartCoroutine(GameManager.Instance.WaitForDataReady());
        if (GameManager.Instance.PlayerSystem != null)
        {
            GameManager.Instance.PlayerSpawnPosition = gameObject.transform.position;
            GameManager.Instance.PlayerSystem.ActivateAllPlayer();
        }
        else
            Debug.LogWarning("PlayerStateManager不存在目前場景");
    }
}
