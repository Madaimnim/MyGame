using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CombatComponent
{
    public Vector2 IntentSkillInitialDirection;
    public Vector2 IntentBaseAttackInitialDirection;

    public bool HasIntentSkill => IntentSkillSlotIndex > 0 ;
    public int IntentSkillSlotIndex = -1;
    public Transform IntentTargetTransform;
    public Vector2 IntentTargetPosition;
    public ISkillRuntime IntentSkillRt;

    public bool HasBaseAttackTarget=> IntentBaseAttackTargetTransform!=null;
    public Transform IntentBaseAttackTargetTransform;

    public int SkillSlotCount { get; private set; }
    private StatsData _statsData;
    private ISkillRuntime _baseAttackRuntime;
    private Dictionary<int, ISkillRuntime> _skillPool;
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

    public BaseAttackSlot BaseAttackSlot;
    public SkillSlot[] SkillSlots;
    public bool HasAnyTarget => SkillSlots.Any(slot => slot.HasSkill && slot.Detector != null && slot.Detector.HasTarget);
    //事件
    public event Action OnSkillsChanged; // (slot, skillId, runtime)
    public event Action<Vector2> OnSkillAnimationPlayed;
    public event Action<int, ISkillRuntime> OnSkillUsed;         // (slot, runtime)
    public event Action<ISkillRuntime> OnSkillHitTarget;        //技能命中事件傳遞

    public CombatComponent(StatsData statsData,int skillSlotCount,Dictionary<int, ISkillRuntime> skillPool,AnimationComponent animationComponent,
        StateComponent stateComponent,Transform transform, Transform sprTransform, IReadOnlyList<IInteractable> targetList,MoveComponent moveComponent,
        HeightComponent heightComponent,ISkillRuntime baseAttackRuntime=null) {
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
        SkillAnimationTick();
        SkillSlotsTick(targetLists);
    }

    private void SkillSlotsTick(IReadOnlyList<IInteractable> targetLists) {
        foreach (var slot in SkillSlots) {
            slot?.DetectorTick(targetLists);
            slot?.CooldownTick();
        }
    }

    private void BaseAttackAnimationTick() {
        if (!HasBaseAttackTarget) return;
        if (_stateComponent.IsCastingSkill) return;

        if (IntentBaseAttackTargetTransform == null) {
            _moveComponent.ClearAllMoveIntent();
            return;
        }

        // 還沒進距離 → 發出「移動意圖」
        Vector2 targetPos = IntentBaseAttackTargetTransform.position;
        if (!SkillSlots[0].Detector.IsInRange(targetPos)) {
            _moveComponent.IntentTargetTransform = IntentTargetTransform;
            _moveComponent.IntentTargetPosition = targetPos;
            return;
        }

        IntentBaseAttackInitialDirection = (_transform.position - IntentBaseAttackTargetTransform.position).normalized;
        _animationComponent.PlayBaseAttack();
    }
    public void UseBaseAttack() {
        GameObject obj = _spawner.Spawn(_baseAttackRuntime.VisualData.Prefab, _transform.position, Quaternion.identity);
        var skillObj = obj.GetComponent<SkillObject>();
        skillObj.Initial(_transform, _sprTransform, _statsData, _baseAttackRuntime, IntentBaseAttackInitialDirection, IntentBaseAttackTargetTransform.position, IntentBaseAttackTargetTransform);
        skillObj.OnHitTarget += skillRt => { OnSkillHitTarget?.Invoke(skillRt); };

        var skillSlot = SkillSlots[IntentSkillSlotIndex];
        skillSlot.TriggerCooldown(IntentSkillRt.Cooldown);          //觸發冷卻
        _skillUseGate.Consume(IntentSkillRt, IntentSkillSlotIndex); //消耗能量

    }

    private void SkillAnimationTick() {
        if((!HasIntentSkill)) return;
        if(_stateComponent.IsCastingSkill) return;

        _animationComponent.PlaySkill(IntentSkillRt.Id);
        IntentSkillInitialDirection = (_transform.position - (Vector3)IntentTargetPosition).normalized;
        OnSkillAnimationPlayed.Invoke(IntentSkillInitialDirection);
    }
    public void UseSkill() {
        GameObject obj = _spawner.Spawn(IntentSkillRt.VisualData.Prefab, _transform.position, Quaternion.identity);
        var skillObj = obj.GetComponent<SkillObject>();
        skillObj.Initial(_transform, _sprTransform, _statsData, IntentSkillRt, IntentSkillInitialDirection, IntentTargetPosition, IntentTargetTransform);
        skillObj.OnHitTarget += skillRt => { OnSkillHitTarget?.Invoke(skillRt); };

        var skillSlot = SkillSlots[IntentSkillSlotIndex];

        skillSlot.TriggerCooldown(IntentSkillRt.Cooldown);          //觸發冷卻
        _skillUseGate.Consume(IntentSkillRt, IntentSkillSlotIndex); //消耗能量
        ClearSkillIntent();                                         //清除意圖
        _stateComponent.SetIsCastingSkill(false);             //技能施放完成
        OnSkillUsed?.Invoke(IntentSkillSlotIndex, IntentSkillRt);   //發事件
    }

    public void SkillPrepareMove(ISkillRuntime skillRt) {
        _animationComponent.SetParameterBool("IsPrepareReady", false);

        _moveComponent.SkillPrepareMove(skillRt);
        _heightComponent.SkillPrepareMove(skillRt);
    }
    public void SkillDashMove(ISkillRuntime skillRt) {

        _moveComponent.SkillDashMove(skillRt);
        _heightComponent.SkillDashMove(skillRt);
    }

    public void EquipBaseAttack(ISkillRuntime baseAttackRt) {
        BaseAttackSlot.Uninstall();
        BaseAttackSlot.SetSlot(baseAttackRt);
        Debug.Log($"{_transform.name}裝備普攻技能 {baseAttackRt.Name}");
    }
    public void EquipSkill(int slotNumber, ISkillRuntime skillIRt) {
        SkillSlots[slotNumber-1].Uninstall();
        SkillSlots[slotNumber-1].SetSlot(skillIRt);

        Debug.Log($"{_transform.name}裝備技能ID{skillIRt.Id} {skillIRt.Name} 到技能槽{slotNumber}");

        //發事件
        OnSkillsChanged?.Invoke();
    }


    public void SetDetectRangeVisible(int slotIndex, bool visible) {
        if (slotIndex < 0 || slotIndex >= SkillSlots.Length) return;

        var slot = SkillSlots[slotIndex];
        if (slot.DetectRangeObject == null) return;

        slot.DetectRangeObject.SetActive(visible);
    }
    public void SetAllDetectRangesVisible(bool visible) {
        foreach (var slot in SkillSlots) {
            if (slot?.DetectRangeObject != null)
                slot.DetectRangeObject.SetActive(visible);
        }
    }

 
    //普攻預設都是slotIndex0，只給玩家使用
    public bool SetIntentBaseAttack(Transform inputTargetTransform) {
        var baseAttackSlot = SkillSlots[0];
        if (!_skillPool.TryGetValue(baseAttackSlot.SkillId, out var baseAttackRt)) return false;
        if (inputTargetTransform == null) return false;
        if (!_stateComponent.CanBaseAttack) return false;
        if (!baseAttackSlot.IsReady || baseAttackSlot.Detector==null) return false;

        IntentBaseAttackTargetTransform = inputTargetTransform;

        return true;
    }

    //觸發一次型技能意圖設定
    public bool SetIntentSkill(int inputSlotIndex, Vector2 inputTargetPosition, Transform inputTargetTransform = null) {
        if(inputSlotIndex < 0 || inputSlotIndex > SkillSlots.Length) return false;
        var skillSlot = SkillSlots[inputSlotIndex];
        if (!skillSlot.IsReady || skillSlot.Detector == null) return false;
        if (!_skillPool.TryGetValue(skillSlot.SkillId, out var inputSkillRt)) return false;

        if (!_stateComponent.CanCastSkill) return false;
        if(inputSkillRt.SkillReleaseType==SkillReleaseType.Target && inputTargetTransform == null) return false;
        if (!_skillUseGate.CanUse(inputSkillRt, inputSlotIndex)) return false;

        var targetPos = GetTargetPosByReleaseType(inputTargetPosition, inputTargetTransform);

        IntentSkillSlotIndex = inputSlotIndex;
        IntentSkillRt = inputSkillRt;
        IntentTargetTransform = inputTargetTransform;       //只給追蹤技能使用
        IntentTargetPosition = targetPos;                   //給一般技能使用

        return true;
    }

    public void ClearSkillIntent() {
        IntentSkillSlotIndex = -1;
    }
    public void ClearBaseAttackTargetTransform() {
        IntentBaseAttackTargetTransform = null;
    }

    public void SetSkillUseGate(ISkillUseGate gate) {
        _skillUseGate = gate;
    }

    private Vector3 GetTargetPosByReleaseType(Vector2 inputTargetPosition, Transform inputTargetTransform) {
        Vector3 targetPos = Vector3.zero; //統一變數方便後續處理
        switch (IntentSkillRt.SkillReleaseType) {
            case SkillReleaseType.Target:
                targetPos = inputTargetTransform.position;
                break;
            case SkillReleaseType.Towerd:
                targetPos = inputTargetPosition;
                break;
        }
        return targetPos;
    }
}