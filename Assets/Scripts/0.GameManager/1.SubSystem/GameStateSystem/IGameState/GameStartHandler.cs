using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameStartHandler : IGameStateHandler
{
    private readonly ICoroutineRunner _runner;  // 交給外部注入，負責啟動協程
    private readonly GameSceneSystem _gameSceneSystem;
    private readonly PlayerStateSystem _playerStateSystem;

    public GameStartHandler(
        ICoroutineRunner runner,
        GameSceneSystem gameSceneSystem,
        PlayerStateSystem playerSystem
        ) {
        _gameSceneSystem = gameSceneSystem;
        _playerStateSystem = playerSystem;
    }

    public void Enter() {
        UIManager.Instance.SetLoadingUI(true);
        _runner.StartCoroutine(WaitForSceconds_LoadSceneStart(0f));
    }
    public void Exit() {
        _playerStateSystem.InitialGameStartPlayerSpawn();
    }

    private IEnumerator WaitForSceconds_LoadSceneStart(float s) {
        yield return new WaitForSeconds(s);
        _gameSceneSystem.LoadSceneByKey("Start");
    }
}
