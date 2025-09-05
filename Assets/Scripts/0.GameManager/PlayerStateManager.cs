using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class PlayerStateManager : MonoBehaviour
{
    //定義
    #region
    public static PlayerStateManager Instance { get; private set; }
    
    public Dictionary<int, PlayerStatsRuntime> playerStatesDtny = new Dictionary<int, PlayerStatsRuntime>();
    public Dictionary<int, GameObject> deployedPlayersDtny = new Dictionary<int, GameObject>();

    public GameObject playerParent;
    public GameObject playerPreviewParent;
    public HashSet<int> unlockedPlayerIDsHashSet = new HashSet<int>();
    public Vector3 stageSpawnPosition;

    // 單一角色的握柄：唯一戰鬥用實體、UI 預覽用實體、對應 runtime 數據
    [System.Serializable] 
    public class UnlockedPlayerData { public GameObject battlePlayerObject; public GameObject uiPlayerObject; public PlayerStatsRuntime stats; }
    // 已解鎖角色_字典(包含「戰鬥腳色」實體、「UI角色」實體、及Stats）
    public Dictionary<int, UnlockedPlayerData> unlockedPlayersDtny = new Dictionary<int, UnlockedPlayerData>();
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

    //取得已解鎖角色物件
    #region GameObject GetUnlockedPlayerObject(int playerID)
    public GameObject GetUnlockedPlayerObject(int playerID) {
        return unlockedPlayersDtny.TryGetValue(playerID, out var data) ? data.battlePlayerObject : null;
    }
    #endregion

    //取得上陣角色物件
    #region GameObject GetDeployedPlayerObject(int playerID)
    public GameObject GetDeployedPlayerObject(int playerID) {
        return deployedPlayersDtny.TryGetValue(playerID, out var playerObject) ? playerObject : null;
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

            SpawnPlayerAndUIPlayer(playerID,stats);
        }
    }
    #endregion

    private void SpawnPlayerAndUIPlayer(int playerID,PlayerStatsRuntime stats) {
        //生成battle角色物件
        GameObject battlePlayerObject = GetSpawnPlayer(playerID, Vector3.zero, Quaternion.identity, playerParent);
        //生成UI角色物件
        GameObject uiPlayerObject = GetSpawnPlayer(playerID, Vector3.zero, Quaternion.identity, playerPreviewParent);
        if (uiPlayerObject != null) UIManager.Instance.activeUIPlayersDtny[playerID] = uiPlayerObject;

        //將battle腳色物件，存入上陣deployed字典
        if (battlePlayerObject)
            deployedPlayersDtny[playerID] = battlePlayerObject;
        unlockedPlayersDtny[playerID] = new UnlockedPlayerData { battlePlayerObject = battlePlayerObject, uiPlayerObject = uiPlayerObject, stats = stats };
        Debug.Log($"角色 {playerID}{playerStatesDtny[playerID].playerName} 解鎖");
    }


    //解鎖技能&裝備技能槽技能(int playerID)，預設解鎖1、2技能
    #region SetupDefaultSkills(int playerID)
    public void SetupDefaultSkills(int playerID) {
        playerStatesDtny[playerID].UnlockSkill(1);                           //解鎖技能1                   //解鎖技能2
        playerStatesDtny[playerID].SetSkillAtSlot(0, 1);            //裝備技能1在技能槽1(index=0)
    }
    #endregion

    //生成腳色、初始化playerStats
    #region GameObject GetSpawnPlayer(int playerID, Vector3 position, Quaternion rotation, GameObject parentObject)
    private GameObject GetSpawnPlayer(int playerID, Vector3 position, Quaternion rotation, GameObject parentObject) {
        if (!playerStatesDtny.TryGetValue(playerID, out var playerStats) || playerStats.playerPrefab == null)
        {
            Debug.LogError($"[PlayerStateManager] 無法生成玩家 {playerID}，可能是 playerPrefab 為 null");
            return null;
        }

        // 生成角色、初始localPosition為(0,0,0)，並隱藏
        GameObject playerPrefab = Instantiate(playerStats.playerPrefab, position, rotation, parentObject.transform);
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

    //激活所有腳色，並放置到關卡的出生點上
    #region ActivateALLPlayer()方法
    //public void ActivateAllPlayer() {
    //    //位移量
    //    float offsetY = -1;
    //    Vector3 spawnPos = stageSpawnPosition;
    //    foreach (var kv in deployedPlayersDtny)
    //    {
    //        var currentPlayerObject = kv.Value;
    //        if (!currentPlayerObject) continue;
    //        currentPlayerObject.SetActive(true);
    //        currentPlayerObject.transform.position = spawnPos;
    //
    //        //Debug.Log($"{ currentPlayerObject.name}被放置到{spawnPos}");
    //
    //        spawnPos = new Vector3(spawnPos.x, spawnPos.y + offsetY, spawnPos.z);
    //        //Debug.Log($"出生點位置更新{spawnPos}");
    //    }
    //}

    public void ActivateAllPlayer() {
        float offsetY = -1;
        Vector3 spawnPos = stageSpawnPosition;

        foreach (var kv in deployedPlayersDtny)
        {
            var currentPlayerObject = kv.Value;
            if (!currentPlayerObject) continue;

            currentPlayerObject.SetActive(true);
            StartCoroutine(SpawnWithDrop(currentPlayerObject, spawnPos));

            spawnPos = new Vector3(spawnPos.x, spawnPos.y + offsetY, spawnPos.z);
        }
    }

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
    #region 
    public void DeactivateAllPlayer() {
        foreach (var kv in deployedPlayersDtny) {
            var currentPlayerObject = kv.Value;
            if (!currentPlayerObject) continue;
            currentPlayerObject.SetActive(false);
            currentPlayerObject.transform.position = Vector2.zero;
        }
    }
    #endregion



    // =================================================== RUNTIME 狀態類 ==========================================================
    
    //PlayerStatsRuntime(內含方法) 
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
        #endregion

        //PlayerStatsRuntime建構、變數從「PlayerStatsTemplate.SkillData」深拷貝
        #region PlayerStatsRuntime(PlayerStatData.PlayerStatsTemplate original)
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
        #endregion
        //skillData
        #region class skillData
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
        #endregion

        //取得skillPool字典裡的技能(skillID)
        #region GetSkillInSkillPoolDtny(int skillID)
        public SkillData GetSkillInSkillPoolDtny(int skillID) {
            return skillPoolDtny.TryGetValue(skillID, out SkillData skill) ? skill : null;
        }
        #endregion
        
        //取得技能槽上技能(slotIndex)
        # region GetSkillAtSkillSlot(int slotIndex) 
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

        //解鎖技能(skillID)
        #region UnlockSkill(int skillID)
        public void UnlockSkill(int skillID) {
            // 確保技能存在於技能池
            if (!skillPoolDtny.ContainsKey(skillID))
            {
                Debug.LogWarning($"角色{playerID}{playerName} 嘗試解鎖不存在skillPool的技能 ID: {skillID}");
                return;
            }

            // 避免重複解鎖
            if (unlockedSkillIDList.Contains(skillID))
            {
                Debug.Log($"角色{playerID}{playerName} 的技能{skillID} 已解鎖，無需重複解鎖");
                return;
            }

            // 解鎖技能
            unlockedSkillIDList.Add(skillID);
            Debug.Log($"角色{playerID}{playerName} 解鎖技能 ID {skillID}{skillPoolDtny[skillID].skillName}");
        }
        #endregion

        //設定技能槽上技能(slotIndex,skillID)包含：
        //1.確認slotIndex及SkillID合理性
        //2.取得Skill
        //3.取得「戰鬥」、「UI」兩個角色物件：
        //  (1)移除兩者：「技能槽偵測器」
        //  (2)設置兩者：新的「技能槽偵測器」
        //  (3)設置兩者：Player內的4個「冷卻時間」變數。
        //  (4)設置兩者：Player內的4個「偵測器」變數。
        #region SetSkillAtSlot(int slotIndex, int skillID)
        public void SetSkillAtSlot(int slotIndex, int skillID) {
            #region 確認SkillID存在，且已存在解鎖技能池裡
            if (slotIndex < 0 || slotIndex >= 4) return;
            if (skillID <= 0)
            {
                Debug.LogWarning("SkillID小於0");
                return;
            }
            else 
            {
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
            }
            #endregion
            #region 透過SkillPoolDtny取得指定SkillID的newSkill
            if (!skillPoolDtny.TryGetValue(skillID, out SkillData newSkill))
            {
                Debug.LogError($"[SetSkillAtSlot] 無法取得技能 ID: {skillID}");
                return;
            }
            #endregion

            equippedSkillIDList[slotIndex] = skillID;

            #region 取得戰鬥用腳色物件
            // 取得戰鬥用角色
            GameObject battlePlayerObject = Instance.GetUnlockedPlayerObject(playerID);
            if (battlePlayerObject == null)
            {
                Debug.LogError($"[SetSkillAtSlot] 未找到戰鬥用角色 {playerID}，無法裝備技能");
                return;
            }
            #endregion
            #region 取得UI預覽用腳色物件
            // 取得UI 預覽角色
            GameObject previewPlayerObject = UIManager.Instance.activeUIPlayersDtny.ContainsKey(playerID)
                ? UIManager.Instance.activeUIPlayersDtny[playerID]
                : null;
            if (previewPlayerObject == null)
            {
                Debug.LogError($"[SetSkillAtSlot] 未找到 UI 預覽角色 {playerID}");
            }
            #endregion

            #region 設置戰鬥用腳色的技能偵測器，及物件內Player腳本的CooldownTime1-4、DetectPrefab1-4。
            Player player = battlePlayerObject.GetComponent<Player>();
            RemoveSkillDetector(player, slotIndex);
            SetPlayerSkillSlotDetectPrefab(player, battlePlayerObject, slotIndex, newSkill);
            SetPlayerSkillSlotCooldownTime(player, slotIndex, newSkill);
            #endregion
            #region 設置UI預覽用腳色的技能偵測器，及物件內Player腳本的CooldownTime1-4、DetectPrefab1-4。
            player = previewPlayerObject.GetComponent<Player>();
            RemoveSkillDetector(player, slotIndex);
            SetPlayerSkillSlotDetectPrefab(player, previewPlayerObject, slotIndex, newSkill);
            SetPlayerSkillSlotCooldownTime(player, slotIndex, newSkill);
            #endregion
        }

        #region RemoveSkillDetector(Player player, int slotIndex)
        private void RemoveSkillDetector(Player player, int slotIndex) {
            if (player == null) return;
            switch (slotIndex)
            {
                case 0: if (player.skillSlots[0].detectPrefab) Destroy(player.skillSlots[0].detectPrefab); player.skillSlots[0].detectPrefab = null; break;
                case 1: if (player.skillSlots[1].detectPrefab) Destroy(player.skillSlots[1].detectPrefab); player.skillSlots[1].detectPrefab = null; break;
                case 2: if (player.skillSlots[2].detectPrefab) Destroy(player.skillSlots[2].detectPrefab); player.skillSlots[2].detectPrefab = null; break;
                case 3: if (player.skillSlots[3].detectPrefab) Destroy(player.skillSlots[3].detectPrefab); player.skillSlots[3].detectPrefab = null; break;
                default: Debug.LogError($"[RemoveSkillDetector] 無效的技能槽索引: {slotIndex}"); break;
            }
        }
        #endregion

        #region SetPlayerSkillSlotDetectPrefab(Player player, GameObject playerObject, int slotIndex, SkillData skill)
        private void SetPlayerSkillSlotDetectPrefab(Player player, GameObject playerObject, int slotIndex, SkillData skill) {
            GameObject detectorObject = EquipTargetDetectPrefab(playerObject, skill, slotIndex);
            if (detectorObject == null)
            {
                Debug.LogError($"[SetPlayerSkillSlotDetectPrefab] 裝備技能 {skill.skillID} 失敗，無法生成目標檢測物件");
                return;
            }

            switch (slotIndex)
            {
                case 0: player.skillSlots[0].detectPrefab = detectorObject; break;
                case 1: player.skillSlots[1].detectPrefab = detectorObject; break;
                case 2: player.skillSlots[2].detectPrefab = detectorObject; break;
                case 3: player.skillSlots[3].detectPrefab = detectorObject; break;
                default: Debug.LogError($"[SetPlayerSkillSlotDetectPrefab] 無效的技能槽索引: {slotIndex}"); break;
            }
        }
        #region 裝備及生成技能偵測器
        private GameObject EquipTargetDetectPrefab(GameObject playerObject, SkillData skill, int slotIndex) {
            if (playerObject == null) return null;
            if (skill == null) return null;

            if (skill.targetDetectPrefab == null)
            {
                Debug.LogError($"[EquipTargetDetectPrefab] 技能 ID {skill.skillID} 的 targetDetectPrefab 為 null");
                return null;
            }

            GameObject newDetect = Instantiate(skill.targetDetectPrefab, playerObject.transform);
            newDetect.transform.localPosition = Vector3.zero;
            newDetect.name = $"TargetDetector_{skill.skillID}";
            return newDetect;
        }
        #endregion

        #endregion
        
        #region SetPlayerSkillSlotCooldownTime(Player player, int slotIndex, SkillData newSkill)
        private void SetPlayerSkillSlotCooldownTime(Player player, int slotIndex, SkillData newSkill) {
            switch (slotIndex)
            {
                case 0: player.skillSlots[0].cooldown = newSkill.cooldown; break;
                case 1: player.skillSlots[1].cooldown = newSkill.cooldown; break;
                case 2: player.skillSlots[2].cooldown = newSkill.cooldown; break;
                case 3: player.skillSlots[3].cooldown = newSkill.cooldown; break;
                default: Debug.LogError($"[SetSkillDetector] 無效的技能槽索引: {slotIndex}"); break;
            }
        }
        #endregion

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
}

