using UnityEngine;


public class DamageInfo
{
    public int damage;
    public float knockbackForce;
    public Vector2 knockbackDirection;

}


public interface IDamageable
{
    void TakeDamage(DamageInfo info);
}

