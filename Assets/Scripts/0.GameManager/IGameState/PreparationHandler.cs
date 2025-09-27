using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparationHandler : IGameStateHandler
{
    private readonly PlayerSystem _playerSystem;
    private readonly GameSceneSystem _sceneSystem;
    private readonly UIController_Input _uiController;

    public PreparationHandler(
        PlayerSystem playerSystem,
        GameSceneSystem sceneSystem,
        UIController_Input uiController
        ) {
        _playerSystem = playerSystem;
        _sceneSystem = sceneSystem;
        _uiController = uiController;
    }
    public void Enter(string sceneKey = null) {
        _sceneSystem.LoadSceneByKey(sceneKey?? "Preparation");
        _uiController.isUIInputEnabled = true;
        TextPopupManager.Instance.TextPrefab_StageClear.transform.localPosition = Vector3.zero;
    }

    public void Exit() {
        _uiController.isUIInputEnabled = false;
        UIManager.Instance.CloseAllUIPanels();
    }
}
