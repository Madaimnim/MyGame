using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;


public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance{get ; private set;}

    public GameState currentState = GameState.GameStart;
    #region Enum �w�q
    public enum GameState
    {
        GameStart,       // �T�ΩҦ�����]�C���}�l�ʵe�ιL���^
        Battle,
        BattlePause,       // �Ȱ����q
        BattleResult,    // �԰��������q
        Preparation,     // �ǳƶ��q
        EndGame          // �C���������q
    }
    #endregion
    #region ��ҼҦ�
    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region �]�w���A
    public void SetState(GameState newState, string sceneName = null) {
        if (currentState == newState) return; // �קK���Ƴ]�m�ۦP���A
        //Debug.Log($"[GameStateManager] ���A�ܧ�: {currentState} -> {newState}");

        //�����}��e���A�A�A�i�J�U�Ӫ��A
        ExitState(currentState);
        EnterState(newState,sceneName);
        currentState = newState;
    }
    #endregion

    #region �U���A��������k
    //�i�X�C���}�l
    private void HandleGameStart() {
        GameSceneManager.Instance.LoadSceneGameStart();
        GameSceneManager.Instance.GameStartButton.gameObject.SetActive(true);   
        UIController_Input.Instance.isUIInputEnabled = false;
    }
    private void ExitGameStart() {
        GameSceneManager.Instance.GameStartButton.interactable=false;
        PlayerStateManager.Instance.UnlockPlayer(1001);
        PlayerStateManager.Instance.SpawnBothPlayers(1001);
        PlayerStateManager.Instance.GetState(1001).UnlockSkill(1);
        PlayerStateManager.Instance.GetState(1001).UnlockSkill(2);
        PlayerStateManager.Instance.SetupPlayerSkillSlot(1001,0,1);

        PlayerStateManager.Instance.UnlockPlayer(1002);
        PlayerStateManager.Instance.SpawnBothPlayers(1002);
        PlayerStateManager.Instance.GetState(1002).UnlockSkill(1);
        PlayerStateManager.Instance.SetupPlayerSkillSlot(1002,0,1);
    }

    //�i�X�ǳƳ���
    #region Handle�BExitPreparation()
    private void HandlePreparation() {
        PlayerStateManager.Instance.DeactivateAllPlayer();
        GameSceneManager.Instance.LoadScenePreparation();
        GameSceneManager.Instance.GameStartButton.gameObject.SetActive(false);
        UIController_Input.Instance.isUIInputEnabled = true;
        TextPopupManager.Instance.TextPrefab_StageClear.transform.localPosition = Vector3.zero;
        TextPopupManager.Instance.TextPrefab_StageDefeat.transform.localPosition = Vector3.zero;
    }
    private void ExitPreparation() {
        UIController_Input.Instance.isUIInputEnabled = false;
        UIManager.Instance.CloseAllUIPanels();
    }
    #endregion

    //�i�X�԰�����
    #region Handle�BExitBattle(string sceneName)
    private void HandleBattle(string sceneName) {
        StartCoroutine(HandleBattle_Coroutine(sceneName));
    }
    
    private IEnumerator HandleBattle_Coroutine(string sceneName) {
        // ���ݳ������J�����]�t�H�X/�[��/�H�J�y�{�^
        yield return GameSceneManager.Instance.LoadSceneForSceneName_Co(sceneName);
        //�A���@�V
        yield return null;

        //�s�����N����A�ҥΪ��a
        PlayerStateManager.Instance.ActivateAllPlayer();
        //��l�ƿ�J�P�M��
        PlayerInputController.Instance.InitailPlayerList();

        //DialogueManager.Instance.StartDialogue();
        //yield return new WaitUntil(() => DialogueManager.Instance.isDialogueRunning == false);
        yield return null;

        PlayerInputController.Instance.isBattleInputEnabled = true;
        GameEventSystem.Instance.Event_BattleStart.Invoke();
    }


    private void ExitBattle() {
        PlayerInputController.Instance.isBattleInputEnabled = false;
        TextPopupManager.Instance.TextPrefab_StageClear.SetActive(false);
        TextPopupManager.Instance.TextPrefab_StageDefeat.SetActive(false);
    }
    #endregion


    //�i�X�԰��Ȱ�

    private void HandleBattlePause() {
        Time.timeScale = 0f;
    }
    private void ExitBattlePause() {
        Time.timeScale = 1f;
    }

    //�i�X�԰����G
    private void HandleBattleResult() {
    }
    private void ExitBattleResult() {
    }

    //�i�X�C������
    private void HandleEndGame() {
        GameSceneManager.Instance.LoadSceneGameStart();
    }
    private void ExitEndGame() {
    }
    #endregion

    //���A�i�XCase�ﶵ
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

    //�i�J�ǳƶ��q��k(�~���I�s��)

    #region
    public void EnterPrepareState() {
        SetState(GameState.Preparation,"Preparation");
    }
    #endregion
}
