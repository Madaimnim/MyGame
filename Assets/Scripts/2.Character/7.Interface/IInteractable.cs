using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    float BottomY { get; }
    float HeightY { get; }
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
