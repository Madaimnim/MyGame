using UnityEngine;

public interface IDamageable
{
    void TakeDamage(
        int damage,
        float knockbackForce, 
        Vector2 knockbackDirection,

        float dotDuration,
        float dotDamage,

        float attackReduction,
        float attackReductionDuration,

        float speedReduction,
        float speedReductionDuration);
}

