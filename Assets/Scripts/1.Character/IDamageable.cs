public interface IDamageable
{
    void TakeDamage(
        int damage,
        float knockbackForce, 

        float dotDuration,
        float dotDamage,

        float attackReduction,
        float attackReductionDuration,

        float speedReduction,
        float speedReductionDuration);
}

