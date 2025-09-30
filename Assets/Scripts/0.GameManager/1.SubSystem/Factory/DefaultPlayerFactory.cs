using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultPlayerFactory : IPlayerFactory
{
    public GameObject CreatPlayer(PlayerStatsRuntime rt, Transform parent) {
        if (rt.VisualData.Prefab == null) {Debug.LogError($"ID:{rt.StatsData.Id}�LCharPrefab����"); return null; }
        var ob = Object.Instantiate(rt.VisualData.Prefab, parent);
        ob.SetActive(false);
        ob.transform.localPosition = Vector3.zero;

        var player = ob.GetComponent<Player>();
        if (player != null) player.Initialize(rt);
        else { Debug.LogWarning($"����SPlayer"); return null; }

        return ob;
    }
}
