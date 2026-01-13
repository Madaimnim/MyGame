using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private CinemachineVirtualCamera virtualCam;

    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 嘗試自動抓取 VirtualCamera
        virtualCam = GetComponentInChildren<CinemachineVirtualCamera>();
        if (virtualCam == null) Debug.LogError("CameraManager 找不到 CinemachineVirtualCamera！");

   
    }



    private IEnumerator Start()
    {
        // 等待 PlayerInputManager 初始化完成
        yield return new WaitUntil(() => PlayerInputManager.Instance != null);
        PlayerInputManager.Instance.OnBattlePlayerSelected += Follow;
    }
    //CM相機的Follow跟隨方法
    public void Follow(Transform target) {
        if (virtualCam != null) virtualCam.Follow = target;
    }
    public void ClearFollow() {
        if (virtualCam != null) virtualCam.Follow = null;
    }

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
            //Debug.Log($" CameraManager: Confiner 已設為 {collider.name}");
        }
    }
    #endregion

}
