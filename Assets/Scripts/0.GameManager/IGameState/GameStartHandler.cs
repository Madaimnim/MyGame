using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameStartHandler : IGameStateHandler
{   private readonly GameSceneManager _sceneManager;
    private readonly UIController_Input _uiController_Input;
    private readonly PlayerStateManager _playerStateManager;

    public GameStartHandler(
        GameSceneManager sceneManager,
        UIController_Input uiController_Input,
        PlayerStateManager playerStateManager
        ) {
        _sceneManager = sceneManager;
        _uiController_Input = uiController_Input;
        _playerStateManager = playerStateManager;
    }

    public void Enter(string sceneName) {
        _sceneManager.LoadSceneGameStart();
        _sceneManager.GameStartButton.gameObject.SetActive(true);
        _uiController_Input.isUIInputEnabled = false;
    }

    public void Exit() {
        _sceneManager.GameStartButton.interactable = false;
        _playerStateManager.UnlockPlayer(1001);
        _playerStateManager.SpawnBothPlayers(1001);
        _playerStateManager.GetState(1001).UnlockSkill(1);
        _playerStateManager.GetState(1001).UnlockSkill(2);
        _playerStateManager.SetupPlayerSkillSlot(1001, 0, 1);


        _playerStateManager.UnlockPlayer(1002);
        _playerStateManager.SpawnBothPlayers(1002);
        _playerStateManager.GetState(1002).UnlockSkill(1);
        _playerStateManager.SetupPlayerSkillSlot(1002, 0, 1);
    }
}
