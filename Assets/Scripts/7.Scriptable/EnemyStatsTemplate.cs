using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="EnemyStatsTemplate",menuName= "GameData/EnemyStatsTemplate")]
public class EnemyStatsTemplate:ScriptableObject
{
    public StatsData StatsData;
    public VisualData VisualData;
    public int Exp;
    public List<SkillTemplate> SkillPoolList = new List<SkillTemplate>();
    public MoveStrategyType MoveStrategyType;


}
