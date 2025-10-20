using UnityEngine;

public interface IInteractable
{
    Collider2D BottomCollider { get; }

    void Interact(InteractInfo info);
}
public struct InteractInfo
{
    public Transform Source;            
    public int Damage;
    public Vector2 KnockbackForce;
}
