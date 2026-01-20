using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Text;

public class StageFlowManager : MonoBehaviour {
    private EnemyCounter _enemyCounter;
    private EnemySpawner _enemySpawner;
    private PlayerEntrySystem _playerEntrySystem;
    private EnemyEntrySystem _enemyEntrySystem;
    private GameStageSystem _gameStageSystem;
    private StageData _stageData;

    [SerializeField] private TMP_Text _timerText;
    private float _battleTimer;
    private bool _isTiming;
    private Coroutine _stageFlowRoutine;

    private void Awake() {
        _enemyCounter = GetComponent<EnemyCounter>();
        _enemySpawner = GetComponent<EnemySpawner>();
        _playerEntrySystem= GetComponent<PlayerEntrySystem>();
        _enemyEntrySystem =GetComponent<EnemyEntrySystem>();
        _gameStageSystem = GameManager.Instance.GameStageSystem;

    }
    public void OnEnable() {
        _stageData = _gameStageSystem.CurrentStageData;

        _gameStageSystem.ResetBattleState();
        _enemyCounter.OnEnemyClear += OnEnemyClear;
        _timerText= UIManager.Instance.Text_TimeCounter;

        if (_stageFlowRoutine != null) { 
            StopCoroutine( _stageFlowRoutine );
        }
        _stageFlowRoutine=StartCoroutine(StageFlow());
    }
    public void OnDisable() {
        _enemyCounter.OnEnemyClear -= OnEnemyClear;
    }

    private void Start() {}
    private void Update() {
        TimingCounter();
    }

    private IEnumerator StageFlow() {
        //關卡開始（可加對話、演出）
        PrepareStage();

        yield return EnterBattle();
        yield return WaitForBattleEnd();

        bool isCleared = _enemyCounter.IsCleared;

        yield return ShowStageResult(isCleared);
        yield return WaitForContinueInput();
        yield return ExitStage(isCleared);
    }

    private void PrepareStage() {
        ExperienceManager.Instance.ResetExp();
    }
    private IEnumerator EnterBattle() {
        List<Coroutine> entryRoutines = new List<Coroutine>();
        // 敵人進場
        foreach (var wave in _stageData.Waves) {
            for (int i = 0; i < wave.SpawnCount; i++) {
                var enemy = _enemySpawner.Spawn(wave.EnemyId);
                entryRoutines.Add(StartCoroutine(_enemyEntrySystem.EntryEnemy(enemy)));
            }
        }
        // 玩家進場
        entryRoutines.Add(StartCoroutine(_playerEntrySystem.BeginEntryRoutine()));

        // 關鍵：等全部完成
        foreach (var routine in entryRoutines) yield return routine;

        yield return new WaitForSeconds(0.5f);
        // 所有人都到位，才開始戰鬥
        GameEventSystem.Instance.Event_BattleStart.Invoke();


        //計時開始
        _battleTimer = 0f;
        _isTiming = true;
    }
    private IEnumerator WaitForBattleEnd() {
        while (!_gameStageSystem.IsBattleEnded) yield return null;
        //停止計時
        _isTiming = false;
        RecordStageTime(_enemyCounter.IsCleared); //  關鍵
        PlayerInputManager.Instance.SetCanControl(false);
    }
    private IEnumerator ShowStageResult(bool isCleared) {
        FadeManager.Instance.FadeSetAlpha(0.3f);

       // yield return new WaitForSeconds(1f);

        var ui = UIManager.Instance.UI_StageClearController;

        if (isCleared) {
            ui.Text_StageClear.gameObject.SetActive(true);
            PlayerInputManager.Instance.CurrentControlPlayer.GrowthComponent.AddExp(ExperienceManager.Instance.GetCurrentExp());
            _gameStageSystem.MarkStageCleared(_gameStageSystem.CurrentStageData.StageId);
        }
        else {
            PlayerInputManager.Instance.CurrentControlPlayer.GrowthComponent.AddExp(ExperienceManager.Instance.GetCurrentExp());
            ui.Text_StageDefeat.gameObject.SetActive(true);
        }

        ui.UI_RewardSystem.gameObject.SetActive(true);
        ui.Text_Continue.gameObject.SetActive(true);

        yield break;
    }
    private IEnumerator WaitForContinueInput() {
        float countdown = 5f;

        var ui = UIManager.Instance.UI_StageClearController;
        while (countdown > 0f) {
            ui.Text_Continue.text =$"{Mathf.CeilToInt(countdown)} 秒後返回(點擊滑鼠)";
            countdown -= Time.deltaTime;

            if (Input.GetMouseButtonDown(0)) break;
            yield return null;
        }
        ui.Text_Continue.gameObject.SetActive(false);
    }
    private IEnumerator ExitStage(bool isCleared) {
        var ui = UIManager.Instance.UI_StageClearController;

        GameManager.Instance.GameStateSystem.SetState(GameState.Preparation);
        GameManager.Instance.PlayerStateSystem.HideAllPlayers();

        ui.UI_RewardSystem.gameObject.SetActive(false);
        ui.Text_StageClear.gameObject.SetActive(false);
        ui.Text_StageDefeat.gameObject.SetActive(false);

        yield break;
    }


    private void OnEnemyClear() {
        _gameStageSystem.SetIsBattleEneded(true);
    }

    private void TimingCounter() { 
        if (!_isTiming) return;
        _battleTimer += Time.deltaTime;

        TimeSpan timeSpan = TimeSpan.FromSeconds(_battleTimer);
        _timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
    }
    private void RecordStageTime(bool isCleared) {
        string folderPath = Path.Combine(Application.dataPath, "../Logs");
        Directory.CreateDirectory(folderPath);

        string fileName = isCleared
            ? "StageTime_Clear.csv"
            : "StageTime_Failed.csv";

        string path = Path.Combine(folderPath, fileName);

        if (!File.Exists(path)) {
            File.WriteAllText(path, "StageId,Time\n");
        }

        string line = $"{_stageData.StageId},{_battleTimer:F2}\n";
        File.AppendAllText(path, line);
    }


}
