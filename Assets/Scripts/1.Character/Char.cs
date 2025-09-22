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
    public TStat StatsRuntime { get; protected set; }          // �����ߤ@�� Runtime �ޥ�
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


    //�Q���h
    protected virtual IEnumerator Knockback(float force, Vector2 knockbackDirection) {
        if (RB != null)
        {
            IsKnockbacking = true;

            RB.velocity = Vector2.zero; // ���M����e�t�סA�קK���h�O�|�[
            RB.AddForce(force * knockbackDirection, ForceMode2D.Impulse); // �K�[���������O
            yield return new WaitForSeconds(0.2f);
            RB.velocity = Vector2.zero;

            IsKnockbacking = false;
        }
    }

    //�{�կS��
    protected virtual IEnumerator FlashWhite(float duration) {
        if (SRender != null)
        {
            SRender.material = GameManager.Instance.flashMaterial;
            yield return new WaitForSeconds(duration);
            SRender.material = GameManager.Instance.normalMaterial;
        }
    }

    //���m���A�Ѽ�
    protected virtual void ResetState() {
        IsPlayingAttackAnimation = false;
        IsKnockbacking = false;
        IsDead = false;
    }

    //�ҥ�AI
    protected virtual void EnableAIRun() {
        CanRunAI = true;
    }

    //�T��AI
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