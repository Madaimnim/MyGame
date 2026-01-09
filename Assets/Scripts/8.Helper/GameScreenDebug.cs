using UnityEngine;

public class GameScreenDebug : MonoBehaviour {
    [Header("總開關")]
    public bool EnableDebug = true;

    [Header("顯示設定")]
    public Vector2 StartPos = new Vector2(10, 10);
    public int FontSize = 24;
    public float LineSpacing = 1.2f;

    [Header("顯示項目")]
    public bool ShowGameState = true;
    public bool ShowCanControl = true;

    private float LineHeight => FontSize * LineSpacing;

    private void OnGUI() {
        if (!EnableDebug) return;
        if (GameManager.Instance == null) return;
        var gameStateSystem = GameManager.Instance.GameStateSystem;
        var playerInputSystem=PlayerInputManager.Instance;
        if (gameStateSystem == null) return;

        GUIStyle style = new GUIStyle(GUI.skin.label) {fontSize = FontSize,wordWrap = false,clipping = TextClipping.Overflow};

        int line = 0;

        if (ShowGameState) DrawLine($"GameState: {gameStateSystem.CurrentState}", ref line, style);

        if (ShowCanControl) DrawLine($"CanControl: {playerInputSystem.CanControl}", ref line, style);
    }

    private void DrawLine(string text, ref int line, GUIStyle style) {
        GUI.Label(new Rect(StartPos.x,StartPos.y + line * LineHeight,1000,LineHeight),text,style);

        line++;
    }
}
