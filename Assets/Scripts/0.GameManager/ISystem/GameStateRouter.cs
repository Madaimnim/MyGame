using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameStateRouter
{
    private readonly GameStateSystem _gameStateManager;
    private readonly Dictionary<GameStateSystem.GameState, IGameStateHandler> _gameStateHandlers;

    public GameStateRouter(GameStateSystem gameStateManager,Dictionary<GameStateSystem.GameState, IGameStateHandler> gameStateHandler)  {
        _gameStateManager = gameStateManager; _gameStateHandlers = gameStateHandler;
        _gameStateManager.OnStateEntered += OnEnter;
        _gameStateManager.OnStateExited += OnExit;
    }

    private void OnEnter(GameStateSystem.GameState gameState, string sceneName) {
        if (_gameStateHandlers.TryGetValue(gameState, out var h)) h.Enter(sceneName);
        else Debug.LogWarning($"[StateRouter] No handler for {gameState}");
    }
    private void OnExit(GameStateSystem.GameState gameState) {
        if (_gameStateHandlers.TryGetValue(gameState, out var h)) h.Exit();
        else Debug.LogWarning($"[StateRouter] No handler for {gameState}");
    }
}