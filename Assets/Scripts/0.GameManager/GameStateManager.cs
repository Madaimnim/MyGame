using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class GameStateManager
{
    public enum GameState
    {
        GameStart,       
        Preparation,     
        Battle,             
        BattleResult,   
        EndGame         
    }
    public event Action<GameState,string> OnStateEntered;
    public event Action<GameState> OnStateExited;
    public event Action<bool> OnControlEnabledChanged;
    public GameState CurrentState { get; private set; } = GameState.GameStart;
    public bool IsControlEnabled => CurrentState == GameState.Battle;

    public GameStateManager() {  }

    public void SetState(GameState newState, string sceneName = null) {
        if (CurrentState == newState) return; // 避免重複設置相同狀態

        var previousState = CurrentState;

        OnStateExited?.Invoke(previousState);
        CurrentState = newState;
        OnStateEntered?.Invoke(newState,sceneName);

        OnControlEnabledChanged?.Invoke(IsControlEnabled);
    }

}
