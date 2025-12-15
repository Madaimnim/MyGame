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
    public EnemyBehaviourTreeType EnemyBehaviourTreeType;
    public MoveStrategyBase MoveStrategy;   //直接持有策略實例
    //Runtime-------------------------------------------------------------------------------------------------------
    public int CurrentHp { get; set; }

    public GameObject UiObject;

    //建構子---------------------------------------------------------------------------------------------------------
    public EnemyStatsRuntime(EnemyStatsTemplate template) {
        StatsData = new StatsData(template.StatsData);
        VisualData = new VisualData(template.VisualData);
        MaxHp = template.MaxHp;
        SkillSlotCount = template.SkillSlotCount;
        CanRespawn = template.CanRespawn;
        Exp = template.Exp;
        foreach (var skill in template.SkillTemplateList)
            SkillPool[skill.StatsData.Id] = new EnemySkillRuntime(skill);
        EnemyBehaviourTreeType = template.EnemyBehaviourTreeType;
        MoveStrategy = template.MoveStrategyType switch {
            MoveStrategyType.Follow => new FollowMoveStrategy(),
            MoveStrategyType.Random => new RandomMoveStrategy(),
            MoveStrategyType.Straight => new StraightMoveStrategy(),
            MoveStrategyType.Stay => new StayMoveStrategy(),
            MoveStrategyType.Flee => new FleeMoveStrategy(),
            _ => new StayMoveStrategy()
        };
    }
}
