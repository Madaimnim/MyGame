using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameStateRouter:GameSubSystem
{
    private GameStateSystem _gameStateSystem;
    private Dictionary<GameState, IGameStateHandler> _gameStateHandlers;

    public GameStateRouter(GameManager gm) : base(gm) {
    } 

    public override void Initialize( ) {
        _gameStateSystem = GameManager.GameStateSystem;
        _gameStateHandlers = GameManager.GameStateHandlers;
        _gameStateSystem.Event_OnStateEntered += OnStateEnter;
        _gameStateSystem.Event_OnStateExited += OnStateExit;
    }

    private void OnStateEnter(GameState gameState) {
        if (_gameStateHandlers.TryGetValue(gameState, out var stateHandler)) stateHandler.Enter();
        else Debug.LogWarning($"No handler for{gameState}");
    }
    private void OnStateExit(GameState gameState) {
        if (_gameStateHandlers.TryGetValue(gameState, out var stateHandler)) stateHandler.Exit();
        else Debug.LogWarning($"No handler for {gameState}");
    }
}