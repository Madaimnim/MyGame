using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    private int enemyCount; //當前關卡敵人的數量

    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        enemyCount = 0;
    }


    #region 註冊敵人生成數量
    public void RegisterEnemy() {
        enemyCount++;
        Debug.Log($"目前敵人數量為{enemyCount}");
    }
    #endregion

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

    private void LevelClear() {
        PopupManager.Instance.ShowStageClearPopup();
        FadeManager.Instance.FadeSetAlpha(0.5f);

        StartCoroutine(WaitAndChangeState());
        //GameStateManager.Instance.SetState(GameStateManager.GameState.Preparation);
    }

    //關卡結束等待或滑鼠點擊，回到準備畫面
    #region 關卡結束切換場景狀態
    private IEnumerator WaitAndChangeState() {
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
