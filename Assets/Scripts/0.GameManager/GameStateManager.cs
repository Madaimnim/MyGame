using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;


public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;
    public GameState currentState = GameState.GameStart;
    #region Enum 定義
    public enum GameState
    {
        GameStart,       // 禁用所有控制（遊戲開始動畫或過場）
        Battle,
        BattlePause,       // 暫停階段
        BattleResult,    // 戰鬥結束階段
        Preparation,     // 準備階段
        EndGame          // 遊戲結束階段
    }
    #endregion
    #region 單例模式
    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 確保跨場景存活
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region 設定狀態
    public void SetState(GameState newState, string sceneName = null) {
        if (currentState == newState) return; // 避免重複設置相同狀態
        //Debug.Log($"[GameStateManager] 狀態變更: {currentState} -> {newState}");

        //先離開當前狀態，再進入下個狀態
        ExitState(currentState);
        EnterState(newState,sceneName);
        currentState = newState;
    }
    #endregion

    #region 各狀態對應的方法
    //進出遊戲開始
    private void HandleGameStart() {
        GameSceneManager.Instance.LoadSceneGameStart();
        GameSceneManager.Instance.GameStartButton.gameObject.SetActive(true);   
        UIInputController.Instance.isUIInputEnabled = false;
    }
    private void ExitGameStart() {
        GameSceneManager.Instance.GameStartButton.interactable=false;
        PlayerStateManager.Instance.UnlockAndSpawnPlayer(1);
        PlayerStateManager.Instance.playerStatesDtny[1].UnlockSkill(2);
        PlayerStateManager.Instance.playerStatesDtny[1].UnlockSkill(3);
        PlayerStateManager.Instance.SetupDefaultSkills(1);

        //PlayerStateManager.Instance.UnlockAndSpawnPlayer(2);
        //PlayerStateManager.Instance.SetupDefaultSkills(2);
    }

    //進出準備場景
    #region Handle、ExitPreparation()
    private void HandlePreparation() {
        PlayerStateManager.Instance.DeactivateAllPlayer();
        GameSceneManager.Instance.LoadScenePreparation();
        GameSceneManager.Instance.GameStartButton.gameObject.SetActive(false);
        UIInputController.Instance.isUIInputEnabled = true;
        TextPopupManager.Instance.TextPrefab_StageClear.transform.localPosition = Vector3.zero;
        TextPopupManager.Instance.TextPrefab_StageDefeat.transform.localPosition = Vector3.zero;
    }
    private void ExitPreparation() {
        UIInputController.Instance.isUIInputEnabled = false;
        UIManager.Instance.CloseAllUIPanels();
    }
    #endregion

    //進出戰鬥場景
    #region Handle、ExitBattle(string sceneName)
    private void HandleBattle(string sceneName) {
        StartCoroutine(HandleBattle_Co(sceneName));
    }
    
    private IEnumerator HandleBattle_Co(string sceneName) {
        // 等待場景載入完成（含淡出/加載/淡入流程）
        yield return GameSceneManager.Instance.LoadSceneForSceneName_Co(sceneName);
        //再等一幀
        yield return null;

        //新場景就緒後再啟用玩家
        PlayerStateManager.Instance.ActivateAllPlayer();
        // 再初始化輸入與清單
        PlayerInputController.Instance.InitailPlayerList();
        
        yield return new WaitForSeconds(0.5f);

        DialogueManager.Instance.StartDialogue();

        yield return new WaitUntil(() => DialogueManager.Instance.isDialogueRunning == false);

        PlayerInputController.Instance.isBattleInputEnabled = true;
    }


    private void ExitBattle() {
        PlayerInputController.Instance.isBattleInputEnabled = false;
        TextPopupManager.Instance.TextPrefab_StageClear.SetActive(false);
        TextPopupManager.Instance.TextPrefab_StageDefeat.SetActive(false);
    }
    #endregion


    //進出戰鬥暫停

    private void HandleBattlePause() {
        Time.timeScale = 0f;
    }
    private void ExitBattlePause() {
        Time.timeScale = 1f;
    }

    //進出戰鬥結果
    private void HandleBattleResult() {
    }
    private void ExitBattleResult() {
    }

    //進出遊戲結束
    private void HandleEndGame() {
        GameSceneManager.Instance.LoadSceneGameStart();
    }
    private void ExitEndGame() {
    }
    #endregion

    //狀態進出Case選項
    #region EnterState(GameState state,string sceneName)
    private void EnterState(GameState state,string sceneName) {
        switch (state)
        {
            case GameState.GameStart:
                HandleGameStart();
                break;

            case GameState.Preparation:
                HandlePreparation();
                break;

            case GameState.Battle:
                HandleBattle(sceneName);
                break;

            case GameState.BattlePause:
                HandleBattlePause();
                break;

            case GameState.BattleResult:
                HandleBattleResult();
                break;

            case GameState.EndGame:
                HandleEndGame();
                break;
        }
    }
    private void ExitState(GameState state) {
        switch (state)
        {
            case GameState.GameStart:
                ExitGameStart();
                break;

            case GameState.Preparation:
                ExitPreparation();
                break;

            case GameState.Battle:
                ExitBattle();
                break;

            case GameState.BattlePause:
                ExitBattlePause();
                break;

            case GameState.BattleResult:
                ExitBattleResult();
                break;

            case GameState.EndGame:
                ExitEndGame();
                break;
        }
    }
    #endregion

    //進入準備階段方法(外部呼叫用)

    #region
    public void EnterPrepareState() {
        SetState(GameState.Preparation,"Preparation");
    }
    #endregion
}
