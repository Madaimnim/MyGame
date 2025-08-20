using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_PlayerSpawner : MonoBehaviour
{

    public List<int> playerIDList=new List<int>();

    private IEnumerator Start() {
        yield return StartCoroutine(GameManager.Instance.WaitForDataReady());
        foreach(var id in playerIDList)
        {
            PlayerStateManager.Instance.UnlockAndSpawnPlayer(id);
        }
        PlayerStateManager.Instance.DeactivateAllPlayer();
        PlayerStateManager.Instance.ActivateAllPlayer();

        PlayerInputController.Instance.InitailPlayerList();
        PlayerInputController.Instance.isBattleInputEnabled = true;
    }



}
