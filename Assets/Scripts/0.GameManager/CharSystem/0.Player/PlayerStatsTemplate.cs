using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum MoveStrategyType {
    [InspectorName("直線")] Straight,
    [InspectorName("隨機")] Random,
    [InspectorName("追蹤")] Follow,
    [InspectorName("站立")] Stay,
    [InspectorName("逃跑")] Flee//todo:未實裝
}
public enum PlayerBehaviourTreeType {
    [InspectorName("最近敵人優先")] NearTargetAttackFirst,
    [InspectorName("防禦優先")] DefensiveAttack //todo:未實裝
}

[CreateAssetMenu(fileName ="PlayerStatsTemplate",menuName="GameData/PlayerStatsTemplate")]
public class PlayerStatsTemplate:ScriptableObject
{
    public PlayerBehaviourTreeType PlayerBehaviourTreeType;
    public MoveStrategyType MoveStrategyType;
    public int SkillSlotCount;
    [FormerlySerializedAs("SkillTemplateList")]
    public List<SkillTemplate> SkillTemplateList = new List<SkillTemplate>();
    public List<int> UnlockedSkillIdList = new List<int>();
    public StatsData StatsData;
    public VisualData VisualData;
    public bool CanRespawn = true;
    public int MaxHp;
    public int[] ExpTable = new int[] {
        4, 6, 9,13, 18, 25, 34, 45, 58, 73,                     // 1~10
        90, 109, 130, 153, 178, 205, 234, 265, 298, 333,        // 11~20
        370, 409, 450, 493, 538, 585, 634, 685, 738, 793,       // 21~30
    };
}
