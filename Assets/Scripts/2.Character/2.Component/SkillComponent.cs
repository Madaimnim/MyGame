using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class SkillComponent
{
    public Transform IntentTargetTransform;
    public Vector2? IntentTargetPosition;
    public int IntentSlotIndex = -1;

    public int SkillSlotCount { get; private set; }
    private StatsData _statsData;
    private Dictionary<int, ISkillRuntime> _skillPool;
    private AnimationComponent _animationComponent;
    private MoveComponent _moveComponent;
    private HeightComponent _heightComponent;
    private StateComponent _stateComponent;
    private SpawnerComponent _spawner;
    private Transform _transform;
    private Transform _sprTransform;
    private DetectorSpriteSpawner _detectorSpriteSpawner;


    //技能暫存狀態
    private int _pendingSlotIndex=-1;
    private Vector2 _pendingPosition;
    private Transform _pendingTransform;

    public SkillSlot[] SkillSlots;
    public bool HasAnyTarget => SkillSlots.Any(slot => slot.HasSkill && slot.Detector != null && slot.Detector.HasTarget);

    public event Action OnSkillsChanged; // (slot, skillId, runtime)
    public event Action<Vector2> OnSkillAnimationPlayed;
    public event Action<int, ISkillRuntime> OnSkillUsed;         // (slot, runtime)

    public SkillComponent(StatsData statsData,int skillSlotCount,Dictionary<int, ISkillRuntime> skillPool,AnimationComponent animationComponent,
        StateComponent stateComponent,Transform transform, Transform sprTransform, IReadOnlyList<IInteractable> targetList,MoveComponent moveComponent,HeightComponent heightComponent) {
        _statsData = statsData;
        SkillSlotCount = skillSlotCount;
        _skillPool = skillPool;
        
        _animationComponent = animationComponent;
        _stateComponent = stateComponent;
        _spawner = new SpawnerComponent();
        _transform = transform;
        _sprTransform = sprTransform;
        _moveComponent = moveComponent;
        _heightComponent= heightComponent;

        _detectorSpriteSpawner = new DetectorSpriteSpawner();

        SkillSlots = new SkillSlot[SkillSlotCount];
        for (int i = 0; i < SkillSlotCount; i++)
            SkillSlots[i] = new SkillSlot(_transform, targetList);
    }

    public void Tick() {
        if (IntentSlotIndex >= 0) _stateComponent.SetIsAttackingIntent(true);
        else _stateComponent.SetIsAttackingIntent(false);
        TryPlaySkillAnimation();
    }

    private void TryPlaySkillAnimation() {
        if (!_stateComponent.CanAttack) return;

        if (IntentSlotIndex < 0) return;
        var slot = SkillSlots[IntentSlotIndex];
        if (!_skillPool.TryGetValue(slot.SkillId, out var skill)) return;
        Vector3 targetPos = Vector3.zero; //統一變數方便後續處理

        switch (skill.SkillReleaseType)
        {
            case SkillReleaseType.Target:
                if (IntentTargetTransform == null)
                {
                    Debug.Log("指定技能沒有目標，無法施放");
                    IntentSlotIndex = -1;
                    IntentTargetPosition = null;
                    IntentTargetTransform = null;
                    return;
                }
                _pendingTransform = IntentTargetTransform;
                targetPos = IntentTargetTransform.position;
                break;

            case SkillReleaseType.Towerd:
                if (!IntentTargetPosition.HasValue)
                {
                    return;
                }
                _pendingTransform = IntentTargetTransform? IntentTargetTransform:null;
                targetPos = IntentTargetPosition.Value;
                break;
        }
        Vector2 direction = (targetPos - _transform.position).normalized;

        _pendingPosition = targetPos;
        _pendingSlotIndex = IntentSlotIndex;

        _animationComponent.PlayAttack(skill.StatsData.Id);
        _stateComponent.SetIsPlayingAttackAnimation(true);
        OnSkillAnimationPlayed.Invoke(direction);
    }

    public void UseSkill() {
        var slot = SkillSlots[_pendingSlotIndex];
        if (!_skillPool.TryGetValue(slot.SkillId, out var skillRt)) return;

        //數值計算
        Vector2 direction = (_pendingPosition - (Vector2)_transform.position).normalized;

        //呼叫Spawner
        if (skillRt.VisualData.Prefab != null)
        {
            GameObject obj = _spawner.Spawn(skillRt.VisualData.Prefab, _transform.position, Quaternion.identity);
            var skillObj = obj.GetComponent<SkillObject>();

            skillObj.Initial(_transform, _sprTransform, _statsData, skillRt, _pendingPosition,_pendingTransform);
            slot.TriggerCooldown(skillRt.Cooldown);
            // 發事件
            OnSkillUsed?.Invoke(_pendingSlotIndex, skillRt);
        }


        // 技能施放結束 → 解鎖狀態與Intent重置
        IntentSlotIndex = -1;
        IntentTargetPosition = null;
        IntentTargetTransform = null;

        _pendingSlotIndex = -1;
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


    public void EquipSkill(int slotIndex, int skillId) {
        SkillSlots[slotIndex].Uninstall();
        SkillSlots[slotIndex].SetSlot(skillId, _skillPool[skillId].Detector);

        SetAllDetectRangesVisible(false);


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

    public void TickCooldownTimer() {
        foreach (var slot in SkillSlots) slot?.Tick();
    }
}