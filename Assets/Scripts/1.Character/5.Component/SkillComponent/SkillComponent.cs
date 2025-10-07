using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class SkillComponent
{
    public int IntentSkillSlot;
    public Vector2 IntentTargetPosition;

    public int SkillSlotCount { get; private set; }
    private StatsData _statsData;
    private Dictionary<int, ISkillRuntime> _skillPool;
    private AnimationComponent _animationComponent;
    private SpawnerComponent _spawner;
    private Transform _transform;

    //技能暫存狀態
    private int _pendingSlotIndex=-1;
    private Transform _pendingTransform;
    private Vector3 _pendingPosition;

    public SkillSlot[] SkillSlots;

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


    public bool CanUseSkill(int index) {
        var slot = SkillSlots[index];
        return slot.Detector.HasTarget &&slot.IsReady&& !_animationComponent.IsPlayingAttackAnimation;
    }

    public void PlaySkillAnimation(int slotIndex) {
        var slot = SkillSlots[slotIndex];
        if (!_skillPool.TryGetValue(slot.SkillId, out var skill)) return;
        _pendingSlotIndex = slotIndex;
        _pendingTransform = slot.Detector.TargetTransform;
        _pendingPosition = _pendingTransform.position;

        _animationComponent.PlayAttackAnimation(skill, _pendingPosition);
    }
    public void UseSkill() {
        if (_pendingSlotIndex == -1) return;
        var slot = SkillSlots[_pendingSlotIndex];
        if (!_skillPool.TryGetValue(slot.SkillId, out var skill)) return;

        //數值計算
        int finalAttackPower = _statsData.Power + skill.StatsData.Power;
        Vector2 direction = (_pendingPosition - _transform.position).normalized;

        //呼叫Spawner
        GameObject obj = _spawner.Spawn(skill.VisualData.Prefab, _transform.position, Quaternion.identity);
        if (obj != null)
        {
            var skillObj = obj.GetComponent<SkillObject>();
            skillObj.SetSkillProperties( finalAttackPower, _statsData.KnockbackPower + skill.StatsData.KnockbackPower, _pendingTransform,_pendingPosition);
            slot.TriggerCooldown(skill.Cooldown);

            // 發事件
            OnSkillUsed?.Invoke(_pendingSlotIndex, skill);
        }
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


    public void UpdateSkillSlotsCooldownTimer() {
        foreach (var slot in SkillSlots)
            slot?.TickCooldown(Time.deltaTime);
    }
}