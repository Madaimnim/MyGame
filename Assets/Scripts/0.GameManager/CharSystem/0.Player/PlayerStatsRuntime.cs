using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class PlayerStatsRuntime : IHealthData
{
    // Template Data---------------------------------------------------------------------------------------------
    public int Id;
    public string Name;
    public int Level;
    public StatsData StatsData;
    public VisualData VisualData;
    public int MaxHp { get; }
    public int SkillSlotCount;
    public bool CanRespawn ;
    public ISkillRuntime BaseAttackRuntime;
    public Dictionary<int, ISkillRuntime> SkillPool = new Dictionary<int, ISkillRuntime>();
    public HashSet<int> UnlockedSkillIdHashSet = new();
    public int[] ExpTable;
    public PlayerBehaviourTreeType PlayerBehaviourTreeType;
    public MoveStrategyBase MoveStrategy;   //直接持有策略實例

    //Runtime-------------------------------------------------------------------------------------------------------
    public int CurrentHp { get; set; }
    public int Exp ;

    public GameObject BattleObject;
    public GameObject UiObject;
    //建構子---------------------------------------------------------------------------------------------------------
    public PlayerStatsRuntime(PlayerStatsTemplate template) {
        Id= template.Id;
        Name= template.Name;
        Level= template.Level;
        StatsData = new StatsData(template.StatsData);
        VisualData = new VisualData(template.VisualData);
        MaxHp = template.MaxHp;
        SkillSlotCount = template.SkillSlotCount;
        CanRespawn = template.CanRespawn;
        BaseAttackRuntime = new PlayerSkillRuntime(template.BaseAttackTemplate);
        foreach (var skill in template.SkillTemplateList) {
            SkillPool[skill.Id] = new PlayerSkillRuntime(skill);
        }

        UnlockedSkillIdHashSet = new HashSet<int>(template.UnlockedSkillIdHashSet);
        ExpTable = template.ExpTable;
        PlayerBehaviourTreeType= template.PlayerBehaviourTreeType;
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
