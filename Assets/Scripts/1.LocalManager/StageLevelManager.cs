using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLevelManager : MonoBehaviour
{
    public static StageLevelManager Instance { get; private set; }
    private StageEnemySpawner _stageEnemySpawner;
    private int _remainenemyCount=0;    //剩餘敵人的數量
    private int _totalEnemyCount=0;     //當前關卡敵人的數量

    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _stageEnemySpawner=GetComponent<StageEnemySpawner>();
    }
    private void OnEnable() {
        GameEventSystem.Instance.Event_OnPlayerDie += RespawnPlayer;
        GameEventSystem.Instance.Event_OnWallBroken += WallBroken;
    }
    private void Start() {
        _totalEnemyCount = CalculateTotalEnemyCount();
        _remainenemyCount = _totalEnemyCount;

        // 只有生成過敵人，旗幟才true
        Debug.Log($"當前關卡敵人數量:{_totalEnemyCount}");
    }

    private void Update() {
        
    }

    private void OnDisable() {
        GameEventSystem.Instance.Event_OnPlayerDie -= RespawnPlayer;
        GameEventSystem.Instance.Event_OnWallBroken -= WallBroken;
    }

    private int CalculateTotalEnemyCount() {
        int total = 0;
        foreach (var wave in _stageEnemySpawner.SpawnConfig.Waves) {
            total += wave.SpawnCount;
        }
        return total;
    }

    private void RespawnPlayer(IInteractable player) {
        //LevelDefeat();
        StartCoroutine(RespawnPlayerCoroutine(player as Player,3));
    }
    private IEnumerator RespawnPlayerCoroutine(Player player,float respawnDelay) {
        if (player == null) yield break;
        Vector3 deathPos = player.transform.position;

        // 顯示倒數 UI
        TextPopupManager.Instance.ShowRespawnTimerPopup(player.transform, respawnDelay);

        yield return new WaitForSeconds(respawnDelay);

        player.RespawnComponent.Respawn();
    }

    private void WallBroken() {
        LevelDefeat();
    }
    public void EnemyDefeated() {
        _remainenemyCount--;
        Debug.Log($"敵人死亡，目前數量{_remainenemyCount}");
        if (_remainenemyCount <= 0)
        {
            LevelClear();
        }
    }

    private void LevelClear() {
        TextPopupManager.Instance.ShowStageClearPopup();
        FadeManager.Instance.FadeSetAlpha(0.3f);

        StartCoroutine(WaitAndGoPrepareState());
    }
    private void LevelDefeat() {
        Debug.Log("觸發Defeat跳躍字體");
        TextPopupManager.Instance.ShowStageDefeatPopup();
        FadeManager.Instance.FadeSetAlpha(0.3f);

        StartCoroutine(WaitAndGoPrepareState());
    }

    //關卡結束等待或滑鼠點擊，回到準備畫面
    private IEnumerator WaitAndGoPrepareState() {
        float timer = 0f;

        while (timer < 5f)
        {
            timer += Time.deltaTime;
            if (Input.GetMouseButtonDown(0)) break;
            yield return null; 
        }

        // 切換狀態
        GameManager.Instance.GameStateSystem.SetState(GameStateSystem.GameState.Preparation);
        GameManager.Instance.PlayerStateSystem.AllPlayerClose();
    }
}
