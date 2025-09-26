using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHandler : IGameStateHandler
{
    private readonly ICoroutineRunner _runner;
    private readonly GameSceneManager _sceneManager;
    private readonly PlayerStateManager _playerStateManager;
    private readonly PlayerInputController _inputController;

    public BattleHandler(
        ICoroutineRunner runner, 
        GameSceneManager sceneManager, 
        PlayerStateManager playerStateManager, 
        PlayerInputController inputController
        ) {
        _runner = runner;
        _sceneManager = sceneManager;
        _playerStateManager = playerStateManager;
        _inputController = inputController;
        ;
    }
    public void Enter(string sceneName = null) {
        _runner.StartCoroutine(EnterBattleCoroutine(sceneName));
    }

    public void Exit() {
        TextPopupManager.Instance.TextPrefab_StageClear.SetActive(false);
        TextPopupManager.Instance.TextPrefab_StageDefeat.SetActive(false);
    }

    private IEnumerator EnterBattleCoroutine(string sceneName) {
        yield return _sceneManager.LoadSceneForSceneName_Co(sceneName);
        yield return null;

        _playerStateManager.ActivateAllPlayer();
        _inputController.InitailPlayerList();

        yield return null;

        GameEventSystem.Instance.Event_BattleStart.Invoke();
    }

}
