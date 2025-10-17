using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerStatsRuntime : IHealthData
{
    // Template Data---------------------------------------------------------------------------------------------
    public StatsData StatsData;
    public VisualData VisualData;
    public int MaxHp { get; }
    public int SkillSlotCount;
    public bool CanRespawn ;
    public Dictionary<int, ISkillRuntime> SkillPool = new Dictionary<int, ISkillRuntime>();
    public List<int> UnlockedSkillIdList= new List<int>();
    public int[] ExpTable;
    public MoveStrategyType MoveStrategyType;
    //Runtime-------------------------------------------------------------------------------------------------------
    public int CurrentHp { get; set; }
    public int Exp ;

    public GameObject BattleObject;
    public GameObject UiObject;
    //«Øºc¤l---------------------------------------------------------------------------------------------------------
    public PlayerStatsRuntime(PlayerStatsTemplate template) {
        StatsData = new StatsData(template.StatsData);
        VisualData = new VisualData(template.VisualData);
        MaxHp = template.MaxHp;
        SkillSlotCount = template.SkillSlotCount;
        CanRespawn = template.CanRespawn;
        foreach (var skill in template.SkillTemplateList)
            SkillPool[skill.StatsData.Id] = new PlayerSkillRuntime(skill);
        UnlockedSkillIdList = new List<int>(template.UnlockedSkillIdList);
        ExpTable = template.ExpTable;
        MoveStrategyType=template.MoveStrategyType;
}
}
