using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameStateRouter:SubSystemBase
{
    private GameStateSystem _gameStateSystem;
    private Dictionary<GameStateSystem.GameState, IGameStateHandler> _gameStateHandlers;

    public GameStateRouter(GameManager gm) : base(gm) { } 

    public override void Initialize( ) {
        _gameStateSystem = GameManager.GameStateSystem;
        _gameStateHandlers = GameManager.GameStateHandlers;
        _gameStateSystem.Event_OnStateEntered += OnStateEnter;
        _gameStateSystem.Event_OnStateExited += OnStateExit;
    }

    private void OnStateEnter(GameStateSystem.GameState gameState, string sceneName) {
        if (_gameStateHandlers.TryGetValue(gameState, out var h)) h.Enter(sceneName);
        else Debug.LogWarning($"[StateRouter] No handler for {gameState}");
    }
    private void OnStateExit(GameStateSystem.GameState gameState) {
        if (_gameStateHandlers.TryGetValue(gameState, out var h)) h.Exit();
        else Debug.LogWarning($"[StateRouter] No handler for {gameState}");
    }
}