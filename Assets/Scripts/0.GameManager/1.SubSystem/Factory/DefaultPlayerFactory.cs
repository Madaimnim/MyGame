using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultPlayerFactory : IPlayerFactory
{
    public GameObject CreatPlayer(PlayerStatsRuntime rt, Transform parent) {
        if (rt.VisualData.Prefab == null) {Debug.LogError($"ID:{rt.StatsData.Id}無CharPrefab物件"); return null; }
        var ob = Object.Instantiate(rt.VisualData.Prefab, parent);
        ob.SetActive(false);
        ob.transform.localPosition = Vector3.zero;

        var player = ob.GetComponent<Player>();
        if (player != null) player.Initialize(rt);
        else { Debug.LogWarning($"物件沒Player"); return null; }

        return ob;
    }
}
