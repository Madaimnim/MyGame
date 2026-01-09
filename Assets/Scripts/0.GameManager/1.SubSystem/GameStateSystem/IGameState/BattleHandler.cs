using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Linq;

public class BattleHandler : IGameStateHandler {
    private readonly GameSceneSystem _sceneSystem;

    public BattleHandler(GameSceneSystem sceneSystem) {
        _sceneSystem = sceneSystem;
    }
    public void Enter() {
        var stageData = GameManager.Instance.GameStageSystem.CurrentStageData;
        _sceneSystem.LoadSceneByKey(stageData.SceneKey);
    }

    public void Exit() {}

}
