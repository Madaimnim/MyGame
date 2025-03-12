using System.Collections.Generic;
using UnityEngine;


public class CameraInstance : MonoBehaviour
{
    private static Dictionary<string, CameraInstance> instances = new Dictionary<string, CameraInstance>();

    private void Awake() {
        string cameraName = gameObject.name;

        // 如果場景內已經有相同名稱的相機，刪除自己
        if (instances.ContainsKey(cameraName))
        {
            Destroy(gameObject);
            return;
        }

        // 記錄相機，確保唯一
        instances[cameraName] = this;
        DontDestroyOnLoad(gameObject); // 讓相機在場景切換時保持
    }
}
