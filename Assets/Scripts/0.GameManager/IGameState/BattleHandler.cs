using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class BattleHandler : IGameStateHandler
{
    private readonly GameSceneSystem _sceneSystem;
    private readonly PlayerSystem _playerSystem;
    private readonly PlayerInputController _inputController;

    public BattleHandler(
            GameSceneSystem sceneSystem,
            PlayerSystem playerSystem,
            PlayerInputController inputController
        ) {
        _sceneSystem = sceneSystem;
        _playerSystem = playerSystem;
        _inputController = inputController;

        // �q�\�ƥ�
        _sceneSystem.OnSceneLoaded += OnSceneLoaded;
    }
    public void Enter(string sceneKey = null) {
        // �uĲ�o�������J�A���b�o�̪�����l��
        _sceneSystem.LoadSceneByKey(sceneKey??"Battle_Default");
    }

    public void Exit() {
        _sceneSystem.OnSceneLoaded -= OnSceneLoaded;

        TextPopupManager.Instance.TextPrefab_StageClear.SetActive(false);
        TextPopupManager.Instance.TextPrefab_StageDefeat.SetActive(false);
    }

    private void OnSceneLoaded(string sceneKey) {
        if (SceneKeyUtility.IsBattle(sceneKey)) // �u�n�O�԰������N�|�i��
        {
            _inputController.SelectDefaultPlayer();
            //�o�ƥ�
            GameEventSystem.Instance.Event_BattleStart?.Invoke();
        }
    }

    public static class SceneKeyUtility
    {
        public static bool IsBattle(string key) => key.StartsWith("Battle");
    }

}
