using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    Transform BottomTransform { get; }
    Collider2D SprCol { get; }
    Vector2 MoveVelocity { get; }

    void Interact(InteractInfo info);
}
public struct InteractInfo
{
    public Transform Source;            
    public int Damage;
    public Vector2 KnockbackForce;
    public float FloatPower;
}
