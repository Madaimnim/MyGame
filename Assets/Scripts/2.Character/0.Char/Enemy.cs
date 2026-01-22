using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//BottomTransform:角色底部位置(水平移動用)
//ScaleTransform:轉向Scale，與UIAnchor分離，避免UI轉向
//SpriteRender:Sprite，純視覺抖動用
//變形、旋轉必須同時針對Collider、Sprite、Shadow，否則回歪斜

public class Enemy :MonoBehaviour,IInteractable,IVisualFacing
{
    //公開--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [SerializeField] private Transform _scaleTransform;
    [SerializeField] private float _heightRange;
    public Transform BackSpriteTransform;

    public Transform BottomTransform => transform;
    public Transform ScaleTransform => _scaleTransform;
    public Collider2D GroundCollider => _groundCollider;
    private Transform _heightTransform => _spr.transform;
    public Transform SpriteTransform => _spr.transform;
    public HeightInfo HeightInfo => new HeightInfo(_heightTransform.localPosition.y,_heightTransform.localPosition.y + _heightRange);

    public GameObject UI_HpSliderCanvas;
    //組件--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [HideInInspector] public Rigidbody2D Rb;
    [HideInInspector] public Animator Ani;
    private SpriteRenderer _spr;
    private Collider2D _groundCollider;
    //模組化 --------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public EnemyStatsRuntime Rt { get; private set; }
    public StateComponent StateComponent { get; private set; } 
    public HealthComponent HealthComponent { get; private set; }
    public RespawnComponent RespawnComponent { get; private set; }
    public AnimationComponent AnimationComponent { get; private set; }
    public EffectComponent EffectComponent { get; private set; }
    public AIComponent AIComponent { get; private set; }
    public MoveComponent MoveComponent { get; private set; }
    public CombatComponent CombatComponent { get; private set; }
    public SpawnerComponent SpawnerComponent { get; private set; }
    public HeightComponent HeightComponent { get; private set; }
    public Vector2 MoveVelocity => MoveComponent.GetCurrentMoveVelocity();

    private Vector3 _lastInteractPosition;
    private HitShakeVisual HitShakeVisual;
    //防二次破壞
    private bool _isDestroyed = false;

    //識別ID--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("手動ID初始化")]public int id; //在Inspector設定敵人ID以載入數據
    private bool _initialized = false;       //確保初始化一次

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        _spr = GetComponentInChildren<SpriteRenderer>();
        Ani = GetComponentInChildren<Animator>();
        HitShakeVisual = GetComponentInChildren<HitShakeVisual>();
        _groundCollider = GetComponentInChildren<Collider2D>();
        //Test Rarity
        //var sr = GetComponentInChildren<SpriteRenderer>();
        //_innerEdgeController = new SpriteInnerEdgeController(sr);
    }
    private void OnEnable()
    {
        //註冊UI:顯示血量、技能冷卻
        if (GameManager.Instance != null)GameManager.Instance.EnemyStateSystem.RegisterEnemy(this);
        if (EnemyListManager.Instance != null)EnemyListManager.Instance.Register(this);

        //UI_HpSliderCanvas.SetActive(false);

    }


    private void OnDisable() {
        if (GameManager.Instance != null) GameManager.Instance.EnemyStateSystem.UnregisterEnemy(this);
        if (EnemyListManager.Instance != null) EnemyListManager.Instance.Unregister(this);
        GameEventSystem.Instance.Event_OnWallBroken -= OnWallBroken;
        GameEventSystem.Instance.Event_OnWallBroken -= AIComponent.DisableAI;
        GameEventSystem.Instance.Event_OnWallBroken -= RespawnComponent.DisableRespawn;

        StopAllCoroutines();
    }

    private void Update() {
        if (CombatComponent != null) CombatComponent.Tick(PlayerListManager.Instance.TargetList);
        if (AIComponent != null && AIComponent.CanRunAI) AIComponent.Tick();
        if (HeightComponent != null) HeightComponent.Tick();

        CombatComponent.DebugIntent(DebugContext.Enemy, Rt.Id);
    }
    private void LateUpdate() {
        if(StateComponent!=null) StateComponent.DebugState();
    }
    private void FixedUpdate()
    {
        if (MoveComponent != null) MoveComponent.FixedTick();
    }
    public void Initialize(EnemyStatsRuntime stats) {
        if (_initialized) return;
        _initialized = true;
  
        Rt = stats;

        //注意依賴順序
        StateComponent = new StateComponent(this,DebugContext.Enemy, Rt.Id);
        HealthComponent = new HealthComponent(Rt, StateComponent);
        AnimationComponent = new AnimationComponent(Ani, transform, Rb,StateComponent);

        HeightComponent = new HeightComponent(_heightTransform, StateComponent ,AnimationComponent, this,Rt.StatsData,
             () => GameSettingManager.Instance.PhysicConfig.GravityScale);
        EffectComponent = new EffectComponent(transform, this, _spr, StateComponent);

        RespawnComponent = new RespawnComponent(this, Rt.CanRespawn);
        MoveComponent = new MoveComponent(Rb,Rt.StatsData, this, AnimationComponent, HeightComponent,StateComponent);
        SpawnerComponent = new SpawnerComponent();
        CombatComponent = new CombatComponent(this,Rt.StatsData, Rt.SkillSlotCount,Rt.SkillPool, AnimationComponent,StateComponent, transform, _groundCollider.transform,
            PlayerListManager.Instance.TargetList,MoveComponent,HeightComponent);
        AIComponent = new AIComponent( MoveComponent, CombatComponent, transform,Rt.MoveStrategy);


        //額外初始化設定
        BehaviorTree behaviourTree =EnemyBehaviourTreeFactory.Create(Rt.EnemyBehaviourTreeType, this);
        AIComponent.SetBehaviorTree(behaviourTree);
        HpSlider hpSlider = GetComponentInChildren<HpSlider>();
        hpSlider.Bind(HealthComponent);

        var enemySkillUseGate = new EnemySkillUseGate();
        CombatComponent.SetSkillUseGate(enemySkillUseGate);

        //事件訂閱
        HealthComponent.OnDie += OnDie;
        HealthComponent.OnHpChanged += OnHpChanged;
        RespawnComponent.OnRespawn += OnRespawn;
        GameEventSystem.Instance.Event_OnWallBroken += OnWallBroken;
        GameEventSystem.Instance.Event_OnWallBroken += AIComponent.DisableAI;
        GameEventSystem.Instance.Event_OnWallBroken += RespawnComponent.DisableRespawn;

        CombatComponent.OnAttackTurn += SetFacingLeft;
        CombatComponent.OnAttackTurn += MoveComponent.SetSkillDashDirection;
        MoveComponent.OnMoveDirectionChanged += SetFacingLeft;

        //初始化狀態--------------------------------------------------------------------------------------------------------------------------------------------------------------------
        transform.name = $"EnemyID_{Rt.Id}:({Rt.Name})";
        ResetState();

        Rt.SkillPool.TryGetValue(1, out var skillRt);
        CombatComponent.EquipSkill(1, skillRt);
        GameEventSystem.Instance.Event_BattleStart+= AIComponent.EnableAI;
        if (GameManager.Instance.GameStageSystem.IsBattleStarted)AIComponent.EnableAI();
    }


    public void Interact(InteractInfo info)
    {
        _lastInteractPosition = info.SourcePosition;  
        EffectComponent.TakeDamageEffect(info.Damage);              //先顯示傷害，儘管為0也顯示

        if (info.Damage <= 0f) return;
        //
        //var directX = info.SourcePosition.x - BottomTransform.localPosition.x;
        //
        //HealthComponent.TakeDamage(info.Damage);
        //MoveComponent.Knockbacked(info);     
        //MoveComponent.StopSkillDashMoveCoroutine();

        EnterHurt(0.5f, info);
    }

    private void EnterHurt(float duration, InteractInfo info) {
        MoveComponent.StopSkillDashMoveCoroutine();

        var directX = info.SourcePosition.x - BottomTransform.localPosition.x;
        HitShakeVisual.Play(HitShakeType.Shake, directX);

        MoveComponent.Knockbacked(info);
        HeightComponent.Hurt(duration, info.FloatPower, HurtType.Hard);
        AnimationComponent.PlayHurt();

        HealthComponent.TakeDamage(info.Damage);
    }

    public void OnHpChanged(int currentHp, int maxHp) {                     //事件方法
        //// 第一次被打（血量不滿）才顯示
        //if (currentHp < maxHp) {
        //    if (!UI_HpSliderCanvas.activeSelf)
        //        UI_HpSliderCanvas.SetActive(true);
        //}
    }      
    public void OnDie() {
        EffectComponent.HideOutline();

        AnimationComponent.PlayDie();
        StartCoroutine(Die());
    }
    private IEnumerator Die()
    {
        AIComponent.DisableAI();


        Vector2 dir = _lastInteractPosition - BottomTransform.transform.position;
        SetFacingLeft(dir); // 重用你現有的方向翻轉方法
                          

        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        yield return null; //等一幀，讓Animator確實切到Die動畫
        AnimatorStateInfo state = Ani.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(state.length / Ani.speed);

        if (RespawnComponent.CanRespawn) RespawnComponent.RespawnAfter(3f);
        else {
            Destroy(gameObject);
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
        StateComponent.ResetState();
        MoveComponent.ClearAllMoveIntent();
        CombatComponent.ClearSkillIntent();
        //CombatComponent.ClearBaseAttackTargetTransform(); //敵人沒有譜攻

        HealthComponent.ResetCurrentHp();
    }
    private void OnWallBroken() {
        if (_isDestroyed) return;
        _isDestroyed = true;

        Destroy(gameObject);
    }

    private void SetFacingLeft(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;     //避免靜止時頻繁執行
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            var scale = ScaleTransform.localScale;
            float mag = Mathf.Abs(scale.x);
            scale.x = (direction.x < 0f) ? mag : -mag;
            ScaleTransform.localScale = scale;


            //if (IsDead) Debug.Log($"更新敵人死亡面相:{transform.localScale}，");
        }
    }

}
