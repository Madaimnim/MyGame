using UnityEngine;
public enum GameState {
    GameStart,
    Preparation, Battle,
    BattleResult, EndGame
}

public class GameStateSystem :GameSubSystem
{
    public event System.Action<GameState> Event_OnStateEntered;
    public event System.Action<GameState> Event_OnStateExited;
    public event System.Action<bool> OnPlayerCanControlChanged;
    public GameState CurrentState { get; private set; } = GameState.GameStart;
    public bool IsControlEnabled => CurrentState == GameState.Battle;

    public GameStateSystem(GameManager gm) : base(gm) {}
    public void SetState(GameState newState) {
        if (CurrentState == newState) return; 
        var prev = CurrentState;

        //µo¨Æ¥ó
        Event_OnStateExited?.Invoke(prev);
        CurrentState = newState;
        Event_OnStateEntered?.Invoke(newState);

        OnPlayerCanControlChanged?.Invoke(IsControlEnabled);
    }

    public override void Initialize() {}
}
