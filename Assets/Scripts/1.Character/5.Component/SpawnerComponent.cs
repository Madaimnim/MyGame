using System.Collections;
using UnityEngine;

public class SpawnerComponent {

    public SpawnerComponent() {}

    // 單純生成，不計算數值、不判斷冷卻
    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation) {
        if (prefab == null)
        {
            Debug.LogError("Spawner: Prefab 為空");
            return null;
        }

        GameObject obj = Object.Instantiate(prefab, position, rotation);
        return obj;
    }
}
