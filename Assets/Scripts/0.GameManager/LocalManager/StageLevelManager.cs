using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLevelManager : MonoBehaviour
{
    public static StageLevelManager Instance;
    private int enemyCount; //��e���d�ĤH���ƶq

    //�ͩR�g��
    #region
    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        enemyCount = 0;
    }

    private void OnEnable() {
        Player.Event_OnPlayerDie += HandlePlayerDie;
    }
    private void OnDisable() {
        Player.Event_OnPlayerDie -= HandlePlayerDie;
    }
    #endregion

    //���a���`
    private void HandlePlayerDie(IDamageable player) {
        LevelDefeat();
    }

    #region ���U�ĤH�ͦ��ƶq
    public void RegisterEnemy() {
        enemyCount++;
        Debug.Log($"�ثe�ĤH�ƶq��{enemyCount}");
    }
    #endregion

    //���U�ĤH���`
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

    //�q�����\
    #region LevelClear()
    private void LevelClear() {
        TextPopupManager.Instance.ShowStageClearPopup();
        FadeManager.Instance.FadeSetAlpha(0.3f);

        StartCoroutine(WaitAndGoPrepareState());
        //GameStateManager.Instance.SetState(GameStateManager.GameState.Preparation);
    }
    #endregion

    //�q������
    #region  LevelDefeat()
    private void LevelDefeat() {
        TextPopupManager.Instance.ShowStageDefeatPopup();
        FadeManager.Instance.FadeSetAlpha(0.3f);

        StartCoroutine(WaitAndGoPrepareState());
    }
    #endregion

    //���d�������ݩηƹ��I���A�^��ǳƵe��
    #region WaitAndGoPrepareState()
    private IEnumerator WaitAndGoPrepareState() {
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
