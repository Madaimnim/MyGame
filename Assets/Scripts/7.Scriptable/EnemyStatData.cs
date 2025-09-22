using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyStatData", menuName = "GameData/EnemyStatData")]
public class EnemyStatData : ScriptableObject
{
    [Header("所有敵人數據列表")]
    public List<EnemyStatsTemplate> enemyStatsTemplateList = new List<EnemyStatsTemplate>();
}