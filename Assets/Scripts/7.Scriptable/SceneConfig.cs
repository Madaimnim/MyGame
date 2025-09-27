using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneConfig", menuName = "GameData/SceneConfig")]
public class SceneConfig : ScriptableObject
{
    [Tooltip("場景清單（Key -> Address）")]
    public NamedAddressEntry[] scenes ;
    private Dictionary<string, string> _cache;

    private void OnEnable() {
        // 初始化快取
        _cache = new Dictionary<string, string>();
        foreach (var entry in scenes)
        {
            if (!string.IsNullOrEmpty(entry.key) && !_cache.ContainsKey(entry.key))
                _cache.Add(entry.key, entry.address);
            else Debug.LogWarning($"SceneConfig: 發現重複或空白的 Key = {entry.key}");
        }
    }

    public string GetAddress(string key) {
        if (_cache.TryGetValue(key, out string address))return address;
        
        Debug.LogError($"SceneConfig: 找不到對應場景 Key = {key}");
        return null;
    }
}
