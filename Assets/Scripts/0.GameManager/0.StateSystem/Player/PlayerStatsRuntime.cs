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

    public Dictionary<int, PlayerSkillRuntime> PlayerSkillPool = new Dictionary<int, PlayerSkillRuntime>();
    public List<int> UnlockedSkillIdList= new List<int>();

    public int[] ExpTable;
    //Runtime-------------------------------------------------------------------------------------------------------
    public int CurrentHp { get; set; }
    public int Exp ;

    public SkillSlot[] SkillSlots;
    public SkillSystem SkillSystem;

    public GameObject BattlePlayerObject;
    public GameObject UiPlayerObject;
    
    public IDamageable Owner;
    //建構子---------------------------------------------------------------------------------------------------------
    public PlayerStatsRuntime(PlayerStatsTemplate template) {
        StatsData = new StatsData(template.StatsData);
        VisualData = new VisualData(template.VisualData);

        MaxHp = template.MaxHp;
        SkillSlotCount = template.SkillSlotCount;

        foreach (var skill in template.SkillTemplateList)
            PlayerSkillPool[skill.StatsData.Id] = new PlayerSkillRuntime(skill);
        UnlockedSkillIdList = new List<int>(template.UnlockedSkillIdList);

        ExpTable = template.ExpTable;

    }
    //方法---------------------------------------------------------------------------------------------------------
    public void Initialize(SkillSystem skillSystem) {
        SkillSlots = new SkillSlot[SkillSlotCount];
        for (int i = 0; i < SkillSlotCount; i++)
            SkillSlots[i] = new SkillSlot();

        SkillSystem = skillSystem;
    } 
    public void InitializeOwner(IDamageable owner) {
        Owner = owner;
    }

    public void SetBattleObject(GameObject ob) {
        BattlePlayerObject = ob;
    }
    public bool IsInEquippedList(int skillId) => SkillSlots.Any(s => s.SkillId == skillId);
    
    //Skill
    public PlayerSkillRuntime GetSkillAtSlot(int index) {
        int skillId = SkillSlots[index].SkillId;
        if (PlayerSkillPool.TryGetValue(skillId, out var skill)) return skill;
        else return null;
    }
    public PlayerSkillRuntime GetSkillRuntime(int skillId) {
        return PlayerSkillPool.TryGetValue(skillId, out var skill) ? skill : null;
    }

}
