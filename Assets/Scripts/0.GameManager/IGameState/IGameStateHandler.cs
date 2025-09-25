using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameStateHandler
{
    void Enter(string sceneName = null);
    void Exit();
}
