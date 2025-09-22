using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStatsRuntime :CharStats<EnemySkillRuntime>,IRuntime
{
    //敵人獨有屬性
    // 動態（可選：如果覺得不需要，可以砍掉）
    public int CurrentHp{ get;private set; }

    // 敵人共用屬性
    public int Exp { get; protected set; }
    public float KnockbackResistance { get; protected set; }

    public MoveStrategyType MoveStrategyType;
    public Dictionary<int, EnemySkillRuntime> SkillPoolDtny { get; private set; }

    public EnemyStatsRuntime(EnemyStatsTemplate template) {
        //CharStats
        Id = template.Id;
        Name = template.Name;
        Level = template.Level;
        MaxHp = template.MaxHp;
        AttackPower = template.AttackPower;
        MoveSpeed = template.MoveSpeed;
        CharPrefab = template.CharPrefab;
        SpriteIcon = template.SpriteIcon;
        SkillSlotCount = template.SkillSlotCount;
        //Runtime
        Exp = template.Exp;

        CurrentHp = MaxHp;

        SkillPoolDtny = new Dictionary<int, EnemySkillRuntime>();
        foreach (var skill in template.skillPoolList)
        {
            SkillPoolDtny[skill.SkillId] = new EnemySkillRuntime(skill);
        }
        InitializeSkillSlots(template.SkillSlotCount);
    }


    public virtual void TakeDamage(int dmg) {
        CurrentHp -= dmg; // 自動觸發事件
    }

    public EnemySkillRuntime GetSkill(int slotId) {
        if (SkillPoolDtny.TryGetValue(slotId, out var skill))
        {
            return skill;
        }
        Debug.LogError($"Skill {slotId} 不存在於敵人 {Name}");
        return null;
    }
}
