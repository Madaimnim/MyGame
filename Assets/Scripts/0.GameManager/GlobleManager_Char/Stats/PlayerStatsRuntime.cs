using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerStatsRuntime : CharStats<PlayerSkillRuntime>, IRuntime
{
    public int CurrentHp
    {
        get => _currentHp;
        protected set
        {
            int newValue = Mathf.Clamp(value, 0, MaxHp);
            if (_currentHp == newValue) return;

            Debug.Log($"[HP變更] {Name} 從 {_currentHp} → {newValue} / {MaxHp}");
            _currentHp = newValue;

            if (EventManager.Instance != null)
                EventManager.Instance.Event_HpChanged?.Invoke(_currentHp, MaxHp, owner);
            else
                Debug.LogError(" EventManager.Instance 為 null，無法觸發血量事件");
        }
    }
    private int _currentHp;
    public int CurrentExp { get; private set; }
    public GameObject BattlePlayerObject { get; set; }
    public GameObject UiPlayerObject { get; set; }
    public Dictionary<int, PlayerSkillRuntime> SkillPoolDtny { get; private set; } = new Dictionary<int, PlayerSkillRuntime>();
    public List<int> UnlockedSkillIDList { get; private set; } = new List<int>();
    public List<int> EquippedSkillIDList { get; private set; } = new List<int>();


    public static readonly int[] ExpTable = new int[]{
        4,6,9,
        13, 18, 25, 34, 45, 58, 73, 90, 109, 130, // 1~10
        153, 178, 205, 234, 265, 298, 333, 370, 409, 450, // 11~20
        493, 538, 585, 634, 685, 738, 793, 850, 909, 970  // 21~30
        };

    public virtual void TakeDamage(int dmg) {
        CurrentHp -= dmg; // 自動觸發事件
    }
    public virtual void Heal(int amount) {
        CurrentHp += amount; // 自動觸發事件
    }
    public bool IsDead => _currentHp <= 0;


    public PlayerStatsRuntime(PlayerStatsTemplate template) {
        //CharStats
        Id = template.Id;
        Name = template.Name;
        Level = template.Level;
        MaxHp = template.MaxHp;
        AttackPower = template.AttackPower;
        MoveSpeed = template.MoveSpeed;
        CharPrefab = template.CharPrefab;
        SpriteIcon = template.SpriteIcon;
        SkillSlotCount = template.SkillSlotCount;
        //PlayerRuntime獨有
        CurrentExp = 0;
        //
        foreach (var skill in template.skillPoolList)
            SkillPoolDtny[skill.SkillId] = new PlayerSkillRuntime(skill);

        UnlockedSkillIDList = new List<int>(template.UnlockedSkillIDList);

        EquippedSkillIDList = new List<int>();
        for (int i = 0; i < SkillSlotCount; i++)
            EquippedSkillIDList.Add(-1);

        InitializeSkillSlots(SkillSlotCount);


        InitializeSkillSlots(SkillSlotCount);      //初始化技能槽數量
    }

    public override void InitializeOwner(IDamageable damageable) {
        base.InitializeOwner(damageable);
        CurrentHp = MaxHp;
    }

    public void RecoverHp() {
        CurrentHp = MaxHp;
    }

    public PlayerSkillRuntime GetSkillDataRuntimeAtSlot(int slotIndex) {
        if (slotIndex < 0 || slotIndex >= EquippedSkillIDList.Count) return null;
        int skillID = EquippedSkillIDList[slotIndex];
        return skillID != -1 ? GetSkillDataRuntimeForId(skillID) : null;
    }           // API：用 slot index 取技能 
    public PlayerSkillRuntime GetSkillDataRuntimeForId(int skillID) {
        return SkillPoolDtny.TryGetValue(skillID, out var skill) ? skill : null;
    }                   // API：用 id 取技能
    public void GainExp(int exp) {
        CurrentExp += exp;

        while (CurrentExp >= GetExpToNextLevel() && Level < ExpTable.Length)
        {
            CurrentExp -= GetExpToNextLevel();
            Level++;
            LevelUp();
        }
    }                                           // API：獲得經驗值    
    public int GetExpToNextLevel() {
        if (Level >= ExpTable.Length)
            return int.MaxValue;

        return ExpTable[Level - 1];
    }                                         //取得下一級所需經驗
    private void LevelUp() {
        TextPopupManager.Instance.ShowLevelUpPopup(Level, BattlePlayerObject.transform);

    }
    public void OnSkillUsed(int slotIndex, Transform ownerTransform) {
        var playerSkillRuntime = GetSkillDataRuntimeAtSlot(slotIndex);
        if (playerSkillRuntime == null) return;

        bool leveledUp = playerSkillRuntime.AddSkillUsageCount();
        if (leveledUp)
        {
            EventManager.Instance.Event_SkillLevelUp?.Invoke(playerSkillRuntime, ownerTransform);
            EventManager.Instance.Event_SkillInfoChanged?.Invoke(slotIndex, ownerTransform.GetComponent<Player>());
        }
    }
    public void UnlockSkill(int skillID) {
        if (!SkillPoolDtny.ContainsKey(skillID)) { Debug.LogWarning($"SkillPoolDtny不包含 ID:{skillID}"); ; return; }
        if (UnlockedSkillIDList.Contains(skillID)) { Debug.LogWarning($"UnlockedSkillIDList已解鎖 ID:{skillID}{SkillPoolDtny[skillID].SkillName}"); ; return; }

        UnlockedSkillIDList.Add(skillID);
    }                                   // API：解鎖技能
    public void SetSkillAtSlot(int slotIndex, int skillID) {
        if (slotIndex < 0 || slotIndex >= SkillSlotCount) { Debug.LogWarning($"slotIndex{slotIndex}超出技能槽索引"); return; }
        if (!SkillPoolDtny.ContainsKey(skillID)){Debug.LogWarning($"角色{Id}試圖裝備SkillPoolDtny沒有的技能 ID: {skillID}"); return;}
        if (!UnlockedSkillIDList.Contains(skillID)){Debug.LogWarning($"角色{Id}試圖裝備未解鎖的技能 ID: {skillID}"); return;}
        if (!SkillPoolDtny.TryGetValue(skillID, out PlayerSkillRuntime skillData)) {Debug.LogError($"[SetSkillAtSlot] 無法取得技能 ID: {skillID}");return;}
        if (slotIndex >= EquippedSkillIDList.Count){Debug.LogError($"[SetSkillAtSlot] 技能槽 {slotIndex} 超出 EquippedSkillIDList 範圍"); return; }

        EquippedSkillIDList[slotIndex] = skillID;

        GameObject battlePlayerObject = BattlePlayerObject;
        if (battlePlayerObject == null) Debug.LogError($"BattlePlayerObject為null");
        else
            battlePlayerObject.GetComponent<Player>()?.SetSkillSlot(slotIndex, skillData); 
        GameObject previewPlayerObject = UIManager.Instance.activeUIPlayersDtny.ContainsKey(Id)
            ? UIManager.Instance.activeUIPlayersDtny[Id]
            : null;
        if (previewPlayerObject != null)
        {
            previewPlayerObject.GetComponent<Player>()?.SetSkillSlot(slotIndex, skillData);
        }
        else
        {
            Debug.LogError($"[SetSkillAtSlot] 未找到 UI 預覽角色 {Id}");
        }
    }                   // API：設定技能槽

}