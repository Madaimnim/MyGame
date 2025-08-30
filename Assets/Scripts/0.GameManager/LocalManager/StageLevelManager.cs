using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLevelManager : MonoBehaviour
{
    public static StageLevelManager Instance;
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
        Player.Event_OnPlayerDie += HandlePlayerDie;
    }
    private void OnDisable() {
        Player.Event_OnPlayerDie -= HandlePlayerDie;
    }
    #endregion

    //當玩家死亡
    private void HandlePlayerDie(IDamageable player) {
        LevelDefeat();
    }

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
