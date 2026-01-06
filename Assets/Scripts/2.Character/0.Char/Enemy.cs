using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//SkillDetectorBase 下轄Circle_Detector、Box_Detector等偵測策略，可生成範圍Sprite物件，無實際技能槽，開關僅關閉可視化範圍

//腳色技能安裝流程:
//PlayerStateSystem.UnlockPlayer()-> PlayerSkillSystem.EquipPlayerSkil()->SkillComponent.EquipSkill->();
//Enemy.Initialized()->EnemySkillSystem.EquipEnemySkill()->EnemySkillComponent.EquipSkill->();

public class Enemy :MonoBehaviour,IInteractable
{
    //公開--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [SerializeField] private Collider2D sprCol;
    public Collider2D SprCol => sprCol;
    public Transform BottomTransform => transform;
    public Transform VisulaRootTransform;
    public Transform BackSpriteTransform;
    public TargetDetector MoveDetector;
    public IInputProvider InputProvider;
    public GameObject UI_HpSliderCanvas;
    //組件--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [HideInInspector] public Rigidbody2D Rb;
    [HideInInspector] public Animator Ani;
    [HideInInspector] public SpriteRenderer Spr;
    //模組化 --------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public EnemyStatsRuntime Rt { get; private set; }
    public StateComponent StateComponent { get; private set; } 
    public ActionLockComponent ActionLockComponent { get; private set; }
    public HealthComponent HealthComponent { get; private set; }
    public RespawnComponent RespawnComponent { get; private set; }
    public AnimationComponent AnimationComponent { get; private set; }
    public EffectComponent EffectComponent { get; private set; }
    public AIComponent AIComponent { get; private set; }
    public MoveComponent MoveComponent { get; private set; }
    public SkillComponent SkillComponent { get; private set; }
    public SpawnerComponent SpawnerComponent { get; private set; }
    public HeightComponent HeightComponent { get; private set; }
    public Vector2 MoveVelocity => MoveComponent.IntentDirection * MoveComponent.MoveSpeed;
    
    private Transform _lastInteractSource;
    private float _initialHeightY ;

    //識別ID--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("手動ID初始化")]public int id; //在Inspector設定敵人ID以載入數據
    private bool _initialized = false;       //確保初始化一次

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Spr = GetComponentInChildren<SpriteRenderer>();
        Ani = GetComponentInChildren<Animator>();
        _initialHeightY = Spr.transform.localPosition.y;
    }
    private void OnEnable()
    {
        //註冊UI:顯示血量、技能冷卻
        if (GameManager.Instance != null)GameManager.Instance.EnemyStateSystem.RegisterEnemy(this);
        if (EnemyListManager.Instance != null)EnemyListManager.Instance.Register(this);

        UI_HpSliderCanvas.SetActive(false);
    }
    private void Start()
    {
        InputProvider = AIComponent;
    }
    private void Update() {
        if (SkillComponent != null)SkillComponent.Tick();
        if (ActionLockComponent != null ) ActionLockComponent.Tick();
        if (AIComponent != null) AIComponent.Tick();

        SkillComponent.TickCooldownTimer();
    }
    private void LateUpdate() {
        StateComponent.DebugState();
    }
    private void FixedUpdate()
    {
        if (MoveComponent != null) MoveComponent.FixedTick();
        if (HeightComponent != null) HeightComponent.FixedTick();
    }
    public void Initialize(EnemyStatsRuntime stats) {
        if (_initialized) return;
        _initialized = true;
  
        Rt = stats;

        //注意依賴順序
        StateComponent = new StateComponent(DebugContext.Enemy, Rt.StatsData.Id);
        ActionLockComponent = new ActionLockComponent(this, StateComponent);
        HealthComponent = new HealthComponent(Rt, StateComponent);
        AnimationComponent = new AnimationComponent(Ani, transform, Rb,StateComponent);

        HeightComponent = new HeightComponent(Spr.transform, StateComponent ,AnimationComponent, this,Rt.StatsData);
        EffectComponent = new EffectComponent(Rt.VisualData, transform, this, Spr, StateComponent);

        RespawnComponent = new RespawnComponent(this, Rt.CanRespawn);
        MoveComponent = new MoveComponent(Rb,Rt.StatsData, this, MoveDetector, AnimationComponent, HeightComponent,StateComponent);
        SpawnerComponent = new SpawnerComponent();
        SkillComponent = new SkillComponent(Rt.StatsData, Rt.SkillSlotCount,Rt.SkillPool, AnimationComponent,StateComponent, transform, sprCol.transform,
            PlayerListManager.Instance.TargetList,MoveComponent,HeightComponent);
        AIComponent = new AIComponent(SkillComponent, MoveComponent, transform,Rt.MoveStrategy);


        //額外初始化設定
        BehaviorTree behaviourTree =EnemyBehaviourTreeFactory.Create(Rt.EnemyBehaviourTreeType, AIComponent, MoveComponent, SkillComponent, Rt.MoveStrategy);
        AIComponent.SetBehaviorTree(behaviourTree);

        HpSlider hpSlider = GetComponentInChildren<HpSlider>();
        hpSlider.Bind(HealthComponent);

        //事件訂閱
        HealthComponent.OnDie += OnDie;
        HealthComponent.OnHpChanged += OnHpChanged;
        RespawnComponent.OnRespawn += OnRespawn;
        if (GameEventSystem.Instance != null)
        {
            GameEventSystem.Instance.Event_BattleStart += AIComponent.EnableAI;
            GameEventSystem.Instance.Event_OnWallBroken += AIComponent.DisableAI;
            GameEventSystem.Instance.Event_OnWallBroken += RespawnComponent.DisableRespawn;
        }
        SkillComponent.OnSkillAnimationPlayed += SetFacingLeft;
        SkillComponent.OnSkillAnimationPlayed += MoveComponent.SetSkillDashDirection;
        MoveComponent.OnMoveDirectionChanged += SetFacingLeft;

        //初始化狀態--------------------------------------------------------------------------------------------------------------------------------------------------------------------
        transform.name = $"EnemyID_{Rt.StatsData.Id}:({Rt.StatsData.Name})";
        InputProvider = AIComponent;
        ResetState();
        SkillComponent.EquipSkill(0, 1);


    }


    public void Interact(InteractInfo info)
    {
        _lastInteractSource= info.Source;
        HealthComponent.TakeDamage(info.Damage);
        EffectComponent.TakeDamageEffect(info.Damage);
        MoveComponent.Knockbacked(info.KnockbackForce, info.Source);

        MoveComponent.StopSkillDashMoveCoroutine();
        HeightComponent.StopSkillDashMoveCoroutine();
        HeightComponent.StopRecoverHeightCoroutine();

        HeightComponent.Hurt(0.5f);
        HeightComponent.AddUpVelocity(info.FloatPower);

        ActionLockComponent.HurtLock(0.5f);
        AnimationComponent.PlayImmediate("Hurt");


    }


    //事件方法
    public void OnHpChanged(int currentHp, int maxHp) {
        // 第一次被打（血量不滿）才顯示
        if (currentHp < maxHp) {
            if (!UI_HpSliderCanvas.activeSelf)
                UI_HpSliderCanvas.SetActive(true);
        }
    }   
    public void OnDie() {
        EffectComponent.HideOutline();

        AnimationComponent.PlayDie();
        StartCoroutine(Die());
    }
    private IEnumerator Die()
    {
        AIComponent.DisableAI();

        if (_lastInteractSource != null)
        {
            Vector2 dir = _lastInteractSource.position - transform.position;
            SetFacingLeft(dir); // 重用你現有的方向翻轉方法
            //Debug.Log($"敵人死亡面相更新來源:{dir}");
        }

        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        yield return null; //等一幀，讓Animator確實切到Die動畫
        AnimatorStateInfo state = Ani.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(state.length / Ani.speed);

        if (RespawnComponent.CanRespawn) RespawnComponent.RespawnAfter(3f);
        else {
            Destroy(gameObject);
            if (StageLevelManager.Instance != null) StageLevelManager.Instance.EnemyDefeated(Rt.Exp);
            if (GameManager.Instance != null) GameManager.Instance.EnemyStateSystem.UnregisterEnemy(this);
        }
    }

    public void OnRespawn() {
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = true;
        ResetState();
        AIComponent.EnableAI();
    }
    public void ResetState() {
        StateComponent.SetIsPlayingAttackAnimation (false);
        MoveComponent.ResetVelocity();
        HealthComponent.ResetCurrentHp();
    }

    private void SetFacingLeft(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;     //避免靜止時頻繁執行
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            var scale = VisulaRootTransform.localScale;
            float mag = Mathf.Abs(scale.x);
            scale.x = (direction.x < 0f) ? mag : -mag;
            VisulaRootTransform.localScale = scale;


            //if (IsDead) Debug.Log($"更新敵人死亡面相:{transform.localScale}，");
        }
    }
}
