using System;
using UnityEngine;

public interface IInputProvider
{
    Vector2 GetMoveDirection();
    bool IsAttackPressed();
}
