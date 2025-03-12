
using UnityEngine;

public class UICurrentPlayerChangEvent
{
    public PlayerStateManager.PlayerStats currentPlayer  { get; private set; }

    public UICurrentPlayerChangEvent(PlayerStateManager.PlayerStats currentPlayer) {
        this.currentPlayer = currentPlayer;
    }
}
