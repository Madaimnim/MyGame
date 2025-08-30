using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class EnemyStateManager : MonoBehaviour
{
    #region 定義
    public static EnemyStateManager Instance { get; private set; }
    public Dictionary<int, EnemyStatsRuntime> enemyStatesDtny = new Dictionary<int, EnemyStatsRuntime>();

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
            enemyStatesDtny[stat.enemyID] = new EnemyStatsRuntime(stat);
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
    public class EnemyStatsRuntime
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

        public Dictionary<int, SkillData> skillPoolDtny = new Dictionary<int, SkillData>(); // 


        //取得技能Data
        #region GetSkill(int skillID)
        public SkillData GetSkill(int skillID) {
            return skillPoolDtny.TryGetValue(skillID, out SkillData skill) ? skill : null;
        }
        #endregion
       

        public EnemyStatsRuntime(EnemyStatData.EnemyStatsTemplate original) {
            enemyID = original.enemyID;
            enemyName = original.enemyName;
            level = original.level;
            maxHealth = original.maxHealth;
            moveSpeed = original.moveSpeed;
            exp = original.exp;
            moveStrategyType = original.moveStrategyType;

            spriteIcon = original.spriteIcon;
            enemyPrefab = original.enemyPrefab;
            damageTextPrefab = original.damageTextPrefab;

            skillPoolDtny = new Dictionary<int, SkillData>();
            foreach (var skill in original.skillPoolList)
            {
                skillPoolDtny[skill.skillID] = new SkillData(skill);
            }

        }

        [System.Serializable]
        public class SkillData
        {
            public int skillID;
            public string skillName;
            public float cooldownTime;
            public float knockbackForce;
            public int attackPower;
            public GameObject attackPrefab;


            public SkillData(EnemyStatData.EnemyStatsTemplate.SkillData original) {
                skillID = original.skillID;
                skillName = original.skillName;
                cooldownTime = original.cooldownTime;
                knockbackForce = original.knockbackForce;
                attackPower = original.attackPower;
                attackPrefab = original.attackPrefab;
            }        
        } 
    }
    #endregion
}

