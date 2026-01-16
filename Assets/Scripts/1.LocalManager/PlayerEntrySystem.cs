using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntrySystem : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _playerInitialPositionProvider;
    [SerializeField] private MonoBehaviour _playerTargetPositionProvider;
    private IPositionProvider InitialPositionProvider => _playerInitialPositionProvider as IPositionProvider;
    private IPositionProvider TargetPositionProvider => _playerTargetPositionProvider as IPositionProvider;

    public IEnumerator BeginEntryRoutine() {
        var initialPos = InitialPositionProvider.GetPosition();
        GameManager.Instance.PlayerStateSystem.AllPlayerAppear(initialPos, GameManager.Instance.GameStageSystem.CurrentStageData.PlayerAppearType);
        CameraManager.Instance.Follow(PlayerInputManager.Instance.CurrentControlPlayer.transform);

        var targetPos = TargetPositionProvider.GetPosition();
        var player = PlayerInputManager.Instance.CurrentControlPlayer;
        HideUI(player);

        var move = player.MoveComponent;
        player.EnergyComponent.Set(1);
        move.SetIgnoreMoveBounds(true);
        move.SetIntentMove(targetPosition: targetPos);

        // 等玩家真的走到
        while (Vector2.Distance(player.transform.position, targetPos) > 0.05f)
            yield return null;

        move.SetIgnoreMoveBounds(false);
        move.ClearAllMoveIntent();
        ShowUI(player);
    }

    private void ShowUI(Player player) {
        player.HpSlider.gameObject.SetActive(true);
        player.EnergyUIController.gameObject.SetActive(true);
    }

    private void HideUI(Player player) {
        player.HpSlider.gameObject.SetActive(false);
        player.EnergyUIController.gameObject.SetActive(false);
    }
}
