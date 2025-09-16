using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class PlayerStateManager : MonoBehaviour
{
    //定義
    #region
    public static PlayerStateManager Instance { get; private set; }


    public GameObject playerParent;
    public GameObject playerPreviewParent;
    public HashSet<int> unlockedPlayerIDsHashSet = new HashSet<int>();
    public Vector3 stageSpawnPosition;


    // 已解鎖角色_字典(包含「戰鬥腳色」實體、「UI角色」實體、及Stats）
    public Dictionary<int, PlayerStatsRuntime> playerStatesDtny = new Dictionary<int, PlayerStatsRuntime>();
    public Dictionary<int, GameObject> deployedPlayersGameObjectDtny = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> uiPlayersGameObjectDtny = new Dictionary<int, GameObject>(); // 如果要追蹤 UI 預覽角色
    #endregion

    //生命週期」
    #region Awake()、IEnumerator Start()
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
    private IEnumerator Start() {
        yield return StartCoroutine(GameManager.Instance.WaitForDataReady());
    }
    #endregion

 
    //解鎖並生成唯一角色
    #region UnlockAndSpawnPlayer(int playerID)
    public void UnlockAndSpawnPlayer(int playerID) {
        if (!unlockedPlayerIDsHashSet.Contains(playerID))
        {
            unlockedPlayerIDsHashSet.Add(playerID);

            // 解鎖當下：生成唯一戰鬥用實體 +UI 預覽實體
            if (!playerStatesDtny.TryGetValue(playerID, out var stats) || stats.playerPrefab == null)
            {
                Debug.LogError($"[UnlockPlayer] 無法解鎖玩家 {playerID}：缺 stats 或 prefab 為 null");
                return;
            }
            SpawnBattleAndUIPlayer(playerID,stats);
        }
    }

    private void SpawnBattleAndUIPlayer(int playerID,PlayerStatsRuntime stats) {
        //生成battle角色物件
        GameObject battlePlayerObject = GetSpawnPlayer(playerID,  Quaternion.identity, playerParent);
        //生成UI角色物件
        GameObject uiPlayerObject = GetSpawnPlayer(playerID,  Quaternion.identity, playerPreviewParent);
        
        if (uiPlayerObject != null) 
            UIManager.Instance.activeUIPlayersDtny[playerID] = uiPlayerObject;
        stats.uiPlayerObject = uiPlayerObject;
        if (battlePlayerObject)
            deployedPlayersGameObjectDtny[playerID] = battlePlayerObject;
        stats.battlePlayerObject = battlePlayerObject;

    }
    #endregion

    //解鎖技能&裝備技能槽技能(int playerID)，預設解鎖1、2技能
    #region SetupDefaultSkills(int playerID)
    public void SetupDefaultSkills(int playerID,int skillID,int slotIndex) {
        playerStatesDtny[playerID].UnlockSkill(skillID);                           //解鎖技能1                   //解鎖技能2
        playerStatesDtny[playerID].SetSkillAtSlot(slotIndex, skillID);            //裝備技能1在技能槽1(index=0)
    }
    #endregion

    //生成腳色、初始化playerStats
    #region GetSpawnPlayer(int playerID, Vector3 position, Quaternion rotation, GameObject parentObject)
    private GameObject GetSpawnPlayer(int playerID, Quaternion rotation, GameObject parentObject) {
        if (!playerStatesDtny.TryGetValue(playerID, out var playerStats) || playerStats.playerPrefab == null)
        {
            Debug.LogError($"[PlayerStateManager] 無法生成玩家 {playerID}，可能是 playerPrefab 為 null");
            return null;
        }

        GameObject playerPrefab = Instantiate(playerStats.playerPrefab, Vector3.zero, rotation, parentObject.transform);
        playerPrefab.SetActive(false);
        playerPrefab.transform.localPosition = new Vector3(0,0,0);
        Player player = playerPrefab.GetComponent<Player>();        // 確保角色能讀取自身的 PlayerStats
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

    //啟用所有deployedPlayerDtny裡的腳色
    #region ActivateAllPlayer()
    public void ActivateAllPlayer() {
        float offsetY = -1;
        Vector3 spawnPos = stageSpawnPosition;

        foreach (var kv in deployedPlayersGameObjectDtny)
        {
            var currentPlayerObject = kv.Value;
            if (!currentPlayerObject) continue;

            currentPlayerObject.SetActive(true);
            StartCoroutine(SpawnWithDrop(currentPlayerObject, spawnPos));

            spawnPos = new Vector3(spawnPos.x, spawnPos.y + offsetY, spawnPos.z);
        }
    }
    #endregion
    //生成腳色逐一向下位移
    #region IEnumerator SpawnWithDrop(GameObject player, Vector3 groundPos) {
    private IEnumerator SpawnWithDrop(GameObject player, Vector3 groundPos) {
        // 從上方開始 (比地面高一點，例如 +3)
        Vector3 startPos = groundPos + new Vector3(0, 10f, 0);
        player.transform.position = startPos;

        float t = 0;
        float duration = 0.5f; // 下落時間

        // 下落 (ease in)
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            player.transform.position = Vector3.Lerp(startPos, groundPos, t * t);
            yield return null;
        }

        // 彈一下 (往上小抬起再回來)
        Vector3 bouncePos = groundPos + new Vector3(0, 0.3f, 0);
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / 0.2f; // 彈跳快一點
            player.transform.position = Vector3.Lerp(groundPos, bouncePos, Mathf.Sin(t * Mathf.PI));
            yield return null;
        }

        // 最後確保回到地面
        player.transform.position = groundPos;
    }
    #endregion
    //反激活角色，並重置位置到原點
    #region DeactivateAllPlayer()
    public void DeactivateAllPlayer() {
        foreach (var kv in deployedPlayersGameObjectDtny) {
            var currentPlayerObject = kv.Value;
            if (!currentPlayerObject) continue;
            currentPlayerObject.SetActive(false);
            currentPlayerObject.transform.position = Vector2.zero;
        }
    }
    #endregion

    // =================================================== RUNTIME 狀態類 ==========================================================

    //PlayerStatsRuntime :Player中的playerStats
    #region class PlayerStatsRuntime
    [System.Serializable]
    public class PlayerStatsRuntime
    {
        //變數
        #region 變數
        public int playerID;
        public string playerName;
        public int level;
        public int currentEXP;
        public int[] expTable;
        public int maxHealth;
        public int attackPower;
        public float moveSpeed;
        public PlayerStatData.PlayerStatsTemplate.PlayerType playerType;
        public GameObject playerPrefab;
        public GameObject damageTextPrefab;

        public Dictionary<int, SkillData> skillPoolDtny = new Dictionary<int, SkillData>(); // 
        public List<int> unlockedSkillIDList = new List<int>();
        public List<int> equippedSkillIDList = new List<int>();

        public GameObject battlePlayerObject;
        public GameObject uiPlayerObject;
        #endregion

        //PlayerStatsRuntime建構、變數從「PlayerStatsTemplate.SkillData」深拷貝
        public PlayerStatsRuntime(PlayerStatData.PlayerStatsTemplate original) {
            // stats的變數 拷貝
            playerID = original.playerID;
            playerName = original.playerName;
            level = original.level;
            currentEXP = original.currentEXP;
            expTable = (original.expTable != null) ? (int[])original.expTable.Clone() : new int[0];
            maxHealth = original.maxHealth;
            attackPower = original.attackPower;
            moveSpeed = original.moveSpeed;
            playerType = original.playerType;
            playerPrefab = original.playerPrefab;
            damageTextPrefab = original.damageTextPrefab;

            //skillList裡的skillData  拷貝存入skillPool字典
            skillPoolDtny = new Dictionary<int, SkillData>();
            foreach (var skill in original.skillPoolList)
            {
                skillPoolDtny[skill.skillID] = new SkillData(skill);
            }

            unlockedSkillIDList = new List<int>(original.unlockedSkillIDList);
            equippedSkillIDList = new List<int>(original.equippedSkillIDList);
        }


        //skillData
        [System.Serializable]
        public class SkillData
        {
            //變數
            #region 變數
            public int skillID;
            public string skillName;
            public int currentLevel;
            public int attack ;
            public float cooldown;
            public int skillUsageCount;
            public int nextSkillLevelCount;

            public GameObject skillPrefab;
            public GameObject targetDetectPrefab;

            public bool isUnlocked;

            #endregion
            // SkillData建構、skilldata的變數 從 「PlayerStatsTemplate.SkillData」深拷貝
            #region SkillData(PlayerStatData.PlayerStatsTemplate.SkillData original)
            public SkillData(PlayerStatData.PlayerStatsTemplate.SkillData original) {
                skillID = original.skillID;
                skillName = original.skillName;
                currentLevel = original.currentLevel;
                attack = original.attack;
                cooldown = original.cooldown;
                skillUsageCount = original.skillUsageCount;
                nextSkillLevelCount = original.nextSkillLevelCount;

                skillPrefab = original.skillPrefab;
                targetDetectPrefab = original.targetDetectPrefab;
            }
            #endregion
        }


        //用skillID取得SkillData
        #region GetSkillInSkillPoolDtny(int skillID)
        public SkillData GetSkillInSkillPoolDtny(int skillID) {
            return skillPoolDtny.TryGetValue(skillID, out SkillData skill) ? skill : null;
        }
        #endregion

        //用slotIndex取得SkillData     (腳色技能槽上)
        #region GetSkillAtSkillSlot(int slotIndex) 
        public SkillData GetSkillAtSkillSlot(int slotIndex) {
            if (slotIndex < 0 || slotIndex >= equippedSkillIDList.Count)
            {
                Debug.LogError($"[PlayerStateManager] 嘗試讀取未裝備的技能槽: {slotIndex}");
                return null;
            }

            int skillID = equippedSkillIDList[slotIndex];
            return skillID != -1 ? GetSkillInSkillPoolDtny(skillID) : null;
        }
        #endregion

        //用slotIndex取得SkillName     (腳色技能槽上)
        #region GetSkillNameAtSlot(int slotIndex)
        public string GetSkillNameAtSlot(int slotIndex) {
            var skill = GetSkillAtSkillSlot(slotIndex);
            return skill != null ? skill.skillName : "";
        }
        #endregion

        //用slotIndex取得currentLevel     (腳色技能槽上)
        #region GetSkillLevelAtSlot(int slotIndex)
        public int GetSkillLevelAtSlot(int slotIndex) {
            var skill = GetSkillAtSkillSlot(slotIndex);
            return skill != null ? skill.currentLevel : 0;
        }
        #endregion

        //解鎖技能
        #region UnlockSkill(int skillID)
        public void UnlockSkill(int skillID) {
            if (!skillPoolDtny.ContainsKey(skillID))
            {
                Debug.LogWarning($"角色{playerID}{playerName} 嘗試解鎖不存在skillPool的技能 ID: {skillID}");
                return;
            }
            if (unlockedSkillIDList.Contains(skillID))
            {
                Debug.Log($"角色{playerID}{playerName} 的技能{skillID} 已解鎖，無需重複解鎖");
                return;
            }

            unlockedSkillIDList.Add(skillID);
            skillPoolDtny[skillID].isUnlocked = true;
            //Debug.Log($"角色{playerID}{playerName} 解鎖技能 ID {skillID}{skillPoolDtny[skillID].skillName}");
        }
        #endregion

        //透過Player去設定技能槽上技能
        #region SetSkillAtSlot(int slotIndex, int skillID)
        public void SetSkillAtSlot(int slotIndex, int skillID) {
            if (slotIndex < 0 || slotIndex >= 4) return;
            if (!skillPoolDtny.ContainsKey(skillID))
            {
                Debug.LogWarning($"角色{playerID}試圖裝備skillPoolDtny沒有的技能 ID: {skillID}");
                return;
            }
            if (!unlockedSkillIDList.Contains(skillID))
            {
                Debug.LogWarning($"角色{playerID}試圖裝備未解鎖的技能 ID: {skillID}");
                return;
            }
            if (!skillPoolDtny.TryGetValue(skillID, out SkillData skillData))
            {
                Debug.LogError($"[SetSkillAtSlot] 無法取得技能 ID: {skillID}");
                return;
            }

            // 更新裝備清單
            if (slotIndex >= equippedSkillIDList.Count)
            {
                Debug.LogError($"[SetSkillAtSlot] 技能槽 {slotIndex} 超出 equippedSkillIDList 範圍");
                return;
            }
            equippedSkillIDList[slotIndex] = skillID;

            // 呼叫 Player 自己設定技能
            GameObject battlePlayerObject = this.battlePlayerObject;
            if (battlePlayerObject != null)
            {
                battlePlayerObject.GetComponent<Player>()?.SetSkillSlot(slotIndex, skillData);
            }
            else
            {
                Debug.LogError($"[SetSkillAtSlot] 未找到戰鬥用角色 {playerID}");
            }

            GameObject previewPlayerObject = UIManager.Instance.activeUIPlayersDtny.ContainsKey(playerID)
                ? UIManager.Instance.activeUIPlayersDtny[playerID]
                : null;
            if (previewPlayerObject != null)
            {
                previewPlayerObject.GetComponent<Player>()?.SetSkillSlot(slotIndex, skillData);
            }
            else
            {
                Debug.LogError($"[SetSkillAtSlot] 未找到 UI 預覽角色 {playerID}");
            }
        }
        #endregion
    }
    #endregion


    //GameManager載入資料時，存入本地的playerStatesDtny
    #region SetPlayerStatesDtny(PlayerStatData playerStatData)方法
    public void SetPlayerStatesDtny(PlayerStatData playerStatData) {

        playerStatesDtny.Clear();
        foreach (var stat in playerStatData.playerStatsList)
        {
            playerStatesDtny[stat.playerID] = new PlayerStatsRuntime(stat);
        }
    }
    #endregion

    //API
    #region

    //取得戰鬥角色
    #region GetBattlePlayerObject(int playerID)
    public GameObject GetBattlePlayerObject(int playerID) {
        return playerStatesDtny.TryGetValue(playerID, out var stats) ? stats.battlePlayerObject : null;
    }
    #endregion

    //取得UI角色
    #region  GetUIPlayerObject(int playerID)
    public GameObject GetUIPlayerObject(int playerID) {
        return playerStatesDtny.TryGetValue(playerID, out var stats) ? stats.uiPlayerObject : null;
    }
    #endregion

    #endregion
}

