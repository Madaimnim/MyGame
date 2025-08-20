using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    private int enemyCount; //��e���d�ĤH���ƶq

    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        enemyCount = 0;
    }


    #region ���U�ĤH�ͦ��ƶq
    public void RegisterEnemy() {
        enemyCount++;
        Debug.Log($"�ثe�ĤH�ƶq��{enemyCount}");
    }
    #endregion

    #region ���U�ĤH���`
    public void EnemyDefeated() {
        enemyCount--;
        Debug.Log($"�ĤH���`�A�ثe�ƶq{enemyCount}");
        if (enemyCount<=0)
        {
            LevelClear();
        }
    }
    #endregion

    private void LevelClear() {
        PopupManager.Instance.ShowStageClearPopup();
        FadeManager.Instance.FadeSetAlpha(0.5f);

        StartCoroutine(WaitAndChangeState());
        //GameStateManager.Instance.SetState(GameStateManager.GameState.Preparation);
    }

    //���d�������ݩηƹ��I���A�^��ǳƵe��
    #region ���d���������������A
    private IEnumerator WaitAndChangeState() {
        float timer = 0f;

        // ���ݳ̦h 5 ��A�Ϊ��I���ƹ��N���e���X
        while (timer < 5f)
        {
            timer += Time.deltaTime;

            if (Input.GetMouseButtonDown(0))
            { // �����I��
                break;
            }

            yield return null; // �C�V�ˬd
        }

        // �������A
        GameStateManager.Instance.SetState(GameStateManager.GameState.Preparation);
    }
    #endregion
}
