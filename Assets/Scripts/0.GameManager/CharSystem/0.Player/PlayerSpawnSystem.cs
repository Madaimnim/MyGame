using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PlayerSpawnSystem
{


    public PlayerSpawnSystem() {}

    public Player SpawnerPlayer(int id) {
        var rt = PlayerUtility.Get(id);
        var ob = Object.Instantiate(rt.VisualData.Prefab, GameManager.Instance.PlayerBattleParent);
        ob.SetActive(false);
        ob.transform.localPosition = Vector3.zero;

        var player = ob.GetComponent<Player>();
        player.Initialize(rt);

        player.Rt.BattleObject = player.gameObject;

        return player;
    }

}
