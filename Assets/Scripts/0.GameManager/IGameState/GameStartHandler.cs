using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameStartHandler : IGameStateHandler
{
    private readonly ICoroutineRunner _runner;  // 交給外部注入，負責啟動協程
    private readonly GameSceneSystem _sceneSystem;
    private readonly UIController_Input _uiController_Input;
    private readonly PlayerSystem _playerSystem;

    public GameStartHandler(
        ICoroutineRunner runner,
        GameSceneSystem sceneSystem,
        UIController_Input uiController_Input,
        PlayerSystem playerSystem
        ) {
        _sceneSystem = sceneSystem;
        _uiController_Input = uiController_Input;
        _playerSystem = playerSystem;
    }

    public void Enter(string sceneKey) {
        UIManager.Instance.SetLoadingUI(true);
        _runner.StartCoroutine(WaitForSceconds_LoadScene(1f, sceneKey));
    }

    public void Exit() {
        _playerSystem.UnlockPlayer(1001);
        _playerSystem.SpawnBothPlayers(1001);
        _playerSystem.GetStatsRuntime(1001).UnlockSkill(1);
        _playerSystem.GetStatsRuntime(1001).UnlockSkill(2);
        _playerSystem.SetupPlayerSkillSlot(1001, 0, 1);

        _playerSystem.UnlockPlayer(1002);
        _playerSystem.SpawnBothPlayers(1002);
        _playerSystem.GetStatsRuntime(1002).UnlockSkill(1);
        _playerSystem.SetupPlayerSkillSlot(1002, 0, 1);

        _playerSystem.DeactivateAllPlayer();
    }

    private IEnumerator WaitForSceconds_LoadScene(float s,string sceneKey) {
        yield return new WaitForSeconds(s);

        _sceneSystem.LoadSceneByKey(sceneKey ?? "Start");
        _uiController_Input.isUIInputEnabled = false;
    }
}
