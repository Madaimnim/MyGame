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
        Debug.Log("StageLevelManager �q�\ Event_OnWallBroken");
        EventManager.Instance.Event_OnPlayerDie += RespawnPlayer;
        EventManager.Instance.Event_OnWallBroken += WallBroken;

    }
    private void OnDisable() {
        EventManager.Instance.Event_OnPlayerDie -= RespawnPlayer;
        EventManager.Instance.Event_OnWallBroken -= WallBroken;
    }
    #endregion

    //���a���`
    #region
    private void RespawnPlayer(IDamageable player) {
        //LevelDefeat();
        StartCoroutine(RespawnPlayerCoroutine(player as Player,3));
    }

    private IEnumerator RespawnPlayerCoroutine(Player player,float respawnDelay) {
        if (player == null) yield break;
        Vector3 deathPos = player.transform.position;

        // ��ܭ˼� UI
        TextPopupManager.Instance.ShowRespawnTimerPopup(player.transform, respawnDelay);

        yield return new WaitForSeconds(respawnDelay);

        player.Respawn();
    }

    #endregion


    //�����a
    #region
    private void WallBroken() {
        LevelDefeat();
    }
    #endregion

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
        Debug.Log("Ĳ�oDefeat���D�r��");
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
