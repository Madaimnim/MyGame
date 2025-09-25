using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparationHandler : IGameStateHandler
{
    private readonly PlayerStateManager _playerStateManager;
    private readonly GameSceneManager _sceneManager;
    private readonly UIController_Input _uiController;

    public PreparationHandler(
        PlayerStateManager playerStateManager,
        GameSceneManager sceneManager,
        UIController_Input uiController
        ) {
        _playerStateManager = playerStateManager;
        _sceneManager = sceneManager;
        _uiController = uiController;
    }
    public void Enter(string sceneName = null) {
        _playerStateManager.DeactivateAllPlayer();
        _sceneManager.LoadScenePreparation();
        _sceneManager.GameStartButton.gameObject.SetActive(false);
        _uiController.isUIInputEnabled = true;
        TextPopupManager.Instance.TextPrefab_StageClear.transform.localPosition = Vector3.zero;
    }

    public void Exit() {
        _uiController.isUIInputEnabled = false;
        UIManager.Instance.CloseAllUIPanels();
    }
}
