using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    [Header("���ո}�⪺ID")]
    public int playerID = 1;
    [Header("���էޯ઺ID")]
    public int testPlayerSkillID =1;

    [Header("���a����ͦ�����m")]
    public Transform playerSpawnTransform;



    public IEnumerator Start(){
        yield return StartCoroutine(GameSystem.Instance.WaitForDataReady());
        //StartCoroutine(CreatPlayer());

        yield return null;

    }


    //�ͦ����ո}��b���ճ���
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
