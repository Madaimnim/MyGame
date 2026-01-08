using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparationHandler : IGameStateHandler
{

    private readonly GameSceneSystem _sceneSystem;

    public PreparationHandler(
        GameSceneSystem sceneSystem
        ) {
        _sceneSystem = sceneSystem;
    }
    public void Enter() {
        _sceneSystem.LoadSceneByKey( "Preparation");
        UIManager.Instance.UIInputController.EnableUIInput(true);
    }

    public void Exit() {
        UIManager.Instance.UIInputController.EnableUIInput(false);
        UIManager.Instance.HideAllUIPanels();
    }
}
