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
    public int Exp;
    public bool CanRespawn;
    public Dictionary<int, ISkillRuntime> SkillPool = new Dictionary<int, ISkillRuntime>();
    public MoveStrategyType MoveStrategyType;
    //Runtime-------------------------------------------------------------------------------------------------------
    public int CurrentHp { get; set; }

    public GameObject UiObject;

    //«Øºc¤l---------------------------------------------------------------------------------------------------------
    public EnemyStatsRuntime(EnemyStatsTemplate template) {
        StatsData = new StatsData(template.StatsData);
        VisualData = new VisualData(template.VisualData);
        MaxHp = template.MaxHp;
        SkillSlotCount = template.SkillSlotCount;
        CanRespawn = template.CanRespawn;
        Exp = template.Exp;
        foreach (var skill in template.SkillTemplateList)
            SkillPool[skill.StatsData.Id] = new EnemySkillRuntime(skill);
        MoveStrategyType = template.MoveStrategyType;
    }
}
