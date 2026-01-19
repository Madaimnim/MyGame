using UnityEditor.Build.Pipeline;
using UnityEngine;

public abstract class MoveStrategyBase
{
    public virtual Vector2? GetMovePosition(AIComponent ai) => null;
    public virtual Transform GetMoveTransform(AIComponent ai) => null;


    public virtual void Reset() { }
}
