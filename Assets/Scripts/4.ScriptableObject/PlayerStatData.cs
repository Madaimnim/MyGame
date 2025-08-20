using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerStatData", menuName = "GameData/PlayerStatData")]
public class PlayerStatData : ScriptableObject
{
    #region 定義
    [Header("所有玩家數據列表")]
    public List<PlayerStats> playerStatsList = new List<PlayerStats>();
    #endregion

    #region 內含類PlayerStats
    [System.Serializable]
    public class PlayerStats
    {
        public int playerID;
        public string playerName;
        public int level;
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
            public float cooldownTime;

            public GameObject skillPrefab;
            public GameObject targetDetectPrefab;
        }
    }
    #endregion
}