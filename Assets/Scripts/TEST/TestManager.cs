using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    [Header("測試腳色的ID")]
    public int playerID = 1;
    [Header("測試技能的ID")]
    public int testPlayerSkillID =1;

    [Header("玩家角色生成的位置")]
    public Transform playerSpawnTransform;



    public IEnumerator Start(){
        yield return StartCoroutine(GameSystem.Instance.WaitForDataReady());
        //StartCoroutine(CreatPlayer());

        yield return null;

    }


    //生成測試腳色在測試場景
    #region
    public IEnumerator CreatPlayer() {
        //Todo
        //PlayerStateManager.Instance.UnlockAndSpawnPlayer(playerID);
        //
        //PlayerStateManager.Instance.playerStatesDtny[1].UnlockSkill(testPlayerSkillID);
        //PlayerStateManager.Instance.playerStatesDtny[playerID].SetSkillAtSlot(0, testPlayerSkillID);
        //
        //PlayerStateManager.Instance.deployedPlayersGameObjectDtny[playerID].SetActive(true);
        //PlayerStateManager.Instance.deployedPlayersGameObjectDtny[playerID].transform.position = playerSpawnTransform.position;
        //
        //
        //PlayerInputController.Instance.InitailPlayerList();
        //
        //PlayerInputController.Instance.isBattleInputEnabled = true;

        yield return null;
    }
    #endregion
}
