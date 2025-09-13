using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private CinemachineVirtualCamera virtualCam;

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 跨場景保留

        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 嘗試自動抓取 VirtualCamera
        virtualCam = GetComponentInChildren<CinemachineVirtualCamera>();
        if (virtualCam == null)
        {
            Debug.LogError("CameraManager 找不到 CinemachineVirtualCamera！");
        }
    }

    //CM相機的Follow跟隨方法
    #region Follow(Transform target)
    public void Follow(Transform target) {
        if (virtualCam != null)
        {
            virtualCam.Follow = target;
            //Debug.Log($"CameraManager: VirtualCam 正在跟隨 {target.name}");
        }
    }
    #endregion

    ////CM相機的移除跟隨腳色方法
    #region ClearFollow()
    public void ClearFollow() {
        if (virtualCam != null)
        {
            virtualCam.Follow = null;
            Debug.Log("CameraManager: VirtualCam 已清除跟隨目標");
        }
    }
    #endregion

    //只刪除「MainCamera」Tag 的相機
    #region CleanupExtraMainCameras()
    public void CleanupExtraMainCameras() {
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (var cam in cameras)
        {
            // 保留 CameraManager 自己的相機
            if (cam.transform.root == this.transform)
                continue;

            // 只刪掉 MainCamera 標籤的相機
            if (cam.CompareTag("MainCamera"))
            {
                Destroy(cam.gameObject);
                Debug.Log($" 移除多餘的 MainCamera: {cam.name}");
            }
        }
    }
    #endregion

    //設定相機邊界Collider2D方法
    #region SetConfiner(PolygonCollider2D collider)
    public void SetConfiner(Collider2D collider) {
        var confiner = virtualCam.GetComponent<CinemachineConfiner2D>();
        if (confiner != null)
        {
            confiner.m_BoundingShape2D = collider;
            confiner.InvalidateCache();
            Debug.Log($" CameraManager: Confiner 已設為 {collider.name}");
        }
    }
    #endregion
}
