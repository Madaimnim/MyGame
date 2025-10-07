using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
    public MoveStrategyType MoveStrategyType;

}
