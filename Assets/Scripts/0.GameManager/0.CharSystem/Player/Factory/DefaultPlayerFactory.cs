using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultPlayerFactory
{
    public Player CreatPlayer(int id) {
        var rt = PlayerUtility.Get(id);
        var ob = Object.Instantiate(rt.VisualData.Prefab, GameManager.Instance.PlayerBattleParent);
        ob.SetActive(false);
        ob.transform.localPosition = Vector3.zero;

        var player = ob.GetComponent<Player>();
        player.Initialize(rt);

        return player;
    }
}
