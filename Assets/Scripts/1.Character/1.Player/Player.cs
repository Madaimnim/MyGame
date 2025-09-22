using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Player : Char<PlayerStatsRuntime, PlayerSkillRuntime>
{
    #region 變數
    public GameObject selectIndicatorPoint;
    public PlayerAI playerAI;

    public PlayerMove playerMove;
    public AudioClip deathSFX;

    private GameObject[] detectors;             //玩家腳色自有一份Detector，不在Runtime上唯一取用


    public bool canRespawn { get; private set; } = true;

    public bool isPlayingAttackAnimation { get; private set; } = false;
    public bool isMoving { get; private set; } = false;

    #endregion

    //生命週期
    #region 生命週期
    protected override void Awake() {
        base.Awake();
    }

    protected override void OnEnable() {
        base.OnEnable();
        ResetState();

        if (UIManager_BattlePlayer.Instance != null)
        {
            StartCoroutine(DelayedRegisterUI());
        }

        if (EventManager.Instance != null)
        {
            EventManager.Instance.Event_BattleStart += EnableAIRun;
            EventManager.Instance.Event_OnWallBroken += DisableAIRun;
            EventManager.Instance.Event_BattleStart += EnableRespawn;
            EventManager.Instance.Event_OnWallBroken += DisableRespawn;
        }
    }

    private void OnDisable() {
        if (UIManager_BattlePlayer.Instance != null)
        {
            UIManager_BattlePlayer.Instance.UnregisterPlayer(this);
        }
        
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Event_BattleStart -= EnableAIRun;
            EventManager.Instance.Event_OnWallBroken -= DisableAIRun;
            EventManager.Instance.Event_BattleStart -= EnableRespawn;
            EventManager.Instance.Event_OnWallBroken -= DisableRespawn;
        }
    }
    #endregion

    public void UpdateSkillCooldowns() {
        foreach (var slot in StatsRuntime.SkillSlots)
        {
            slot?.TickCooldown(Time.deltaTime);
        }
    }
    //延遲註冊UI
    private IEnumerator DelayedRegisterUI() {
        yield return null;
        UIManager_BattlePlayer.Instance.RegisterPlayerPanelUI(this);
    }

    public void SetPlayingAttackAnimation(bool b) {
        isPlayingAttackAnimation = b;
    }
    public void TryMove(Vector2 direction) {
        if (IsDead || IsKnockbacking) return;

        isMoving = direction != Vector2.zero;

        if (!isPlayingAttackAnimation && isMoving)
        {
            PlayAnimation("Move");
        }
        playerMove.Move(direction, this, RB);
    }

    public override void TakeDamage(DamageInfo info) {
        if (StatsRuntime == null) return;

        base.TakeDamage(info);
        TextPopupManager.Instance.ShowTakeDamagePopup(info.damage, transform); // 顯示傷害數字

        StartCoroutine(FlashWhite(0.1f)); // 執行閃白協程
        StartCoroutine(Knockback(info.knockbackForce, info.knockbackDirection));

    }
    protected override void Die() {
        base.Die();

        VFXManager.Instance.Play("PlayerDeath", transform.position);
        AudioManager.Instance.PlaySFX(deathSFX, 0.5f);

        DisableAIRun();

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        if (SRender != null)
        {
            Color c = SRender.color;
            c.a = 0.5f;  // 透明度 0=完全透明，1=完全不透明，你可以調整成 0.3~0.7
            SRender.color = c;
        }

        PlayAnimation("Die");

        ShadControl.SetShadowOffset();

        EventManager.Instance.Event_OnPlayerDie?.Invoke(this);
    }


    public void Respawn() {
        ResetState();
        StatsRuntime.RecoverHp();
        EnableAIRun();


        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = true;
        }

        if (SRender != null)
        {
            Color c = SRender.color;
            c.a = 1f;
            SRender.color = c;
        }

        PlayAnimation("Idle");

        ShadControl.ResetShadow();
    }

    //提供PlayerStateManager呼叫初始化StatsRuntime使用

    public void Initialize(PlayerStatsRuntime stats) {
        StatsRuntime = stats;
        Id = StatsRuntime.Id;
        transform.name = $"玩家ID_{StatsRuntime.Id}:({StatsRuntime.Name})";
        detectors = new GameObject[StatsRuntime.SkillSlots.Length];

        StatsRuntime.InitializeOwner(this);
    }


    //啟禁用AI、啟禁用復活
    private void EnableRespawn() {
        canRespawn = true;
    }
    private void DisableRespawn() {
        canRespawn = false;
    }

    //取得人物狀態API
    public int GetPlayerAttackPower() => StatsRuntime.AttackPower;
    public int GetSkillSlotsLength() => StatsRuntime.SkillSlots.Length;

    public void SetSkillSlot(int slotIndex, PlayerSkillRuntime skillData) {
        if (slotIndex < 0 || slotIndex >= StatsRuntime.SkillSlots.Length) return;
        // 綁定資料
        StatsRuntime.SkillSlots[slotIndex].BindSkill(skillData);

        // 清理舊 Detector（只清理自己這個 Player 身上的，不影響別人）
        if (detectors[slotIndex] != null)
        {
            Destroy(detectors[slotIndex]);
            detectors[slotIndex] = null;
        }

        // 生成新 Detector
        if (skillData != null && skillData.TargetDetectPrefab != null)
        {
            var detector = Instantiate(skillData.TargetDetectPrefab, transform);
            detector.transform.localPosition = Vector3.zero;
            detector.name = $"TargetDetector_{skillData.SkillName}_ID:{skillData.SkillId}";
            detectors[slotIndex] = detector;
        }

    }

    public GameObject GetSkillSlotDetector(int slotIndex) =>
    (slotIndex < 0 || slotIndex >= detectors.Length) ? null : detectors[slotIndex];


}
