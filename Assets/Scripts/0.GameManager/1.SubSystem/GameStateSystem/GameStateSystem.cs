using UnityEngine;
public enum GameState {
    None,
    GameStart,
    Preparation, Battle,
    BattleResult, EndGame
}

public class GameStateSystem :GameSubSystem
{
    public event System.Action<GameState> Event_OnStateEntered;
    public event System.Action<GameState> Event_OnStateExited;
    public GameState CurrentState { get; private set; } = GameState.None;
    public GameStateSystem(GameManager gm) : base(gm) {}
    public void SetState(GameState newState) {
        if (CurrentState == newState) return; 
        var prev = CurrentState;

        //µo¨Æ¥ó
        Event_OnStateExited?.Invoke(prev);
        CurrentState = newState;
        Event_OnStateEntered?.Invoke(newState);
    }

    public override void Initialize() {}
}
