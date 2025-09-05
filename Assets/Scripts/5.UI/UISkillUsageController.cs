using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UISkillUsageController : MonoBehaviour
{
    //包裹Class
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
    [Header("UI 顯示的技能面板")]
    public List<SkillInfo> skillsInfoList = new List<SkillInfo>();

    [Header("要顯示哪個角色的技能 (從 PlayerStateManager 抓資料)")]
    public int targetPlayerID;


    #endregion

    //生命週期
    #region 生命週期
    private void OnEnable() {
        StartCoroutine(WaitAndInit());   
    }
    #endregion

    private IEnumerator WaitAndInit() {
        yield return new WaitUntil(() =>  PlayerStateManager.Instance != null);
        UpdateSkillUsageMasteryUI();
    }


    //更新UI技能熟練度介面
    #region UpdateSkillUsageMasteryUI()
    public void UpdateSkillUsageMasteryUI() {
        if (!PlayerStateManager.Instance.playerStatesDtny.TryGetValue(targetPlayerID, out var stats))
        {
            Debug.LogError($"[UISkillUsageController] 找不到 PlayerStatsRuntime, playerID={targetPlayerID}");
            return;
        }
        if (UIManager.Instance == null || PlayerStateManager.Instance == null) return;

        foreach (var skillInfo in skillsInfoList)
        {
            var skill = stats.GetSkillInSkillPoolDtny(skillInfo.skillID);
            if (skill == null)
            {
                Debug.LogWarning($"[UISkillUsageController] 技能 {skillInfo.skillID} 不存在於角色 {stats.playerName} 的技能池");
                continue;
            }

            // 技能等級
            if (skillInfo.levelText != null) skillInfo.levelText.text = $"Lv.{skill.currentLevel}";
            if (skillInfo.nextLevelText != null) skillInfo.nextLevelText.text = $"Lv.{skill.currentLevel + 1}";

            // 攻擊力
            if (skillInfo.attackText != null) skillInfo.attackText.text = $"ATK:{skill.attack}";
            if (skillInfo.nextAttackText != null) skillInfo.nextAttackText.text = $"ATK:{skill.attack + 1}";

            // 使用次數（需求數先用 nextSkillLevelCount）
            if (skillInfo.usageText != null)
                skillInfo.usageText.text = $"使用次數:{skill.skillUsageCount}/{skill.nextSkillLevelCount}";
            
            //expBar 伸縮
            if (skillInfo.expBar != null)
            {
                float progress = 0f;

                if (skill.nextSkillLevelCount > 0) // 避免除以 0
                    progress = Mathf.Clamp01((float)skill.skillUsageCount / skill.nextSkillLevelCount);

                Vector3 scale = skillInfo.expBar.transform.localScale;
                scale.x = progress; //  根據進度改 X 軸
                skillInfo.expBar.transform.localScale = scale;
            }
        }
    }
    #endregion
}
