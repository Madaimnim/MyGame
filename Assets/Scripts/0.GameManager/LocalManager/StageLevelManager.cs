using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLevelManager : MonoBehaviour
{
    public static StageLevelManager Instance { get; private set; }
    private int enemyCount; //當前關卡敵人的數量

    //生命週期
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
        GameEventSystem.Instance.Event_OnPlayerDie += RespawnPlayer;
        GameEventSystem.Instance.Event_OnWallBroken += WallBroken;

    }
    private void OnDisable() {
        GameEventSystem.Instance.Event_OnPlayerDie -= RespawnPlayer;
        GameEventSystem.Instance.Event_OnWallBroken -= WallBroken;
    }
    #endregion

    //當玩家死亡
    #region
    private void RespawnPlayer(IDamageable player) {
        //LevelDefeat();
        StartCoroutine(RespawnPlayerCoroutine(player as Player,3));
    }

    private IEnumerator RespawnPlayerCoroutine(Player player,float respawnDelay) {
        if (player == null) yield break;
        Vector3 deathPos = player.transform.position;

        // 顯示倒數 UI
        TextPopupManager.Instance.ShowRespawnTimerPopup(player.transform, respawnDelay);

        yield return new WaitForSeconds(respawnDelay);

        player.CharRespawnComponent.Respawn();
    }

    #endregion


    //當城牆毀壞
    #region
    private void WallBroken() {
        LevelDefeat();
    }
    #endregion

    #region 註冊敵人生成數量
    public void RegisterEnemy() {
        enemyCount++;
        Debug.Log($"目前敵人數量為{enemyCount}");
    }
    #endregion

    //註冊敵人死亡
    #region 註冊敵人死亡
    public void EnemyDefeated() {
        enemyCount--;
        Debug.Log($"敵人死亡，目前數量{enemyCount}");
        if (enemyCount<=0)
        {
            LevelClear();
        }
    }
    #endregion

    //通關成功
    #region LevelClear()
    private void LevelClear() {
        TextPopupManager.Instance.ShowStageClearPopup();
        FadeManager.Instance.FadeSetAlpha(0.3f);

        StartCoroutine(WaitAndGoPrepareState());
        //GameStateManager.Instance.SetState(GameStateManager.GameState.Preparation);
    }
    #endregion

    //通關失敗
    #region  LevelDefeat()
    private void LevelDefeat() {
        Debug.Log("觸發Defeat跳躍字體");
        TextPopupManager.Instance.ShowStageDefeatPopup();
        FadeManager.Instance.FadeSetAlpha(0.3f);

        StartCoroutine(WaitAndGoPrepareState());
    }
    #endregion

    //關卡結束等待或滑鼠點擊，回到準備畫面
    #region WaitAndGoPrepareState()
    private IEnumerator WaitAndGoPrepareState() {
        float timer = 0f;

        // 等待最多 5 秒，或者點擊滑鼠就提前跳出
        while (timer < 5f)
        {
            timer += Time.deltaTime;

            if (Input.GetMouseButtonDown(0))
            { // 左鍵點擊
                break;
            }

            yield return null; // 每幀檢查
        }

        // 切換狀態
        GameStateManager.Instance.SetState(GameStateManager.GameState.Preparation);
    }
    #endregion
}
