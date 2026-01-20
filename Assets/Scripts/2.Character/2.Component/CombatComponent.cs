using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CombatComponent
{
    public Vector2 IntentSkillInitialDirection;
    public Vector2 IntentBaseAttackInitialDirection;

    public bool HasIntentSkill => IntentSkillSlotNumber >= 1 ;
    public int IntentSkillSlotNumber = -1;
    public Transform IntentTargetTransform;
    public Vector2 IntentTargetPosition;
    public ISkillRuntime IntentSkillRt;

    public bool HasBaseAttackTarget=> IntentBaseAttackTargetTransform!=null;
    public Transform IntentBaseAttackTargetTransform;

    public int SkillSlotCount { get; private set; }
    private StatsData _statsData;
    private ISkillRuntime _baseAttackRuntime;
    private Dictionary<int, ISkillRuntime> _skillPool;

    private MonoBehaviour _monoBehaviour;
    private AnimationComponent _animationComponent;
    private MoveComponent _moveComponent;
    private HeightComponent _heightComponent;
    private StateComponent _stateComponent;
    private SpawnerComponent _spawner;
    private Transform _transform;
    private Transform _sprTransform;
    private DetectorSpriteSpawner _detectorSpriteSpawner;



    //技能判斷大門
    private ISkillUseGate _skillUseGate;
    //技能鍊
    private PlayerSkillChain _playerSkillChain;

    //普攻、技能槽
    public BaseAttackSlot BaseAttackSlot;
    public SkillSlot[] SkillSlots;
    public bool HasAnySkillTarget => SkillSlots.Any(slot => slot.HasSkill && slot.Detector != null && slot.Detector.HasTarget);
    //事件
    public event Action OnSkillsChanged; // (slot, skillId, runtime)
    public event Action<Vector2> OnAttackTurn;

    public event Action<int, ISkillRuntime> OnSkillUsed;         // (slot, runtime)
    public event Action<ISkillRuntime> OnAttackHitTarget;        //技能命中事件傳遞

    public CombatComponent(MonoBehaviour monoBehaviour, StatsData statsData,int skillSlotCount,Dictionary<int, ISkillRuntime> skillPool,AnimationComponent animationComponent,
        StateComponent stateComponent,Transform transform, Transform sprTransform, IReadOnlyList<IInteractable> targetList,MoveComponent moveComponent,
        HeightComponent heightComponent,ISkillRuntime baseAttackRuntime=null) {
        _monoBehaviour= monoBehaviour;

        _statsData = statsData;
        SkillSlotCount = skillSlotCount;
        _baseAttackRuntime= baseAttackRuntime;
        _skillPool = skillPool;
    
        _animationComponent = animationComponent;
        _stateComponent = stateComponent;
        _spawner = new SpawnerComponent();
        _transform = transform;
        _sprTransform = sprTransform;
        _moveComponent = moveComponent;
        _heightComponent= heightComponent;

        _detectorSpriteSpawner = new DetectorSpriteSpawner();

        BaseAttackSlot=new BaseAttackSlot(_transform);
        SkillSlots = new SkillSlot[SkillSlotCount];
        for (int i = 0; i < SkillSlotCount; i++)
            SkillSlots[i] = new SkillSlot(_transform);
    }

    public void Tick(IReadOnlyList<IInteractable> targetLists ) {
        
        BaseAttackAnimationTick();
        CheckBaseAttackAnimationIsValid();   // 加這行
        SkillAnimationTick();

        SlotsTick_DetectorAndCooldown(targetLists);
    }

    private void SlotsTick_DetectorAndCooldown(IReadOnlyList<IInteractable> targetLists) {
        BaseAttackSlot.DetectorTick(targetLists);
        BaseAttackSlot.CooldownTick();

        foreach (var slot in SkillSlots) {
            slot?.DetectorTick(targetLists);
            slot?.CooldownTick();
        }
    }

    //普通攻擊
    private void BaseAttackAnimationTick() {
        if (!HasBaseAttackTarget) return;
        if (_stateComponent.IsCastingSkill) return;
        if (_stateComponent.IsBaseAttacking) return; 
        
        // 有目標，但不在距離內 → 發出「移動意圖」
        Vector2 targetPos = IntentBaseAttackTargetTransform.position;
        if (!BaseAttackSlot.Detector.IsInRange(targetPos)) {
            _moveComponent.SetIntentMovePosition (targetPos);
            return;
        }
   
        IntentBaseAttackInitialDirection = (IntentBaseAttackTargetTransform.position - _transform.position).normalized;
        _animationComponent.PlayBaseAttack();

        OnAttackTurn.Invoke(IntentBaseAttackInitialDirection);
    }
    public void SpawnBaseAttack() {
        if (!HasBaseAttackTarget) return;
     
        GameObject obj = _spawner.Spawn(_baseAttackRuntime.VisualData.Prefab, _transform.position, Quaternion.identity);
        var skillObj = obj.GetComponent<SkillObject>();
        skillObj.Initial(_transform, _sprTransform, _statsData, _baseAttackRuntime, IntentBaseAttackInitialDirection, IntentBaseAttackTargetTransform.position, IntentBaseAttackTargetTransform);
        skillObj.OnHitTarget += baseAttackRt => { OnAttackHitTarget?.Invoke(baseAttackRt); };

        BaseAttackSlot.TriggerCooldown(_baseAttackRuntime.Cooldown);          //觸發冷卻
    }
    //檢查普通攻擊中，目標若離開或消失，則取消普攻動畫
    private void CheckBaseAttackAnimationIsValid() {
        if (!_stateComponent.IsBaseAttacking) return;

        // 目標消失
        if (IntentBaseAttackTargetTransform == null) {
            _animationComponent.PlayIdle();
            return;
        }

        // 目標離開攻擊距離
        Vector2 targetPos = IntentBaseAttackTargetTransform.position;
        if (!BaseAttackSlot.Detector.IsInRange(targetPos)) {
            _animationComponent.PlayIdle();
            return;
        }
    }
    //技能攻擊
    private void SkillAnimationTick() {
        if((!HasIntentSkill)) return;
        if(_stateComponent.IsCastingSkill) return;

        _animationComponent.PlaySkill(IntentSkillRt.Id);
        IntentSkillInitialDirection = ((Vector3)IntentTargetPosition-_transform.position).normalized;

        OnAttackTurn.Invoke(IntentSkillInitialDirection);
    }
    public void SpawnSkill() {
        GameObject obj = _spawner.Spawn(IntentSkillRt.VisualData.Prefab, _transform.position, Quaternion.identity);
        var skillObj = obj.GetComponent<SkillObject>();
        skillObj.Initial(_transform, _sprTransform, _statsData, IntentSkillRt, IntentSkillInitialDirection, IntentTargetPosition, IntentTargetTransform);
        skillObj.OnHitTarget += skillRt => { OnAttackHitTarget?.Invoke(skillRt); };

        var skillSlot = SkillSlots[IntentSkillSlotNumber-1];

        skillSlot.TriggerCooldown(IntentSkillRt.Cooldown);          //觸發冷卻
   
        ClearSkillIntent();                                         //清除意圖
        _stateComponent.SetIsCastingSkill(false);             //技能施放完成
        OnSkillUsed?.Invoke(IntentSkillSlotNumber, IntentSkillRt);  //發事件
    }
    //衝刺攻擊
    public void SkillPrepareMove(ISkillRuntime skillRt) {
        _animationComponent.SetParameterBool("IsPrepareReady", false);

        _moveComponent.SkillPrepareMove(skillRt);
        _heightComponent.SkillPrepareMove(skillRt);
    }
    public void SkillDashMove(ISkillRuntime skillRt) {

        _moveComponent.SkillDashMove(skillRt);
        _heightComponent.SkillDashMove(skillRt);
    }

    //裝備普攻、技能
    public void EquipBaseAttack(ISkillRuntime baseAttackRt) {
        BaseAttackSlot.Uninstall();
        BaseAttackSlot.SetSlot(baseAttackRt);
        Debug.Log($"{_transform.name}裝備普攻技能 {baseAttackRt.Name}");
    }
    public void EquipSkill(int slotNumber, ISkillRuntime skillIRt) {
        SkillSlots[slotNumber-1].Uninstall();
        SkillSlots[slotNumber-1].SetSlot(skillIRt);

        //Debug.Log($"{_transform.name}裝備技能ID{skillIRt.Id} {skillIRt.Name} 到技能槽{slotNumber}");

        //發事件
        OnSkillsChanged?.Invoke();
    }
    public void EquipSkillChain(PlayerSkillChain playerSkillChain) {
        _playerSkillChain = playerSkillChain;
    }
    //顯示/隱藏偵測範圍
    public void SetBaseAttackDetectRangeVisible(bool visible) {
        if (BaseAttackSlot == null) return;
        if (BaseAttackSlot.DetectRangeObject == null) return;

        BaseAttackSlot.DetectRangeObject.SetActive(visible);
    }
    public void SetSkillDetectRangeVisible(int slotNumber, bool visible) {
        if (slotNumber < 1 || slotNumber > SkillSlots.Length) return;

        var slot = SkillSlots[slotNumber-1];
        if (slot.DetectRangeObject == null) return;

        slot.DetectRangeObject.SetActive(visible);
    }
    public void SetAllDetectRangesVisible(bool visible) {
        foreach (var slot in SkillSlots) {
            if (slot?.DetectRangeObject != null)
                slot.DetectRangeObject.SetActive(visible);
        }

        SetBaseAttackDetectRangeVisible(visible);
    }

 
    //普攻預設都是slotIndex0，只給玩家使用
    public bool SetIntentBaseAttack(Transform inputTargetTransform) {
        if (inputTargetTransform == null) return false;
        if (_baseAttackRuntime == null) return false;
        if (!_stateComponent.CanBaseAttack) return false;
        if (!BaseAttackSlot.IsReady || BaseAttackSlot.Detector==null) return false;
        
        EnemyOutlineManager.Instance.SetTarget(inputTargetTransform.GetComponent<Enemy>());
        IntentBaseAttackTargetTransform = inputTargetTransform;

        return true;
    }

    //觸發一次型技能意圖
    //若有上個技能，先釋放上個技能
    public bool SetIntentSkill(int inputSlotNumber, Vector2 inputTargetPosition, Transform inputTargetTransform = null) {
        // ===== 檢查區域 =====
        if (inputSlotNumber < 1 || inputSlotNumber > SkillSlots.Length) return false;
        var skillSlot = SkillSlots[inputSlotNumber-1];
        if (!skillSlot.IsReady || skillSlot.Detector == null) return false;
        if (!_skillPool.TryGetValue(skillSlot.SkillId, out var inputSkillRt)) return false;
        if (!_stateComponent.CanCastSkill) return false;
        if(inputSkillRt.SkillReleaseType==SkillReleaseType.Target && inputTargetTransform == null) return false;      
        if (!_skillUseGate.CanUse(inputSkillRt, inputSlotNumber)) return false;

        // ===== 先結算上一招 ===== 
        if (IntentSkillSlotNumber >= 1) {
            SpawnSkill();                                       //釋放上個技能
            _animationComponent.PlayIdle();
        }
        // ===== Commit 區域 =====
        var targetPos = GetTargetPosByReleaseType(inputSkillRt, inputTargetPosition, inputTargetTransform);
        var finalRt = inputSkillRt;

        //技能鍊處理
        if (_playerSkillChain != null) {
            _playerSkillChain.AddChain(_monoBehaviour);
            Debug.Log($"技能鍊++,CurrentChain={_playerSkillChain.CurrentChain}");                                     //技能鍊+1
            if (_playerSkillChain.CurrentChain >= 3) {
                var playerRt = inputSkillRt as PlayerSkillRuntime;
                finalRt = playerRt.EnhancedSkillRuntime;
                Debug.Log($"{inputSkillRt.Name} enhanced to {finalRt.Name}");
                _playerSkillChain.ResetChain();                                        //重置技能鍊
            }
        }

        _skillUseGate.Consume(finalRt, inputSlotNumber);

        IntentSkillSlotNumber = inputSlotNumber;
        IntentSkillRt = finalRt;
        IntentTargetTransform = inputTargetTransform;       //只給追蹤技能使用
        IntentTargetPosition = targetPos;                   //給一般技能使用

        return true;
    }
    //清除意圖
    public void ClearSkillIntent() {
        IntentSkillSlotNumber = -1;
    }
    public void ResetSkillChain() {
        _playerSkillChain?.ResetChain();
    }
    public void ClearBaseAttackTargetTransform() {
        IntentBaseAttackTargetTransform = null;
    }

    public void SetSkillUseGate(ISkillUseGate gate) {
        _skillUseGate = gate;
    }

    private Vector3 GetTargetPosByReleaseType(ISkillRuntime inputSkillRt,Vector2 inputTargetPosition, Transform inputTargetTransform=null) {
        Vector3 targetPos = Vector3.zero; //統一變數方便後續處理

        switch (inputSkillRt.SkillReleaseType) {
            case SkillReleaseType.Target:
                targetPos = inputTargetTransform.position;
                break;
            case SkillReleaseType.Towerd:
                targetPos = inputTargetPosition;
                break;
        }
        return targetPos;
    }
    public void DebugIntent(DebugContext context, int id) {
        if (context == DebugContext.None) return;

        string prefix = context == DebugContext.Player
            ? $"[Player {id}]"
            : $"[Enemy {id}]";

        // ===== BaseAttack Intent =====
        PlayerScreenDebug.Set($"{prefix} HasBaseAttackTarget", HasBaseAttackTarget);
        PlayerScreenDebug.Set($"{prefix} BaseAttackTarget",
            IntentBaseAttackTargetTransform != null
                ? IntentBaseAttackTargetTransform.name
                : "null");

        PlayerScreenDebug.Set($"{prefix} BaseAttackReady",
            BaseAttackSlot != null && BaseAttackSlot.IsReady);

        PlayerScreenDebug.Set($"{prefix} BaseAttackInRange",
            HasBaseAttackTarget &&
            BaseAttackSlot?.Detector != null &&
            BaseAttackSlot.Detector.IsInRange(
                IntentBaseAttackTargetTransform.position));

        // ===== Skill Intent =====
        PlayerScreenDebug.Set($"{prefix} HasIntentSkill", HasIntentSkill);
        PlayerScreenDebug.Set($"{prefix} IntentSkillSlot", IntentSkillSlotNumber);
        PlayerScreenDebug.Set($"{prefix} IntentSkillId",
            IntentSkillRt != null ? IntentSkillRt.Id : -1);

        PlayerScreenDebug.Set($"{prefix} SkillSlotReady",
            HasIntentSkill &&
            SkillSlots[IntentSkillSlotNumber - 1].IsReady);

        PlayerScreenDebug.Set($"{prefix} SkillGateCanUse",
            HasIntentSkill &&
            _skillUseGate != null &&
            _skillUseGate.CanUse(IntentSkillRt, IntentSkillSlotNumber));

        PlayerScreenDebug.Set($"{prefix} IntentTargetPos", IntentTargetPosition);
        PlayerScreenDebug.Set($"{prefix} IntentTargetTransform",
            IntentTargetTransform != null
                ? IntentTargetTransform.name
                : "null");
    }
}