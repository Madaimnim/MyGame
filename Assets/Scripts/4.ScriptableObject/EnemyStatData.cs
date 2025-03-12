using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyStatData", menuName = "GameData/EnemyStatData")]
public class EnemyStatData : ScriptableObject
{
    #region 定義
    [Header("所有敵人數據列表")]
    public List<EnemyStats> enemyStatsList = new List<EnemyStats>();
    #endregion

    #region 內含類EnemyStats
    [System.Serializable]
    public class EnemyStats
    {
        public int enemyID;
        public string enemyName;
        public int level;
        public int maxHealth;
        public int attackPower;
        public float moveSpeed;
        public MoveStrategyType moveStrategyType;

        public Sprite spriteIcon;
        public GameObject enemyPrefab;
        public GameObject damageTextPrefab;

        public List<SkillData> skillPoolList = new List<SkillData>();  //  存放角色的技能數據
        public List<int> unlockedSkillIDList = new List<int>(new int[4] { 1, -1, -1, -1 }); // 已解鎖的技能 ID
        public List<int> equippedSkillIDList = new List<int>(new int[4] { 1, -1, -1, -1 }); // 技能槽

        // **技能數據類**
        [System.Serializable]
        public class SkillData
        {
            public int skillID;
            public string skillName;
            public int currentLevel = 1;
            public List<SkillLevelData> skillLevelsDataList = new List<SkillLevelData>();
        }

        // ✅ **技能等級數據類**
        [System.Serializable]
        public class SkillLevelData
        {
            public int level;
            public float cooldownTime;

            public float moveSpeed;
            public float knockForce;
            public float destroySelfDelay;
            public int attackPower;
            public LayerMask targetLayers;

            public GameObject attackPrefab;
            public GameObject attackDetectPrefab;
        }
    }
    #endregion
}