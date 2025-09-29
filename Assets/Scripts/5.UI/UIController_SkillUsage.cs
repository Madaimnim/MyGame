using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIController_SkillUsage : UI_PanelBase
{
    [System.Serializable] // 讓 Unity Inspector 可以顯示
    public class SkillInfo
    {
        [Header("狀態欄")]
        public int skillID;

        [Header("對應 UI 元件")]
        public TMP_Text levelText;          
        public TMP_Text nextLevelText;
        public TMP_Text attackText;
        public TMP_Text nextAttackText;
        public TMP_Text usageText;
        public GameObject expBar;
    }

    //變數
    #region 變數
    [Header("UI顯示的技能熟練度面板")]
    public List<SkillInfo> skillsInfoList = new List<SkillInfo>();

    [Header("要顯示哪個角色的技能 (從 PlayerStateManager 抓資料)")]
    private int targetPlayerID;


    #endregion

    #region 生命週期

    private void OnEnable() {
        base.OnEnable();
        GameManager.Instance.OnAllDataLoaded += OnAllDataLoaded;
    }
    #endregion
    private void OnDisable() {
        GameManager.Instance.OnAllDataLoaded -= OnAllDataLoaded;
    }

    //更新UI技能熟練度介面
    public void UpdateSkillUsageMastery() {
        //if (!GameManager.Instance.PlayerStateSystem.TryGetStatsRuntime(targetPlayerID, out var playerStatsRuntime)) return;
        //if (UIManager.Instance == null || GameManager.Instance.PlayerStateSystem == null) return;
        //
        //foreach (var skillInfo in skillsInfoList)
        //{
        //    var playerSkillDataRuntime = playerStatsRuntime.GetSkillRuntime(skillInfo.skillID);
        //    if (playerSkillDataRuntime == null)
        //    {
        //        Debug.LogWarning($"[UIController_SkillUsage] 技能 {skillInfo.skillID} 不存在於角色 {playerStatsRuntime.StatsData.Name} 的技能池");
        //        continue;
        //    }
        //
        //    // 技能等級
        //    if (skillInfo.levelText != null) skillInfo.levelText.text = $"Lv.{playerSkillDataRuntime.SkillLevel}";
        //    if (skillInfo.nextLevelText != null) skillInfo.nextLevelText.text = $"Lv.{playerSkillDataRuntime.SkillLevel + 1}";
        //
        //    // 攻擊力
        //    if (skillInfo.attackText != null) skillInfo.attackText.text = $"ATK:{playerSkillDataRuntime.SkillPower}";
        //    if (skillInfo.nextAttackText != null) skillInfo.nextAttackText.text = $"ATK:{playerSkillDataRuntime.SkillPower + 1}";
        //
        //    // 使用次數（需求數先用 nextSkillLevelCount）
        //    if (skillInfo.usageText != null)
        //        skillInfo.usageText.text = $"使用次數:{playerSkillDataRuntime.SkillUsageCount}/{playerSkillDataRuntime.NextSkillLevelCount}";
        //    
        //    //expBar 伸縮
        //    if (skillInfo.expBar != null)
        //    {
        //        float progress = 0f;
        //
        //        if (playerSkillDataRuntime.NextSkillLevelCount > 0) // 避免除以 0
        //            progress = Mathf.Clamp01((float)playerSkillDataRuntime.SkillUsageCount / playerSkillDataRuntime.NextSkillLevelCount);
        //
        //        Vector3 scale = skillInfo.expBar.transform.localScale;
        //        scale.x = progress; //  根據進度改 X 軸
        //        skillInfo.expBar.transform.localScale = scale;
        //    }
        //}
    }

    //事件方法
    public void OnAllDataLoaded() {
        UpdateSkillUsageMastery();
    }

    public override void Refresh() {
      
    }
}
