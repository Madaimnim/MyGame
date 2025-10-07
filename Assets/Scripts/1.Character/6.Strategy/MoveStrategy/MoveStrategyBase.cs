using UnityEditor.Build.Pipeline;
using UnityEngine;


public enum MoveStrategyType
{
    Straight,
    Random,
    FollowPlayer
}


public abstract class MoveStrategyBase
{
    public abstract void MoveMethod();
    public abstract Vector2 MoveDirection();
    
}
