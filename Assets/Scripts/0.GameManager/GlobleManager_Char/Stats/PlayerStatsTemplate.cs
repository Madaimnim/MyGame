using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PlayerStatsTemplate",menuName="GameData/PlayerStatsTemplate")]
public class PlayerStatsTemplate:ScriptableObject
{
    public StatsData StatsData;
    public VisualData VisualData;
    public int BeginLevel = 1;
    public List<SkillTemplate> SkillPoolList = new List<SkillTemplate>();
    public List<int> UnlockedSkillIdList = new List<int>();


    // ExpTable[Level i] = 從 Level i 升到 Level i+1 所需經驗值
    public int[] ExpTable = new int[] {
        4, 6, 9,13, 18, 25, 34, 45, 58, 73,                     // 1~10
        90, 109, 130, 153, 178, 205, 234, 265, 298, 333,        // 11~20
        370, 409, 450, 493, 538, 585, 634, 685, 738, 793,       // 21~30
    };
}
