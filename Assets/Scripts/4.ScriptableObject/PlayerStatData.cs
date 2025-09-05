using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerStatData", menuName = "GameData/PlayerStatData")]
public class PlayerStatData : ScriptableObject
{
    #region 定義
    [Header("所有玩家數據列表")]
    public List<PlayerStatsTemplate> playerStatsList = new List<PlayerStatsTemplate>();
    #endregion

    #region 內含類PlayerStats
    [System.Serializable]
    public class PlayerStatsTemplate
    {
        public int playerID;
        public string playerName;
        public int level=1;
        public int currentEXP=0;
        public int[] expTable = new int[]{
        4,6,9, 
        13, 18, 25, 34, 45, 58, 73, 90, 109, 130, // 1~10
        153, 178, 205, 234, 265, 298, 333, 370, 409, 450, // 11~20
        493, 538, 585, 634, 685, 738, 793, 850, 909, 970  // 21~30
        };
 
        public int maxHealth;
        public int attackPower;
        public float moveSpeed;
        public PlayerType playerType;
        public GameObject playerPrefab;
        public GameObject damageTextPrefab;

        public List<SkillData> skillPoolList = new List<SkillData>();  //  存放角色的技能數據
        public List<int> unlockedSkillIDList = new List<int>(new int[4] { 1, -1, -1, -1 }); // 已解鎖的技能 ID
        public List<int> equippedSkillIDList = new List<int>(new int[4] { 1, -1, -1, -1 }); // 技能槽

        public enum PlayerType
        {
            [InspectorName("近戰")] Melee,
            [InspectorName("遠程")] Ranged,
        }


        // **技能數據類**
        [System.Serializable]
        public class SkillData
        {
            public int skillID;
            public string skillName;
            public int currentLevel = 1;
            public int attack = 1;
            public float cooldown;
            public int skillUsageCount;
            public int nextSkillLevelCount;

            public GameObject skillPrefab;
            public GameObject targetDetectPrefab;
        }
    }
    #endregion
}