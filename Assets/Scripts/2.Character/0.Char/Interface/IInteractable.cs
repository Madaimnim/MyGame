using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    Transform BottomTransform { get; }
    Collider2D RootSpriteCollider { get; }
    Vector2 MoveVelocity { get; }

    void Interact(InteractInfo info);
}
public struct InteractInfo
{
    public Vector3 SourcePosition;            
    public int Damage;
    public float KnockbackPower;
    public float FloatPower;
}
