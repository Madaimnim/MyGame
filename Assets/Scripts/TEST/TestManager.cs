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
        yield return StartCoroutine(GameManager.Instance.WaitForDataReady());
        //StartCoroutine(CreatPlayer());

        yield return null;

    }


    //生成測試腳色在測試場景
    #region
    public IEnumerator CreatPlayer() {
        //Todo
        //GameManager.Instance.PlayerSystem.UnlockAndSpawnPlayer(playerID);
        //
        //GameManager.Instance.PlayerSystem.playerStatesDtny[1].AddUnlockSkill(testPlayerSkillID);
        //GameManager.Instance.PlayerSystem.playerStatesDtny[playerID].SetSkillAtSlot(0, testPlayerSkillID);
        //
        //GameManager.Instance.PlayerSystem.deployedPlayersGameObjectDtny[playerID].SetActive(true);
        //GameManager.Instance.PlayerSystem.deployedPlayersGameObjectDtny[playerID].transform.position = playerSpawnTransform.position;
        //
        //
        //PlayerInputController.Instance.InitailPlayerList();
        //
        //PlayerInputController.Instance.isBattleInputEnabled = true;

        yield return null;
    }
    #endregion
}
