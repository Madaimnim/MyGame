using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum MoveStrategyType {
    Straight,
    Random,
    Follow,
    Stay,
    Flee
}

public enum PlayerBehaviourTreeType {
    NearTargetAttackFirst,
    DefensiveAttack
}

[CreateAssetMenu(fileName ="PlayerStatsTemplate",menuName="GameData/PlayerStatsTemplate")]
public class PlayerStatsTemplate:ScriptableObject
{
    public StatsData StatsData;
    public VisualData VisualData;
    public int MaxHp;
    public int SkillSlotCount;
    public bool CanRespawn=true;
    public PlayerBehaviourTreeType PlayerBehaviourTreeType;
    public MoveStrategyType MoveStrategyType;
    [FormerlySerializedAs("SkillTemplateList")]
    public List<SkillTemplate> SkillTemplateList = new List<SkillTemplate>();
    public List<int> UnlockedSkillIdList = new List<int>();

    public int[] ExpTable = new int[] {
        4, 6, 9,13, 18, 25, 34, 45, 58, 73,                     // 1~10
        90, 109, 130, 153, 178, 205, 234, 265, 298, 333,        // 11~20
        370, 409, 450, 493, 538, 585, 634, 685, 738, 793,       // 21~30
    };
}
