using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Player:MonoBehaviour,IDamageable
{
    //封裝--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public PlayerStatsRuntime Rt { get; private set; }
    public CharHealthComponent CharHealthComponent { get; protected set; }
    public CharRespawnComponent CharRespawnComponent { get; protected set; }
    public CharAnimationComponent CharAnimationComponent { get;protected set; }
    public CharEffectComponent CharEffectComponent { get; protected set; }
    public CharExpComponent CharExpComponent { get; protected set; }
    public CharBattleComponent CharBattleComponent { get; protected set; }
    public CharMovementComponent CharMovementComponent { get; protected set; }
    public CharAIComponent CharAIComponent { get; protected set; }
    public CharSkillComponent CharSkillComponent { get; protected set; }

    //Unity元件--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Rigidbody2D Rb { get; protected set; }
    public SpriteRenderer Spr { get; protected set; }
    public Animator Ani{ get; protected set; }
    public Collider2D Col { get; protected set; }
    public ShadowController ShadowControl { get; protected set; }
 
    //公開--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public GameObject SelectIndicatorPoint;
    public PlayerAI PlayerAI;
    public bool IsDead => CharHealthComponent.IsDead;
    public bool IsKnockbacking => CharBattleComponent.IsKnockbacking;
    public IInputProvider InputProvider { get; private set; }
    //私有--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private GameObject[] _detectors;             //玩家腳色自有一份Detector，不在Rt上唯一取用


    #region 生命週期
    private void Awake() {
        Rb = GetComponent<Rigidbody2D>();
        Spr = GetComponent<SpriteRenderer>();
        Ani = GetComponent<Animator>();
        Col = GetComponent<Collider2D>();
        ShadowControl = GetComponentInChildren<ShadowController>();
    }
    private void OnEnable(){}
    private void OnDisable() {
        if (CharRespawnComponent != null) CharRespawnComponent.OnRespawn -= OnRespawn;
        if (CharHealthComponent != null)
        {
            CharHealthComponent.OnDie -= OnDie;
            CharHealthComponent.OnHpChanged -= OnHpChanged;
        }
        if (CharExpComponent != null)
        {
            CharExpComponent.OnLevelUp -= OnLevelUp;
            CharExpComponent.OnExpGained -= OnExpGained;
            CharExpComponent.OnExpChanged -= OnExpChanged;
        }

        if (UIManager_BattlePlayer.Instance != null)
        {
            UIManager_BattlePlayer.Instance.UnregisterPlayer(this);
        }      

        if (GameEventSystem.Instance != null)
        {
            GameEventSystem.Instance.Event_BattleStart -= EnableAIRun;
            GameEventSystem.Instance.Event_OnWallBroken -= DisableAIRun;
            if (CharRespawnComponent != null)
            {
                GameEventSystem.Instance.Event_BattleStart -= CharRespawnComponent.EnableRespawn;
                GameEventSystem.Instance.Event_OnWallBroken -= CharRespawnComponent.DisableRespawn;
            }
        }
    }
    private void Update() {
        //確定當前狀態能不能控制
        if (InputProvider == null) return;
        // 移動輸入
        Vector2 dir = InputProvider.GetMoveDirection();
        ApplyInput(dir);

        // 攻擊輸入（如果是 AI，就來自行為樹）
        if (InputProvider.IsAttackPressed())
        {
            CharBattleComponent.DoAttack(); // 或呼叫技能系統
        }
    }

    #endregion

    public void Initialize(PlayerStatsRuntime stats) {
        Rt = stats;

        //初始化模組、訂閱模組事件-----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        CharHealthComponent = new CharHealthComponent(Rt);
        CharHealthComponent.OnDie +=OnDie;
        CharHealthComponent.OnHpChanged +=OnHpChanged;

        CharRespawnComponent = new CharRespawnComponent(this);
        CharRespawnComponent.OnRespawn += OnRespawn;

        CharAnimationComponent = new CharAnimationComponent(Ani);

        CharEffectComponent = new CharEffectComponent(Rt,transform);

        CharExpComponent = new CharExpComponent(Rt);
        CharExpComponent.OnLevelUp += OnLevelUp;
        CharExpComponent.OnExpGained += OnExpGained;
        CharExpComponent.OnExpChanged += OnExpChanged;

        CharBattleComponent = new CharBattleComponent();

        CharMovementComponent = new CharMovementComponent(Rb);

        CharAIComponent = new CharAIComponent();

        CharSkillComponent = new CharSkillComponent(Rt,gameObject);

        //角色物件命名---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        transform.name = $"玩家ID_{Rt.StatsData.Id}:({Rt.StatsData.Name})";
        _detectors = new GameObject[Rt.SkillSlotCount];

        //訂閱外部事件---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        if (GameEventSystem.Instance != null)
        {
            GameEventSystem.Instance.Event_BattleStart += EnableAIRun;
            GameEventSystem.Instance.Event_OnWallBroken += DisableAIRun;

            GameEventSystem.Instance.Event_BattleStart += CharRespawnComponent.EnableRespawn;
            GameEventSystem.Instance.Event_OnWallBroken += CharRespawnComponent.DisableRespawn;
        }

        //註冊UI:顯示血量、技能冷卻
        if (UIManager_BattlePlayer.Instance != null)
        {
            StartCoroutine(DelayedRegisterUI());
        }
        //初始化狀態--------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ResetState();
        Rt.InitializeOwner(this);
    }

    //設定控制來源
    public void SetInputProvider(IInputProvider provider) {
        InputProvider = provider;
        // 如果是 AI → 開啟 AI
        if (provider is PlayerInputController)
            DisableAIRun(); 
        else
            EnableAIRun();
    }

    public void UpdateSkillCooldowns() {
        foreach (var slot in Rt.SkillSlots)
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
        CharAnimationComponent.IsPlayingAttackAnimation = b;
    }
    //移動輸入窗口
    public void ApplyInput(Vector2 direction) {
        if (CharHealthComponent.IsDead || CharBattleComponent.IsKnockbacking) return;

        //移動
        if (direction == Vector2.zero) CharMovementComponent.Stop();
        else CharMovementComponent.Move(direction, Rt.StatsData.MoveSpeed);
        
        //撥放移動動畫
        if (!CharAnimationComponent.IsPlayingAttackAnimation && CharMovementComponent.IsMoving)
            CharAnimationComponent.Play("Move");

        //翻轉朝向
        UpdateFacing(direction);
    }
    private void UpdateFacing(Vector2 direction) {
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            var s = transform.localScale;
            float mag = Mathf.Abs(s.x);
            s.x = (direction.x < 0f) ? -mag : mag;
            transform.localScale = s;
        }
    }

    public void TakeDamage(DamageInfo info) {
        if (Rt == null) return;
        CharHealthComponent.TakeDamage(info.damage);

        StartCoroutine(FlashWhite(0.1f)); // 執行閃白協程
        StartCoroutine(Knockback(info.knockbackForce, info.knockbackDirection));

        TextPopupManager.Instance.ShowTakeDamagePopup(info.damage, transform); // 顯示傷害數字
    }
    //HealthComponent
    public void OnDie() {
        DisableAIRun();

        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;
        if (Spr != null)
        {
            Color c = Spr.color;
            c.a = 0.5f;  // 透明度 0=完全透明，1=完全不透明，你可以調整成 0.3~0.7
            Spr.color = c;
        }

        CharAnimationComponent.Play("Die");
        CharEffectComponent.PlayDeathEffect();

        ShadowControl.SetShadowOffset();
        
        //發事件
        GameEventSystem.Instance.Event_OnPlayerDie?.Invoke(this);
    }
    public void OnHpChanged(int currentHp,int maxHp) {
        
    }
    //RespawnComponent
    public void OnRespawn() {
        ResetState();
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = true;
        }
        if (Spr != null)
        {
            Color c = Spr.color;
            c.a = 1f;
            Spr.color = c;
        }

        CharAnimationComponent.Play("Idle");
        ShadowControl.ResetShadow();
        EnableAIRun();
    }
    //ExpComponent
    public void OnLevelUp(int newLevel) {
        TextPopupManager.Instance.ShowLevelUpPopup(newLevel, transform);
        //撥放音效
    }
    public void OnExpGained(int exp) {
        //跳出獲得經驗值動畫
    }
    public void OnExpChanged(int currentExp ,int expToNext) {
        //更新UI
    }

    protected virtual IEnumerator Knockback(float force, Vector2 knockbackDirection) {
        if (Rb != null)
        {
            CharBattleComponent.IsKnockbacking = true;

            Rb.velocity = Vector2.zero; // 先清除當前速度，避免擊退力疊加
            Rb.AddForce(force * knockbackDirection, ForceMode2D.Impulse); // 添加瞬間衝擊力
            yield return new WaitForSeconds(0.2f);
            Rb.velocity = Vector2.zero;

            CharBattleComponent.IsKnockbacking = false;
        }
    }
    protected virtual IEnumerator FlashWhite(float duration) {
        if (Spr != null)
        {
            Spr.material = Rt.VisualData.FlashMaterial;
            yield return new WaitForSeconds(duration);
            Spr.material = Rt.VisualData.NormalMaterial;
        }
    }

    //重置狀態
    public void ResetState() {
        CharAnimationComponent.IsPlayingAttackAnimation = false;
        CharBattleComponent.IsKnockbacking = false;
        CharHealthComponent.ResetCurrentHp();

        if (Spr != null)
        { // 確保顏色重設
            var c = Spr.color;
            c.a = 1f;
            Spr.color = c;
        }
    }
    protected void ResetMaterial() {
        if (Spr != null)
            Spr.material = Rt.VisualData.NormalMaterial;
    }


    //AI
    public void EnableAIRun() {
        CharAIComponent.CanRunAI = true;
    }
    public void DisableAIRun() {
        CharAIComponent.CanRunAI = false;
    }


    //取得人物狀態API
    public int GetPlayerAttackPower() => Rt.StatsData.Power;
    public int GetSkillSlotsLength() => Rt.SkillSlotCount;
    public void SetSkillSlot(int slotIndex, int skillId) {
        if (slotIndex < 0 || slotIndex >= Rt.SkillSlotCount) return;
        // 綁定資料
        Rt.SkillSlots[slotIndex].SetId(skillId);

        // 清理舊 Detector（只清理自己這個 Player 身上的，不影響別人）
        if (_detectors[slotIndex] != null)
        {
            Destroy(_detectors[slotIndex]);
            _detectors[slotIndex] = null;
        }
 
        // 生成新 Detector
        if (Rt.PlayerSkillPool.TryGetValue(skillId, out var skillRt) 
            && skillRt.VisualData.DetectPrefab != null)
        {
            var detector = Instantiate(skillRt.VisualData.DetectPrefab, transform);
            detector.transform.localPosition = Vector3.zero;
            detector.name = $"TargetDetector_{skillRt.StatsData.Name}_ID:{skillRt.StatsData.Id}";
            _detectors[slotIndex] = detector;
        }

    }
    public GameObject GetSkillSlotDetector(int slotIndex) =>
    (slotIndex < 0 || slotIndex >= _detectors.Length) ? null : _detectors[slotIndex];


}
