public class GameStateSystem:SubSystemBase
{
    public enum GameState{GameStart,Preparation,Battle,BattleResult,EndGame}
    public event System.Action<GameState,string> OnStateEntered;
    public event System.Action<GameState> OnStateExited;
    public event System.Action<bool> OnControlEnabledChanged;
    public GameState CurrentState { get; private set; } = GameState.GameStart;
    public bool IsControlEnabled => CurrentState == GameState.Battle;

    public GameStateSystem(GameManager gm):base(gm) {}

    public void SetState(GameState newState, string sceneName = null) {
        if (CurrentState == newState) return; 
        var prev = CurrentState;
        OnStateExited?.Invoke(prev);
        CurrentState = newState;
        OnStateEntered?.Invoke(newState,sceneName);
        OnControlEnabledChanged?.Invoke(IsControlEnabled);
    }


    public override void Initialize() { }
    public override void Update(float deltaTime) { }
    public override void Shutdown() { }
}
