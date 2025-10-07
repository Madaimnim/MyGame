using UnityEngine;

public class GameStateSystem :SubSystemBase
{

    public enum GameState{GameStart,Preparation,Battle,BattleResult,EndGame}
    public event System.Action<GameState,string> Event_OnStateEntered;
    public event System.Action<GameState> Event_OnStateExited;
    public event System.Action<bool> OnPlayerCanControlChanged;
    public GameState CurrentState { get; private set; } = GameState.GameStart;
    public bool IsControlEnabled => CurrentState == GameState.Battle;

    public GameStateSystem(GameManager gm) : base(gm) {}
    public void SetState(GameState newState, string sceneKey = null) {
        if (CurrentState == newState) return; 
        var prev = CurrentState;
        
        Debug.Log($"°h¥X{prev}");

        Event_OnStateExited?.Invoke(prev);

        CurrentState = newState;
        Debug.Log($"¶i¤J{CurrentState}");

        Event_OnStateEntered?.Invoke(newState,sceneKey);
        OnPlayerCanControlChanged?.Invoke(IsControlEnabled);
    }

    public override void Initialize() {}
}
