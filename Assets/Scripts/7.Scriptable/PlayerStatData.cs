using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerStatData", menuName = "GameData/PlayerStatData")]
public class PlayerStatData : ScriptableObject
{
    [Header("所有玩家數據列表")]
    [FormerlySerializedAs("playerStatsTemplateList")]
    public List<PlayerStatsTemplate> playerStatsTemplateList = new List<PlayerStatsTemplate>();
}