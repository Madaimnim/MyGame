using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "EnemyStatData", menuName = "GameData/EnemyStatData")]
public class EnemyStatData : ScriptableObject
{
    [Header("所有敵人數據列表")]
    [FormerlySerializedAs("enemyStatsTemplateList")]
    public List<EnemyStatsTemplate> enemyStatsTemplateList = new List<EnemyStatsTemplate>();
}