using UnityEngine;

public interface IDamageable
{
    void TakeDamage(
        int damage,
        float knockbackForce, 
        Vector2 knockbackDirection);
}

