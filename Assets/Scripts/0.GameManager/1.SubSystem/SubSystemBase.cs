using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SubSystemBase : ISubSystem
{
    protected readonly GameManager GameManager;
    public SubSystemBase(GameManager gm) {
        GameManager =gm;
        GameManager.RegisterSubsystem(this);
    }
    public abstract void Initialize();
    public virtual void Update(float deltaTime) {}
    public virtual void Shutdown() {}
}
