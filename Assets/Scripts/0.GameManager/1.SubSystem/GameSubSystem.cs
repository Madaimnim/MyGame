using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameSubSystem : IGameSubSystem
{
    protected readonly GameManager GameManager;
    public GameSubSystem(GameManager gm) {
        GameManager =gm;
        GameManager.RegisterSubsystem(this);
    }
    public abstract void Initialize();
    public virtual void Update(float deltaTime) {}
    public virtual void Shutdown() {}
}
