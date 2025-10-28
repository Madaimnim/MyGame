using UnityEditor.Build.Pipeline;
using UnityEngine;

public abstract class MoveStrategyBase
{

    public abstract Vector2? GetMoveDirection(AIComponent ai);
    public virtual Vector2? GetTargetPosition(AIComponent ai) => null;
    public virtual Transform GetTargetTransform(AIComponent ai) => null;


    public virtual void Reset() { }
}
