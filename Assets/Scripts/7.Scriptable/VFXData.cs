using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VFXData", menuName = "GameData/VFXData")]
public class VFXData : ScriptableObject
{
    [System.Serializable]
    public class VFXPair
    {
        public string key;
        public GameObject prefab;
    }

    public List<VFXPair> EffectList = new();
}
