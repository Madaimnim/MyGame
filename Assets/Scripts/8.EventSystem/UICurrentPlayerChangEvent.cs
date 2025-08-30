
using UnityEngine;

public class UICurrentPlayerChangEvent
{
    public PlayerStateManager.PlayerStatsRuntime currentPlayer  { get; private set; }

    public UICurrentPlayerChangEvent(PlayerStateManager.PlayerStatsRuntime currentPlayer) {
        this.currentPlayer = currentPlayer;
    }
}
