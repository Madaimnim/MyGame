using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NoneHandler : IGameStateHandler {
    private readonly GameSceneSystem _gameSceneSystem;

    public NoneHandler(GameSceneSystem gameSceneSystem) {
        _gameSceneSystem = gameSceneSystem;
    }

    public void Enter() {
    }
    public void Exit() {
    }
}
