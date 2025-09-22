
using UnityEngine;

public class UICurrentPlayerChangEvent
{
    public PlayerStatsRuntime currentPlayer  { get; private set; }

    public UICurrentPlayerChangEvent(PlayerStatsRuntime currentPlayer) {
        this.currentPlayer = currentPlayer;
    }
}
