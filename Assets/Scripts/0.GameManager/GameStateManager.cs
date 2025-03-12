using UnityEngine;
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
    public void SetState(GameState newState) {
        if (currentState == newState) return; // 避免重複設置相同狀態

        Debug.Log($"[GameStateManager] 狀態變更: {currentState} -> {newState}");

        //先離開當前狀態，再進入下個狀態
        ExitState(currentState);
        EnterState(newState);
        currentState = newState;
    }
    #endregion

    #region 各狀態對應的方法
    //進出遊戲開始
    private void HandleGameStart() {
        GameSceneManager.Instance.LoadSceneGameStart();
        GameSceneManager.Instance.GameStartButton.gameObject.SetActive(true);   
        UIInputController.Instance.isInputEnabled = false;
    }
    private void ExitGameStart() {
        GameSceneManager.Instance.GameStartButton.interactable=false;
    }

    //進出準備
    private void HandlePreparation() {
        PlayerStateManager.Instance.UnlockAndSpawnPlayer(1);
        PlayerStateManager.Instance.UnlockAndSpawnPlayer(2);

        GameSceneManager.Instance.LoadScenePreparation();
        GameSceneManager.Instance.GameStartButton.gameObject.SetActive(false);
        PlayerStateManager.Instance.DeactivateAllPlayer();

        UIInputController.Instance.isInputEnabled = true;
    }
    private void ExitPreparation() {
        UIInputController.Instance.isInputEnabled = false;
        UIManager.Instance.CloseAllUIPanels();
    }

    //進出戰鬥
    private void HandleBattle() {
        GameSceneManager.Instance.LoadSceneBattle();

    }
    private void ExitBattle() {

    }

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

    #region 狀態處理邏輯
    private void EnterState(GameState state) {
        switch (state)
        {
            case GameState.GameStart:
                HandleGameStart();
                break;

            case GameState.Preparation:
                HandlePreparation();
                break;

            case GameState.Battle:
                HandleBattle();
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
}
