using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Linq;

public class BattleHandler : IGameStateHandler
{
    private readonly GameSceneSystem _sceneSystem;
    private readonly PlayerStateSystem _playerStateSystem;

    public BattleHandler(GameSceneSystem sceneSystem,PlayerStateSystem playerSystem) {
        _sceneSystem = sceneSystem;
        _playerStateSystem = playerSystem;

        // 訂閱事件
        _sceneSystem.OnSceneLoaded += OnSceneLoaded;
    }
    public void Enter() {
        var stageData = GameManager.Instance.GameStageSystem.CurrentStageData;
        _sceneSystem.LoadSceneByKey(stageData.SceneKey);
    }

    public void Exit() {

    }

    private void OnSceneLoaded(string sceneKey) {
        if (SceneKeyUtility.IsBattle(sceneKey)) // 只要是戰鬥場景就會進來
        {
            _playerStateSystem.AllPlayerEnterBattle(PlayerSpawnPoint.Instance.transform.position);
            CameraManager.Instance.Follow(PlayerUtility.AllPlayers.Values.FirstOrDefault().transform);
            //發事件
            GameEventSystem.Instance.Event_BattleStart?.Invoke();
        }
    }

    public static class SceneKeyUtility
    {
        public static bool IsBattle(string key) => key.StartsWith("Battle");
    }

}
