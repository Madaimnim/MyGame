using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerStatsRuntime : IHealthData,IExpData
{
    // Template Data
    public StatsData StatsData;
    public VisualData VisualData;
    private readonly Dictionary<int, PlayerSkillRuntime> _playerSkillRuntimeDtny = new Dictionary<int, PlayerSkillRuntime>();
    private readonly List<int> _unlockedSkillIdList = new List<int>();
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
    public IReadOnlyCollection<int> UnlockedSkillIdList => _unlockedSkillIdList;
    public IReadOnlyDictionary<int, PlayerSkillRuntime> PlayerSkillRuntimeDtny => _playerSkillRuntimeDtny;
    public IReadOnlyList<int> EquippedSkillIdList => _equippedSkillIds;
    public IReadOnlyList<SkillSlotRuntime> SkillSlots => _skillSlots;
    public bool HasEquippedSkill(int skillId) => _equippedSkillIds.Contains(skillId);
    public SkillSystem SkillSystem=>_skillSystem;

    private SkillSystem _skillSystem;
    private int[] _equippedSkillIds;
    private SkillSlotRuntime[] _skillSlots;

    //Runtime Reference
    public GameObject BattlePlayerObject;
    public GameObject UiPlayerObject;
    public IDamageable Owner;

    public PlayerStatsRuntime(PlayerStatsTemplate template) {
        // Template
        StatsData = new StatsData(template.StatsData);
        VisualData = new VisualData(template.VisualData);
        foreach (var skill in template.SkillPoolList)
            _playerSkillRuntimeDtny[skill.SkillId] = new PlayerSkillRuntime(skill);
        _unlockedSkillIdList = new List<int>(template.UnlockedSkillIdList);
        _expTable = template.ExpTable;
        _beginLevle = template.BeginLevel;
    }

    public void Initialize(SkillSystem skillSystem) {
        CurrentHp = StatsData.MaxHp;
        CurrentExp = 0;
        CurrentLevel = _beginLevle;

        _equippedSkillIds = new int[StatsData.SkillSlotCount];
        for (int i = 0; i < StatsData.SkillSlotCount; i++)
            _equippedSkillIds[i]=-1;

        _skillSlots = new SkillSlotRuntime[StatsData.SkillSlotCount];
        for (int i = 0; i < StatsData.SkillSlotCount; i++)
            _skillSlots[i] = new SkillSlotRuntime(i);

        _skillSystem = skillSystem;
    }
  
    public void InitializeOwner(IDamageable owner) {
        Owner = owner;
    }

    public PlayerSkillRuntime GetSkillRuntimeAtSlot(int slotIndex) {
        if (slotIndex < 0 || slotIndex >= StatsData.SkillSlotCount) return null;
        int skillId = _equippedSkillIds[slotIndex];
        return skillId != -1 ? GetSkillDataRuntimeForId(skillId) : null;
    }
    public PlayerSkillRuntime GetSkillDataRuntimeForId(int skillId) {
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
    public void SetEquippedSkillIds(int slotIndex, int skillId) => _equippedSkillIds[slotIndex] = skillId;
}
