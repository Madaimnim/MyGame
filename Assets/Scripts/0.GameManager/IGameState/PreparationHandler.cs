using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparationHandler : IGameStateHandler
{

    private readonly GameSceneSystem _sceneSystem;
    private  UIController_Input _uiController => UIController_Input.Instance;

    public PreparationHandler(
        GameSceneSystem sceneSystem
        ) {
        _sceneSystem = sceneSystem;
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
