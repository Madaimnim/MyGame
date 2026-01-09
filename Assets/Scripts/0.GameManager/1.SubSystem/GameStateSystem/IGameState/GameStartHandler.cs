using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameStartHandler : IGameStateHandler
{
    private readonly GameSceneSystem _gameSceneSystem;

    public GameStartHandler(GameSceneSystem gameSceneSystem) {
        _gameSceneSystem = gameSceneSystem;
    }

    public void Enter() {
        UIManager.Instance.SetLoadingUI(true);
        _gameSceneSystem.LoadSceneByKey("Start");
        UIManager.Instance.HideAllUIPanels();
    }
    public void Exit() {
        GameManager.Instance.PlayerStateSystem.PrepareInitialPlayers();
    }
}
