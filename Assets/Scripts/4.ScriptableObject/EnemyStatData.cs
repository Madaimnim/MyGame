using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyStatData", menuName = "GameData/EnemyStatData")]
public class EnemyStatData : ScriptableObject
{
    #region 定義
    [Header("所有敵人數據列表")]
    public List<EnemyStatsTemplate> enemyStatsList = new List<EnemyStatsTemplate>();
    #endregion

    #region 內含類EnemyStats
    [System.Serializable]
    public class EnemyStatsTemplate
    {
        public int enemyID;
        public string enemyName;
        public int level;
        public int maxHealth;
        public float moveSpeed;
        public int exp;
        public MoveStrategyType moveStrategyType;

        public Sprite spriteIcon;
        public GameObject enemyPrefab;
        public GameObject damageTextPrefab;

        public List<SkillData> skillPoolList = new List<SkillData>();  //  存放角色的技能數據

        // **技能數據類**
        [System.Serializable]
        public class SkillData {
            public int skillID;
            public string skillName;

            [Min(0f)] public float cooldown;
            [Min(0f)] public float knockbackForce;
            [Min(0f)] public int attackPower;
        
            public GameObject attackPrefab;
        }

    }
    #endregion
}