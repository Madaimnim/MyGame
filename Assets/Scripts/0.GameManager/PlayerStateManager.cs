using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class PlayerStateManager : MonoBehaviour
{
    #region 定義
    public static PlayerStateManager Instance { get; private set; }
    public Dictionary<int, PlayerStats> playerStatesDtny = new Dictionary<int, PlayerStats>();
    public Dictionary<int, GameObject> activePlayersDtny = new Dictionary<int, GameObject>();

    public GameObject playerParent;
    public GameObject playerPreview;
    public HashSet<int> unlockedPlayerIDsHashSet = new HashSet<int>();
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
    #region UnlockAndSpawnPlayer(int playerID)
    //解鎖並生成腳色
    public void UnlockAndSpawnPlayer(int playerID) {
        if (!unlockedPlayerIDsHashSet.Contains(playerID))
        {
            unlockedPlayerIDsHashSet.Add(playerID);
            //UIManager.Instance.UpdateUICrrentIndexAndPlayer();
            Debug.Log($"角色 {playerID} 解鎖成功！");
        }

        GameObject playerObject = SpawnPlayer(playerID, new Vector3(0, 0,0), Quaternion.identity, playerParent);
        activePlayersDtny[playerID] = playerObject;
        
        GameObject playerUIObject = SpawnPlayer(playerID, new Vector3(0, 0, 0), Quaternion.identity, playerPreview);
        UIManager.Instance.activeUIPlayersDtny[playerID] = playerUIObject;
    }

    #region SpawnPlayer(int playerID, Vector3 position, Quaternion rotation)
    //腳色生成
    private GameObject SpawnPlayer(int playerID, Vector3 position, Quaternion rotation, GameObject parentObject) {
        if (!playerStatesDtny.TryGetValue(playerID, out var playerStats) || playerStats.playerPrefab == null)
        {
            Debug.LogError($"[PlayerStateManager] 無法生成玩家 {playerID}，可能是 playerPrefab 為 null");
            return null;
        }

        // 生成角色、初始localPosition為(0,0,0)，並隱藏
        GameObject playerPrefab = Instantiate(playerStats.playerPrefab, position, rotation, parentObject.transform);
        playerPrefab.transform.localPosition = new Vector3(0,0,0);
        playerPrefab.SetActive(false);

        // 確保角色能讀取自身的 PlayerStats
        Player player = playerPrefab.GetComponent<Player>();
        if (player != null)
        {
            player.Initialize(playerStats);
        }
        else
        {
            Debug.LogWarning($"[PlayerStateManager] 玩家 {playerID} 沒有 PlayerController，無法初始化屬性");
        }

        return playerPrefab;
    }
    #endregion
    #endregion

    #region ActivateALLPlayer()方法
    //激活所有腳色，並放置到關卡的出生點上
    public void ActivateAllPlayer() {
        foreach (var playerID in unlockedPlayerIDsHashSet)
        {
            GameObject currentPlayerObject=GetPlayerObject(playerID);
           
            currentPlayerObject.SetActive(true);
           
            currentPlayerObject.transform.position = stageSpawnPosition;

            stageSpawnPosition = new Vector3(stageSpawnPosition.x, stageSpawnPosition.y - 1, stageSpawnPosition.z);
        }
    }
    #endregion
    #region DeactivateAllPlayer()方法
    //激活所有腳色，並放置到關卡的出生點上
    public void DeactivateAllPlayer() {
        foreach (var playerID in unlockedPlayerIDsHashSet)
        {
            GetPlayerObject(playerID).SetActive(false);
            GetPlayerObject(playerID).transform.position = new Vector2(0,0);
        }
    }
    #endregion

    #region  GetPlayerObject(int playerID)方法
    public GameObject GetPlayerObject(int playerID) {
        return activePlayersDtny.TryGetValue(playerID, out GameObject player) ? player : null;
    }
    #endregion

    #region SetPlayerStatesDtny(PlayerStatData playerStatData)方法
    //GameManager載入資料時，存入本地的playerStatesDtny
    public void SetPlayerStatesDtny(PlayerStatData playerStatData) {

        playerStatesDtny.Clear();
        foreach (var stat in playerStatData.playerStatsList)
        {
            playerStatesDtny[stat.playerID] = new PlayerStats(stat);
        }
    }
    #endregion

    #region 建構
    [System.Serializable]
    public class PlayerStats
    {
        public int playerID;
        public string playerName;
        public int level;
        public int maxHealth;
        public int attackPower;
        public float moveSpeed;
        public int currentEXP;
        public MoveStrategyType moveStrategyType;

        public int currentHealth;

        public Sprite spriteIcon;
        public GameObject playerPrefab;
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
                Debug.LogError($"[PlayerStateManager] 嘗試讀取未裝備的技能槽: {slotIndex}");
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
                Debug.LogWarning($"[PlayerStats] 試圖裝備未解鎖的技能 ID: {skillID}");
                return;
            }
            equippedSkillIDList[slotIndex] = skillID;
        }
        #endregion

        public PlayerStats(PlayerStatData.PlayerStats original) {
            playerID = original.playerID;
            playerName = original.playerName;
            level = original.level;
            maxHealth = original.maxHealth;
            attackPower = original.attackPower;
            moveSpeed = original.moveSpeed;
            moveStrategyType = original.moveStrategyType;

            spriteIcon = original.spriteIcon;
            playerPrefab = original.playerPrefab;
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
            public Dictionary<int,SkillLevelData> skillLevelsDataDtny = new Dictionary<int,SkillLevelData>();

            public SkillData(PlayerStatData.PlayerStats.SkillData original) {
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

            public SkillLevelData(PlayerStatData.PlayerStats.SkillLevelData original) {
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

