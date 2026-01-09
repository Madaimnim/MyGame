using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntryController : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _playerPositionProvider;
    private IPositionProvider PositionProvider => _playerPositionProvider as IPositionProvider;

    public void PlayerEntryBattle() {
        var appearPos = PositionProvider.GetPosition();
        GameManager.Instance.PlayerStateSystem.AllPlayerAppear(appearPos, GameManager.Instance.GameStageSystem.CurrentStageData.PlayerAppearType);
        CameraManager.Instance.Follow(PlayerInputManager.Instance.CurrentControlPlayer.transform);

    }
}
