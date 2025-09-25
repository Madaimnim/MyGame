using System;
using UnityEngine;

public class CharMovementComponent
{
    private Rigidbody2D _rb;
    public bool IsMoving { get; private set; }
    public bool CanMove { get; private set; } = true;

    public CharMovementComponent(Rigidbody2D rb) {
        _rb = rb ?? throw new ArgumentNullException(nameof(rb));
    }

    public void Move(Vector2 direction, float speed) {
        if (!CanMove || direction == Vector2.zero) {Stop(); return; }

        Vector2 newPosition = _rb.position + direction * speed * Time.fixedDeltaTime;
        _rb.MovePosition(newPosition);                                              // 用 Rigidbody2D 物理方式移動，會自動和其他 Collider2D 發生碰撞
        IsMoving = true;
    }


    public void Stop() {
        IsMoving = false;
        _rb.velocity = Vector2.zero;
    }
}
