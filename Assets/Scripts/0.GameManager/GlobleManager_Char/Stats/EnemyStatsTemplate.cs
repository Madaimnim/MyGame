using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStatsTemplate : CharStats<EnemySkillRuntime>
{
    // 敵人共用屬性
    public int Exp { get; protected set; }
    public float KnockbackResistance { get; protected set; }

    public MoveStrategyType moveStrategyType;
    public List<SkillTemplate> skillPoolList = new List<SkillTemplate>();

}
