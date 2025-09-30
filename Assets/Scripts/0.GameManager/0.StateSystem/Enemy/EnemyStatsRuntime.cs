using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStatsRuntime:IHealthData
{
    // Template Data-----------------------------------------
    public StatsData StatsData;
    public VisualData VisualData;
    public int MaxHp { get; }
    public int SkillSlotCount;

    public Dictionary<int, EnemySkillRuntime> EnemySkillPool;

    public int CurrentHp { get; set; }

    public SkillSlot[] SkillSlots;
    public int Exp { get; private set; }

    public MoveStrategyType MoveStrategyType { get; private set; }

    public IDamageable Owner;



    public EnemyStatsRuntime(EnemyStatsTemplate template) {
        StatsData = new StatsData(template.StatsData);
        VisualData = new VisualData(template.VisualData);
        Exp = template.Exp;

        CurrentHp = MaxHp;

        EnemySkillPool = new Dictionary<int, EnemySkillRuntime>();
        foreach (var skill in template.SkillTemplateList)
        {
            EnemySkillPool[skill.StatsData.Id] = new EnemySkillRuntime(skill);
        }
        InitializeSkillSlots(SkillSlotCount);
        SkillSlotCount = template.SkillSlotCount;
    }

    public void InitializeSkillSlots(int slotCount) {
        SkillSlots = new SkillSlot[SkillSlotCount];
        for (int i = 0; i < SkillSlotCount; i++)
            SkillSlots[i] = new SkillSlot();

    }

    public void InitializeOwner(IDamageable owner) {
        Owner = owner;
    }

    public virtual void TakeDamage(int dmg) {
        CurrentHp -= dmg; // 自動觸發事件
    }

    public EnemySkillRuntime GetSkill(int slotId) {
        if (EnemySkillPool.TryGetValue(slotId, out var skill))
        {
            return skill;
        }
        Debug.LogError($"Skill {slotId} 不存在於敵人 {StatsData.Name}");
        return null;
    }
}
