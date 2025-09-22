using System;
using System.Collections;
using UnityEngine;
public abstract class Char<TStat,TSkill> : MonoBehaviour,IDamageable 
    where TStat:CharStats<TSkill>, IRuntime
    where TSkill : SkillBase
{
    public int Id { get; protected set; }
    public bool IsDead { get; protected set; } = false;
    public bool IsKnockbacking { get; protected set; } = false;
    public bool IsPlayingAttackAnimation { get; protected set; } = false;
    public bool CanRunAI { get; protected set; } = false;
    public bool CanMove { get; protected set; } = true;

    public Rigidbody2D RB { get; protected set; }
    public SpriteRenderer SRender{ get; protected set; }
    private Animator Ani;
    public Collider2D Col { get; protected set; }
    public ShadowController ShadControl{ get; protected set; }
    public TStat StatsRuntime { get; protected set; }          // 持有唯一的 Runtime 引用
    public IDamageable owner { get; protected set; }
    public virtual void Initialize(TStat runtime,IDamageable damageable) {
        StatsRuntime = runtime;
        owner = damageable;
        Id = runtime.Id;
    }        

    public virtual void TakeDamage(DamageInfo info) {
        if (IsDead || StatsRuntime == null) return;
        StatsRuntime.TakeDamage(info.damage);

        if (StatsRuntime.CurrentHp <= 0 && !IsDead)
            Die();
    }

    protected virtual void Die() {
        IsDead = true;
    }

    protected virtual void Awake() {
        RB = GetComponent<Rigidbody2D>();
        SRender = GetComponent<SpriteRenderer>();
        Ani = GetComponent<Animator>();
        Col = GetComponent<Collider2D>();
        ShadControl = GetComponentInChildren<ShadowController>();
    }

    protected virtual void OnEnable() {
        ResetMaterial();
    }


    //被擊退
    protected virtual IEnumerator Knockback(float force, Vector2 knockbackDirection) {
        if (RB != null)
        {
            IsKnockbacking = true;

            RB.velocity = Vector2.zero; // 先清除當前速度，避免擊退力疊加
            RB.AddForce(force * knockbackDirection, ForceMode2D.Impulse); // 添加瞬間衝擊力
            yield return new WaitForSeconds(0.2f);
            RB.velocity = Vector2.zero;

            IsKnockbacking = false;
        }
    }

    //閃白特效
    protected virtual IEnumerator FlashWhite(float duration) {
        if (SRender != null)
        {
            SRender.material = GameManager.Instance.flashMaterial;
            yield return new WaitForSeconds(duration);
            SRender.material = GameManager.Instance.normalMaterial;
        }
    }

    //重置狀態參數
    protected virtual void ResetState() {
        IsPlayingAttackAnimation = false;
        IsKnockbacking = false;
        IsDead = false;
    }

    //啟用AI
    protected virtual void EnableAIRun() {
        CanRunAI = true;
    }

    //禁用AI
    protected virtual void DisableAIRun() {
        CanRunAI = false;
    }

    public virtual void PlayAnimation(string animationName) {
        Ani?.Play(Animator.StringToHash(animationName));
    }
    protected void ResetMaterial() {
        if (SRender != null)
            SRender.material = GameManager.Instance.normalMaterial;
    }

}