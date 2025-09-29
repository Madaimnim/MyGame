using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSkillRuntime:SkillBase
{
    //Player�W��
    public int SkillLevel { get; private set; }
    public int SkillUsageCount { get; private set; }
    public int NextSkillLevelCount { get; private set; }

    public PlayerSkillRuntime(SkillTemplate template) {
        #region �`����
        //SkillDataBase
        SkillId = template.SkillId;
        SkillName = template.SkillName;
        SkillPower = template.SkillPower;
        SkillCooldown = template.SkillCooldown;
        KnockbackForce = template.KnockbackForce;
        SkillPrefab = template.SkillPrefab;
        TargetDetectPrefab = template.TargetDetectPrefab;
        #endregion

        //PlayerRuntime
        SkillLevel = 1;
        SkillUsageCount = 0;
        NextSkillLevelCount = 10;
    }

    public bool AddSkillUsageCount() {
        SkillUsageCount++;
        if (SkillUsageCount >= NextSkillLevelCount)
        {
            SkillLevelUp();
            return true; // �i�D�I�s�ݡu�ɯŤF�v
        }
        return false;
    }
    private void SkillLevelUp() {
        SkillLevel++;
        SkillPower++;
        NextSkillLevelCount += SkillLevel * 10;
    }
}