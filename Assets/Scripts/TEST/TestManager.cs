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
        yield return StartCoroutine(GameManager.Instance.WaitForDataReady());
        //StartCoroutine(CreatPlayer());

        yield return null;

    }


    //�ͦ����ո}��b���ճ���
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
