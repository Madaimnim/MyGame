using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStatsRuntime:IHealthData
{

    public StatsData StatsData;
    public VisualData VisualData;
    public int MaxHp { get;  }
    public int CurrentHp { get; set; }

    public SkillSlotRuntime[] SkillSlots;
    public int Exp { get; private set; }
    public MoveStrategyType MoveStrategyType { get; private set; }
    public Dictionary<int, EnemySkillRuntime> SkillPoolDtny { get; private set; }
    public IDamageable Owner;


    public EnemyStatsRuntime(EnemyStatsTemplate template) {
        StatsData = new StatsData(template.StatsData);
        VisualData = new VisualData(template.VisualData);
        Exp = template.Exp;

        CurrentHp = MaxHp;

        SkillPoolDtny = new Dictionary<int, EnemySkillRuntime>();
        foreach (var skill in template.SkillPoolList)
        {
            SkillPoolDtny[skill.SkillId] = new EnemySkillRuntime(skill);
        }
        InitializeSkillSlots(StatsData.SkillSlotCount);
    }

    public void InitializeSkillSlots(int slotCount) {
        SkillSlots = new SkillSlotRuntime[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            SkillSlots[i] = new SkillSlotRuntime(i);
        }
    }

    public void InitializeOwner(IDamageable owner) {
        Owner = owner;
    }

    public virtual void TakeDamage(int dmg) {
        CurrentHp -= dmg; // 自動觸發事件
    }

    public EnemySkillRuntime GetSkill(int slotId) {
        if (SkillPoolDtny.TryGetValue(slotId, out var skill))
        {
            return skill;
        }
        Debug.LogError($"Skill {slotId} 不存在於敵人 {StatsData.Name}");
        return null;
    }
}
