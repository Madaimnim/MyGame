using UnityEngine;
using System.Collections;
using System;

public class StageFlowManager : MonoBehaviour {
    private EnemyCounter _enemyCounter;
    private EnemySpawner _enemySpawner;
    private PlayerEntryController _playerEntryController;

    private bool _isClear;      // 關卡結果
    private bool _stageEnded;   // 流程控制

    private void Awake() {
        _enemyCounter = GetComponent<EnemyCounter>();
        _enemySpawner = GetComponent<EnemySpawner>();
        _playerEntryController= GetComponent<PlayerEntryController>();  
    }

    public IEnumerator Start() {
        yield return StartCoroutine(StageFlow());
    }

    private IEnumerator StageFlow() {
        //關卡開始（可加對話、演出）
        yield return null;

        _playerEntryController.PlayerEntryBattle();

        //開始生怪
        _enemySpawner.SpawnBegin();

        _stageEnded = false;
        _isClear = false;

        GameEventSystem.Instance.Event_BattleStart.Invoke();


        _enemyCounter.OnEnemyClear += OnEnemyClear;
        // Todo
        // wallHealth.OnWallDestroyed +=OnStageDefeat;

        //等待關卡結束
        while (!_stageEnded) yield return null;

        PlayerInputManager.Instance.SetCanControl(false);
        //結算 / UI / 切狀態（原本在 StageLevelManager 的東西）
        yield return ShowStageResultAndExit();
    }

    private IEnumerator ShowStageResultAndExit() {

        FadeManager.Instance.FadeSetAlpha(0.3f);

        float timer = 0f;
        while (timer < 1f) {
            timer += Time.deltaTime;
            yield return null;
        }

        if (_isClear) {
            UIManager.Instance.UI_StageClearController.Text_StageClear.gameObject.SetActive(true);
            PlayerInputManager.Instance.CurrentControlPlayer.GrowthComponent
                .AddExp(ExperienceManager.Instance.GetCurrentExp());
        }
        else {
            UIManager.Instance.UI_StageClearController.Text_StageDefeat.gameObject.SetActive(true);
        }

        UIManager.Instance.UI_StageClearController.UI_RewardSystem.gameObject.SetActive(true);

        timer = 0f;
        while (timer < 5f) {
            timer += Time.deltaTime;
            if (Input.GetMouseButtonDown(0)) break;
            yield return null;
        }

        GameManager.Instance.GameStateSystem.SetState(GameState.Preparation);
        GameManager.Instance.PlayerStateSystem.HideAllPlayers();

        UIManager.Instance.UI_StageClearController.UI_RewardSystem.gameObject.SetActive(false);
        UIManager.Instance.UI_StageClearController.Text_StageClear.gameObject.SetActive(false);
        UIManager.Instance.UI_StageClearController.Text_StageDefeat.gameObject.SetActive(false);
    }

    private void OnEnemyClear() {
        _isClear = true;
        _stageEnded = true;
    }
    private void OnStageDefeat() {
        _isClear = false;
        _stageEnded = true;
    }
}
