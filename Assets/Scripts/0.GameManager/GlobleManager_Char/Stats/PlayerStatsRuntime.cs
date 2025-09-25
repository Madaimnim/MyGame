using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStatsRuntime : IHealthData,IExpData
{
    // Template Data
    public StatsData StatsData;
    public VisualData VisualData;
    public Dictionary<int, PlayerSkillRuntime> SkillPoolDtny = new Dictionary<int, PlayerSkillRuntime>();
    public List<int> UnlockedSkillIdList = new List<int>();
    private int[] _expTable;
    private int _beginLevle;


    //IHealthData
    public int CurrentHp { get; set; }    
    public int MaxHp => StatsData.MaxHp;

    //IExpData
    public int CurrentLevel { get; set; }
    public int CurrentExp { get; set; }
    public int[] ExpTable => _expTable;

    public List<int> EquippedSkillIdList = new List<int>();
    public GameObject BattlePlayerObject;
    public GameObject UiPlayerObject;

    public SkillSlotRuntime[] SkillSlots;

    public IDamageable Owner;

    public PlayerStatsRuntime(PlayerStatsTemplate template) {
        // Template
        StatsData = new StatsData(template.StatsData);
        VisualData = new VisualData(template.VisualData);
        foreach (var skill in template.SkillPoolList)
            SkillPoolDtny[skill.SkillId] = new PlayerSkillRuntime(skill);
        UnlockedSkillIdList = new List<int>(template.UnlockedSkillIdList);
        _expTable = template.ExpTable;
        _beginLevle = template.BeginLevel;
       // ----------------Runtime------------------------
       //IHealthData
       CurrentHp = StatsData.MaxHp;

        //IExpData
        CurrentExp = 0;
        CurrentLevel = _beginLevle;



        EquippedSkillIdList = new List<int>();
        for (int i = 0; i < StatsData.SkillSlotCount; i++)
            EquippedSkillIdList.Add(-1);

        InitializeSkillSlots(StatsData.SkillSlotCount);
    }

    public void InitializeSkillSlots(int slotCount) {
        SkillSlots = new SkillSlotRuntime[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            SkillSlots[i] = new SkillSlotRuntime(i);
        }
    }


    public PlayerSkillRuntime GetSkillDataRuntimeAtSlot(int slotIndex) {
        if (slotIndex < 0 || slotIndex >= EquippedSkillIdList.Count) return null;
        int skillId = EquippedSkillIdList[slotIndex];
        return skillId != -1 ? GetSkillDataRuntimeForId(skillId) : null;
    }

    public PlayerSkillRuntime GetSkillDataRuntimeForId(int skillId) {
        return SkillPoolDtny.TryGetValue(skillId, out var skill) ? skill : null;
    }

    public void InitializeOwner(IDamageable owner) {
        Owner = owner;
    }


    public void OnSkillUsed(int slotIndex, Transform ownerTransform) {
        var playerSkillRuntime = GetSkillDataRuntimeAtSlot(slotIndex);
        if (playerSkillRuntime == null) return;

        bool leveledUp = playerSkillRuntime.AddSkillUsageCount();
        if (leveledUp)
        {
            GameEventSystem.Instance.Event_SkillLevelUp?.Invoke(playerSkillRuntime, ownerTransform);
            GameEventSystem.Instance.Event_SkillInfoChanged?.Invoke(slotIndex, ownerTransform.GetComponent<Player>());
        }
    }

    public void UnlockSkill(int skillId) {
        if (!SkillPoolDtny.ContainsKey(skillId))
        {
            Debug.LogWarning($"SkillPoolDtny不包含 ID:{skillId}");
            return;
        }
        if (UnlockedSkillIdList.Contains(skillId))
        {
            Debug.LogWarning($"UnlockedSkillIdList已解鎖 ID:{skillId} {SkillPoolDtny[skillId].SkillName}");
            return;
        }

        UnlockedSkillIdList.Add(skillId);
    }

    public void SetSkillAtSlot(int slotIndex, int skillId) {
        if (slotIndex < 0 || slotIndex >= StatsData.SkillSlotCount)
        {
            Debug.LogWarning($"slotIndex {slotIndex} 超出技能槽索引");
            return;
        }
        if (!SkillPoolDtny.ContainsKey(skillId))
        {
            Debug.LogWarning($"角色 {StatsData.Id} 試圖裝備 SkillPoolDtny 沒有的技能 ID: {skillId}");
            return;
        }
        if (!UnlockedSkillIdList.Contains(skillId))
        {
            Debug.LogWarning($"角色 {StatsData.Id} 試圖裝備未解鎖的技能 ID: {skillId}");
            return;
        }
        if (!SkillPoolDtny.TryGetValue(skillId, out PlayerSkillRuntime skillData))
        {
            Debug.LogError($"[SetSkillAtSlot] 無法取得技能 ID: {skillId}");
            return;
        }
        if (slotIndex >= EquippedSkillIdList.Count)
        {
            Debug.LogError($"[SetSkillAtSlot] 技能槽 {slotIndex} 超出 EquippedSkillIdList 範圍");
            return;
        }

        EquippedSkillIdList[slotIndex] = skillId;

        if (BattlePlayerObject == null)
        {
            Debug.LogError("BattlePlayerObject 為 null");
        }
        else
        {
            BattlePlayerObject.GetComponent<Player>()?.SetSkillSlot(slotIndex, skillData);
        }

        GameObject previewPlayerObject = UIManager.Instance.activeUIPlayersDtny.ContainsKey(StatsData.Id)
            ? UIManager.Instance.activeUIPlayersDtny[StatsData.Id]
            : null;

        if (previewPlayerObject != null)
        {
            previewPlayerObject.GetComponent<Player>()?.SetSkillSlot(slotIndex, skillData);
        }
        else
        {
            Debug.LogError($"[SetSkillAtSlot] 未找到 UI 預覽角色 {StatsData.Id}");
        }
    }
}
