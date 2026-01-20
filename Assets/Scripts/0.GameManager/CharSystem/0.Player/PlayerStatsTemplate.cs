using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum MoveStrategyType {
    [InspectorName("直線")] Straight,
    [InspectorName("隨機")] Random,
    [InspectorName("站立")] Stay,
}
public enum PlayerBehaviourTreeType {
    [InspectorName("移動攻擊模式")] MoveAttackType,
    [InspectorName("防禦優先Todo")] DefensiveType //todo:未實裝
}

[CreateAssetMenu(fileName ="PlayerStatsTemplate",menuName="GameData/PlayerStatsTemplate")]
public class PlayerStatsTemplate:ScriptableObject
{
    public int Id;
    public string Name;
    public int Level;
    public StatsData StatsData;
    public VisualData VisualData;
    public PlayerBehaviourTreeType PlayerBehaviourTreeType;
    public MoveStrategyType MoveStrategyType;
    public int SkillSlotCount;
    [FormerlySerializedAs("SkillTemplateList")]
    public SkillTemplate BaseAttackTemplate;
    public List<SkillTemplate> SkillTemplateList = new List<SkillTemplate>();
    public List<SkillTemplate> EnhancedSkillTemplateList = new List<SkillTemplate>();
    public HashSet<int> UnlockedSkillIdHashSet = new();

    public bool CanRespawn = true;
    public int MaxHp;
    public int[] ExpTable = new int[] {0,
        4, 6, 9,13, 18, 25, 34, 45, 58, 73,                     // 1~10
        90, 109, 130, 153, 178, 205, 234, 265, 298, 333,        // 11~20
        370, 409, 450, 493, 538, 585, 634, 685, 738, 793,       // 21~30
    };
}
