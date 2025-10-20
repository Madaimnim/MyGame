using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    private SpawnerComponent _spawner;
    private Transform _transform;

    //技能暫存狀態
    private int _pendingSlotIndex=-1;
    private Vector2 _pendingPosition;
    private Transform _pendingTransform;

    public SkillSlot[] SkillSlots;
    public bool HasAnyTarget =>
        SkillSlots.Any(slot => slot.HasSkill && slot.Detector != null && slot.Detector.HasTarget);

    public event Action OnSkillsChanged; // (slot, skillId, runtime)
    public event Action<int, ISkillRuntime> OnSkillUsed;         // (slot, runtime)

    public SkillComponent(StatsData statsData, int skillSlotCount, Dictionary<int, ISkillRuntime> skillPool, AnimationComponent animationComponent, Transform transform) {
        _statsData = statsData;
        SkillSlotCount = skillSlotCount;
        _skillPool = skillPool;

        _animationComponent = animationComponent;
        _spawner = new SpawnerComponent();
        _transform = transform;

        SkillSlots = new SkillSlot[SkillSlotCount];
        for (int i = 0; i < SkillSlotCount; i++)
            SkillSlots[i] = new SkillSlot();
    }

    public void Tick() {
        TryPlaySkillAnimation();
        TickCooldownTimer();
    }

    private void TryPlaySkillAnimation() {
        if (_animationComponent.IsPlayingAttackAnimation) return;                             // 正在施放中，不能再播放
        if (IntentSlotIndex < 0) return;
        var slot = SkillSlots[IntentSlotIndex];
        if (!_skillPool.TryGetValue(slot.SkillId, out var skill)) return;
        Vector3 targetPos = Vector3.zero; //統一變數方便後續處理

        switch (skill.TargetType)
        {
            case SkillTargetType.Target:
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

            case SkillTargetType.Point:
                if (!IntentTargetPosition.HasValue)
                {
                    return;
                }
                _pendingTransform = null;
                targetPos = IntentTargetPosition.Value;
                break;
        }
        Vector2 direction = (targetPos - _transform.position).normalized;

        _pendingPosition = targetPos;
        _pendingSlotIndex = IntentSlotIndex;

        _animationComponent.PlayAttackAnimation(skill.StatsData.Id);
        _animationComponent.FaceDirection(direction);
    }

    public void UseSkill() {
        var slot = SkillSlots[_pendingSlotIndex];
        if (!_skillPool.TryGetValue(slot.SkillId, out var skill)) return;

        //數值計算
        int finalAttackPower = _statsData.Power + skill.StatsData.Power;
        Vector2 direction = (_pendingPosition - (Vector2)_transform.position).normalized;

        //呼叫Spawner
        if (skill.VisualData.Prefab != null)
        {
            GameObject obj = _spawner.Spawn(skill.VisualData.Prefab, _transform.position, Quaternion.identity);
            var skillObj = obj.GetComponent<SkillObject>();
            skillObj.Initial(
                finalAttackPower,
                skill.StatsData.KnockbackForce + skill.StatsData.KnockbackForce,
                _pendingPosition,
                _pendingTransform
                );

            slot.TriggerCooldown(skill.Cooldown);

            // 發事件
            OnSkillUsed?.Invoke(_pendingSlotIndex, skill);
        }


        // 技能施放結束 → 解鎖狀態與Intent重置
        IntentSlotIndex = -1;
        IntentTargetPosition = null;
        IntentTargetTransform = null;

        _pendingSlotIndex = -1;
    }

    public void EquipSkill(int slotIndex, int skillId,GameObject detectorPrefab) {
        SkillSlots[slotIndex].Uninstall();

        if (detectorPrefab != null)
        {
            var detector = new Detector(detectorPrefab, _transform, $"Detector_{skillId}");
            SkillSlots[slotIndex].SetSlot(skillId, detector);

            //發事件
            OnSkillsChanged?.Invoke();
        }       
    }

    private void TickCooldownTimer() {
        foreach (var slot in SkillSlots)
            slot?.TickCooldown(Time.deltaTime);
    }
}