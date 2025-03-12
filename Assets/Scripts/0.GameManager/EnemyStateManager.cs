using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class EnemyStateManager : MonoBehaviour
{
    #region 定義
    public static EnemyStateManager Instance { get; private set; }
    public Dictionary<int, EnemyStats> enemyStatesDtny = new Dictionary<int, EnemyStats>();

    public GameObject enemyParent;
    public Vector3 stageSpawnPosition;

    #endregion
    #region 生命週期
    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    private IEnumerator Start() {
        yield return StartCoroutine(GameManager.Instance.WaitForDataReady());
    }



    #region SetEnemyStatesDtny(EnemyStatData enemyStatData)方法
    //GameManager載入資料時，存入本地的enemyStatesDtny
    public void SetEnemyStatesDtny(EnemyStatData enemyStatData) {

        enemyStatesDtny.Clear();
        foreach (var stat in enemyStatData.enemyStatsList)
        {
            enemyStatesDtny[stat.enemyID] = new EnemyStats(stat);
        }
    }
    #endregion

    #region SpawnEnemy(int enemyID, Vector3 position, Quaternion rotation)
    //怪物生成
    private GameObject SpawnEnemy(int enemyID, Vector3 position, Quaternion rotation, GameObject parentObject) {
        if (!enemyStatesDtny.TryGetValue(enemyID, out var enemyStats) || enemyStats.enemyPrefab == null)
        {
            Debug.LogError($"[EnemyStateManager] 無法生成玩家 {enemyID}，可能是 enemyPrefab 為 null");
            return null;
        }

        // 生成角色
        GameObject enemyPrefab = Instantiate(enemyStats.enemyPrefab, position, rotation, parentObject.transform);

        // 確保角色能讀取自身的 EnemyStats
        Enemy enemy = enemyPrefab.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Initialize(enemyStats);
        }
        else
        {
            Debug.LogWarning($"[EnemyStateManager] 玩家 {enemyID} 沒有 EnemyController，無法初始化屬性");
        }
        return enemyPrefab;
    }
    #endregion

    #region 建構
    [System.Serializable]
    public class EnemyStats
    {
        public int enemyID;
        public string enemyName;
        public int level;
        public int maxHealth;
        public int attackPower;
        public float moveSpeed;
        public int currentEXP;
        public MoveStrategyType moveStrategyType;

        public int currentHealth;

        public Sprite spriteIcon;
        public GameObject enemyPrefab;
        public GameObject damageTextPrefab;

        public Dictionary<int, SkillData> skillPoolDtny = new Dictionary<int, SkillData>(); // 
        public List<int> unlockedSkillIDList = new List<int>();
        public List<int> equippedSkillIDList = new List<int>();

        #region GetSkill(int skillID)
        public SkillData GetSkill(int skillID) {
            return skillPoolDtny.TryGetValue(skillID, out SkillData skill) ? skill : null;
        }
        #endregion
        # region GetSkillAtSkillSlot(int slotIndex) 
        public SkillData GetSkillAtSkillSlot(int slotIndex) {
            if (slotIndex < 0 || slotIndex >= equippedSkillIDList.Count)
            {
                Debug.LogError($"[EnemyStateManager] 嘗試讀取未裝備的技能槽: {slotIndex}");
                return null;
            }

            int skillID = equippedSkillIDList[slotIndex];
            return skillID != -1 ? GetSkill(skillID) : null;
        }
        #endregion
        #region SetSkillAtSlot(int slotIndex, int skillID)
        public void SetSkillAtSlot(int slotIndex, int skillID) {
            if (slotIndex < 0 || slotIndex >= equippedSkillIDList.Count) return;

            if (skillID > 0 && !skillPoolDtny.ContainsKey(skillID))
            {
                Debug.LogWarning($"[EnemyStats] 試圖裝備未解鎖的技能 ID: {skillID}");
                return;
            }
            equippedSkillIDList[slotIndex] = skillID;
        }
        #endregion

        public EnemyStats(EnemyStatData.EnemyStats original) {
            enemyID = original.enemyID;
            enemyName = original.enemyName;
            level = original.level;
            maxHealth = original.maxHealth;
            attackPower = original.attackPower;
            moveSpeed = original.moveSpeed;
            moveStrategyType = original.moveStrategyType;

            spriteIcon = original.spriteIcon;
            enemyPrefab = original.enemyPrefab;
            damageTextPrefab = original.damageTextPrefab;

            currentHealth = maxHealth;

            skillPoolDtny = new Dictionary<int, SkillData>();
            foreach (var skill in original.skillPoolList)
            {
                skillPoolDtny[skill.skillID] = new SkillData(skill);
            }

            unlockedSkillIDList = new List<int>(original.unlockedSkillIDList);
            equippedSkillIDList = new List<int>(original.equippedSkillIDList);
        }

        [System.Serializable]
        public class SkillData
        {
            public int skillID;
            public string skillName;
            public int currentLevel = 1;
            public Dictionary<int, SkillLevelData> skillLevelsDataDtny = new Dictionary<int, SkillLevelData>();

            public SkillData(EnemyStatData.EnemyStats.SkillData original) {
                skillID = original.skillID;
                skillName = original.skillName;
                currentLevel = original.currentLevel;
                skillLevelsDataDtny = new Dictionary<int, SkillLevelData>();

                foreach (var levelData in original.skillLevelsDataList)
                {
                    skillLevelsDataDtny[levelData.level] = new SkillLevelData(levelData); // ✅ 以 level 當 Key 存入
                }
            }
            public SkillLevelData GetSkillLevelData(int level) {
                return skillLevelsDataDtny.TryGetValue(level, out SkillLevelData skillLevel) ? skillLevel : null;
            }
        }

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

            public SkillLevelData(EnemyStatData.EnemyStats.SkillLevelData original) {
                level = original.level;
                cooldownTime = original.cooldownTime;

                moveSpeed = original.moveSpeed;
                knockForce = original.knockForce;
                destroySelfDelay = original.destroySelfDelay;
                attackPower = original.attackPower;
                targetLayers = original.targetLayers;

                attackPrefab = original.attackPrefab;
                attackDetectPrefab = original.attackDetectPrefab;
            }
        }
    }
    #endregion
}

