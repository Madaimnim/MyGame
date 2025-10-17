using UnityEngine;


public class DamageInfo
{
    public int Damage;
    public float KnockbackForce;
    public Vector2 KnockbackDirection;

}


public interface IDamageable
{
    void TakeDamage(DamageInfo info);
}

