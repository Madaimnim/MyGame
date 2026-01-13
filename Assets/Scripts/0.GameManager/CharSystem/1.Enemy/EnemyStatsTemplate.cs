using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum EnemyBehaviourTreeType {
    [InspectorName("½ÄÀð")] RushWall,
    [InspectorName("«e¶i§ðÀ»")] AdvanceAttack,
    [InspectorName("«á°h§ðÀ»")] BackAwayAttack,
    [InspectorName("Boss")] Boss
}


[CreateAssetMenu(fileName ="EnemyStatsTemplate",menuName= "GameData/EnemyStatsTemplate")]
public class EnemyStatsTemplate:ScriptableObject
{
    public int Id;
    public string Name;
    public StatsData StatsData;
    public VisualData VisualData;
    public EnemyBehaviourTreeType EnemyBehaviourTreeType;
    public MoveStrategyType MoveStrategyType;
    public int SkillSlotCount;

    [FormerlySerializedAs("SkillTemplateList")]
    public List<SkillTemplate> SkillTemplateList = new List<SkillTemplate>();
    public bool CanRespawn = false;
    public int MaxHp;
    public int Exp;

 
}
