using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerStatsRuntime : IHealthData,IExpData
{

    public IReadOnlyDictionary<int, PlayerSkillRuntime> PlayerSkillRuntimeDtny = new Dictionary<int, PlayerSkillRuntime>();
    public IReadOnlyCollection<int> UnlockedSkillIdList => _unlockedSkillIdList;
    public bool HasEquippedSkill(int skillId) => _equippedSkillSlots.Any(s => s.SkillId == skillId);
    public EquippedSkillSlot[] EquippedSkillSlots => _equippedSkillSlots;
    public StatsData StatsData => _statsData;
    public VisualData VisualData => _visualData;
    //Runtime Reference
    public GameObject BattlePlayerObject;
    public GameObject UiPlayerObject;
    public IDamageable Owner;


    // Template Data
    private StatsData _statsData;
    private VisualData _visualData;
    private Dictionary<int, PlayerSkillRuntime> _playerSkillRuntimeDtny = new Dictionary<int, PlayerSkillRuntime>();
    private List<int> _unlockedSkillIdList = new List<int>();
    private int _beginLevle;
    private int[] _expTable;
    //IHealthData
    public int CurrentHp { get; set; }    
    public int MaxHp => StatsData.MaxHp;
    //IExpData
    public int CurrentLevel { get; set; }
    public int CurrentExp { get; set; }
    public int[] ExpTable => _expTable;

    //SkillRelated
    private EquippedSkillSlot[] _equippedSkillSlots;
    public SkillSystem SkillSystem=>_skillSystem;

    private SkillSystem _skillSystem;




    public PlayerStatsRuntime(PlayerStatsTemplate template) {
        // Template
        _statsData = new StatsData(template.StatsData);
        _visualData = new VisualData(template.VisualData);
        foreach (var skill in template.SkillTemplateList)
            _playerSkillRuntimeDtny[skill.SkillId] = new PlayerSkillRuntime(skill);
        _unlockedSkillIdList = new List<int>(template.UnlockedSkillIdList);
        _expTable = template.ExpTable;
        _beginLevle = template.BeginLevel;
    }

    public void Initialize(SkillSystem skillSystem) {
        CurrentHp = StatsData.MaxHp;
        CurrentExp = 0;
        CurrentLevel = _beginLevle;

        _equippedSkillSlots = new EquippedSkillSlot[StatsData.SkillSlotCount];
        for (int i = 0; i < StatsData.SkillSlotCount; i++)
            _equippedSkillSlots[i] = new EquippedSkillSlot();

        _skillSystem = skillSystem;
    }
  
    public void InitializeOwner(IDamageable owner) {
        Owner = owner;
    }

    public PlayerSkillRuntime GetSkillRuntimeAtSlot(int slotIndex) {
        if (slotIndex < 0 || slotIndex >= StatsData.SkillSlotCount) return null;
        int skillId = _equippedSkillSlots[slotIndex].SkillId;
        return skillId != -1 ? GetSkillRuntime(skillId) : null;
    }
    public PlayerSkillRuntime GetSkillRuntime(int skillId) {
        return _playerSkillRuntimeDtny.TryGetValue(skillId, out var skill) ? skill : null;
    }


    public void OnSkillUsed(int slotIndex, Transform ownerTransform) {
        var playerSkillRuntime = GetSkillRuntimeAtSlot(slotIndex);
        if (playerSkillRuntime == null) return;

        bool leveledUp = playerSkillRuntime.AddSkillUsageCount();
        if (leveledUp)
        {
            GameEventSystem.Instance.Event_SkillLevelUp?.Invoke(playerSkillRuntime, ownerTransform);
            GameEventSystem.Instance.Event_SkillInfoChanged?.Invoke(slotIndex, ownerTransform.GetComponent<Player>());
        }
    }
    public void AddUnlockSkillList(int skillId) => _unlockedSkillIdList.Add(skillId);
    public void SetEquippedSkillIds(int slotIndex, int skillId) {
        _equippedSkillSlots[slotIndex].Equip(skillId);
    }
}
