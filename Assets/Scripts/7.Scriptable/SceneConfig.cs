using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SceneConfig", menuName = "GameData/SceneConfig")]
public class SceneConfig : ScriptableObject
{
    [Tooltip("場景清單（Key -> Address）")]
    [FormerlySerializedAs("SceneKeyAddressis")]
    public NamedAddressEntry[] SceneKeyAddressis;              //可序列化、可在 Inspector 編輯的字典項目，單筆資料[]
    private Dictionary<string, string> _sceneKeyAddressDtny;      //將Inspector裡輸入的NamedAddressEntry[]，存入字典

    private void OnEnable() {
        //轉成字典
        _sceneKeyAddressDtny = new Dictionary<string, string>();
        foreach (var entry in SceneKeyAddressis)
        {
            if (!string.IsNullOrEmpty(entry.key) && !_sceneKeyAddressDtny.ContainsKey(entry.key)) _sceneKeyAddressDtny.Add(entry.key, entry.address);
            else Debug.LogWarning($"SceneConfig: 發現重複或空白的 Key = {entry.key}");
        }
    }

    public string GetAddress(string key) {
        //用key查字典裡的Address
        if (_sceneKeyAddressDtny.TryGetValue(key, out string address))return address;
        Debug.LogError($"SceneConfig: 找不到對應場景 Key = {key}");
        return null;
    }
}
