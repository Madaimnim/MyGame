using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerStatsTemplate : CharStats<PlayerSkillRuntime>
{
    // ���a�@�Ϊ��ݩ�
    public List<int> UnlockedSkillIDList = new List<int>();

    public static readonly int[] ExpTable = new int[]{
        4,6,9,
        13, 18, 25, 34, 45, 58, 73, 90, 109, 130, // 1~10
        153, 178, 205, 234, 265, 298, 333, 370, 409, 450, // 11~20
        493, 538, 585, 634, 685, 738, 793, 850, 909, 970  // 21~30
        };
    [Header("���a�ޯ��")]
    public List<SkillTemplate> skillPoolList = new List<SkillTemplate>();
}
