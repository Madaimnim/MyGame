using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerStatData", menuName = "GameData/PlayerStatData")]
public class PlayerStatData : ScriptableObject
{
    [Header("所有玩家數據列表")]
    public List<PlayerStatsTemplate> playerStatsTemplateList = new List<PlayerStatsTemplate>();
}