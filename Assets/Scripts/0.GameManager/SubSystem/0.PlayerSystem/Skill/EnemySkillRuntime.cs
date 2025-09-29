using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySkillRuntime : SkillBase
{
    public EnemySkillRuntime(SkillTemplate template){
        #region ▓`л■ий
        //SkillDataBase
        SkillId = template.SkillId;
        SkillName = template.SkillName;
        SkillPower = template.SkillPower;
        SkillCooldown = template.SkillCooldown;
        KnockbackForce = template.KnockbackForce;
        SkillPrefab = template.SkillPrefab;
        TargetDetectPrefab = template.TargetDetectPrefab;
        #endregion
    }
}