using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameStartHandler : IGameStateHandler
{
    private readonly ICoroutineRunner _runner;  // 交給外部注入，負責啟動協程
    private readonly GameSceneSystem _sceneSystem;
    private readonly PlayerStateSystem _playerStateSystem;

    public GameStartHandler(
        ICoroutineRunner runner,
        GameSceneSystem sceneSystem,
        PlayerStateSystem playerSystem
        ) {
        _sceneSystem = sceneSystem;
        _playerStateSystem = playerSystem;
    }

    public void Enter(string sceneKey) {
        UIManager.Instance.SetLoadingUI(true);
        _runner.StartCoroutine(WaitForSceconds_LoadScene(1f, sceneKey));
    }
    public void Exit() {
        _playerStateSystem.InitialGameStartPlayerSpawn();
    }

    private IEnumerator WaitForSceconds_LoadScene(float s,string sceneKey) {
        yield return new WaitForSeconds(s);

        _sceneSystem.LoadSceneByKey(sceneKey ?? "Start");
    }
}
