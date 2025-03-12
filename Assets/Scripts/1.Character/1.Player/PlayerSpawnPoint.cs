using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private void OnEnable() {
        if (PlayerStateManager.Instance != null)
        {
            PlayerStateManager.Instance.stageSpawnPosition = gameObject.transform.position;
            Debug.Log($"生成點座標為{ gameObject.transform.position}");
            PlayerStateManager.Instance.ActivateAllPlayer();
        }
        else
            Debug.LogWarning("PlayerStateManager不存在目前場景");
    }
}
