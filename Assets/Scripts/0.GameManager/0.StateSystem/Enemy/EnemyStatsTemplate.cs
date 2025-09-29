using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName ="EnemyStatsTemplate",menuName= "GameData/EnemyStatsTemplate")]
public class EnemyStatsTemplate:ScriptableObject
{
    public StatsData StatsData;
    public VisualData VisualData;
    public int Exp;
    [FormerlySerializedAs("SkillTemplateList")]
    public List<SkillTemplate> SkillTemplateList = new List<SkillTemplate>();
    public MoveStrategyType MoveStrategyType;


}
