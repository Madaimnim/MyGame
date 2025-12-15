using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum EnemyBehaviourTreeType {
    RushWall,
    AdvanceAttack,
    BackAwayAttack,
    Boss
}


[CreateAssetMenu(fileName ="EnemyStatsTemplate",menuName= "GameData/EnemyStatsTemplate")]
public class EnemyStatsTemplate:ScriptableObject
{
    public StatsData StatsData;
    public VisualData VisualData;
    public int MaxHp;
    public int SkillSlotCount;
    public int Exp;
    public bool CanRespawn=false;
    [FormerlySerializedAs("SkillTemplateList")]
    public List<SkillTemplate> SkillTemplateList = new List<SkillTemplate>();
    public EnemyBehaviourTreeType EnemyBehaviourTreeType;
    public MoveStrategyType MoveStrategyType;
}
