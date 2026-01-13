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
        //Debug.Log("EnterGameStartHandler");
        UIManager.Instance.HideAllUIPanels();
        UIManager.Instance.SetLoadingUI(true);
        _gameSceneSystem.LoadSceneByKey("Start");
    }
    public void Exit() {
        GameManager.Instance.PlayerStateSystem.PrepareInitialPlayers();
    }
}
