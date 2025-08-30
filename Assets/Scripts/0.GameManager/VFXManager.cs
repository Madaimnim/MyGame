using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [System.Serializable]
    public class VFXPair
    {
        public string key;       // ¨Ò¦p "Death"
        public GameObject prefab;
    }

    public List<VFXPair> effectsList;
    private Dictionary<string, GameObject> effectDict = new();

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (var vfxPair in effectsList)
            {
                effectDict[vfxPair.key] = vfxPair.prefab;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Play(string key, Vector3 position, float duration = 2f) {
        if (!effectDict.ContainsKey(key))
        {
            Debug.LogWarning($"VFX {key} not found!");
            return;
        }

        GameObject vfx = Instantiate(effectDict[key], position, Quaternion.identity);
    }
}
