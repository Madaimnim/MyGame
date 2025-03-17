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
    public GameObject playerPreviewParent;
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
    private IEnumerator Start() {
        yield return StartCoroutine(GameManager.Instance.WaitForDataReady());
    }
    #endregion

    #region  GetPlayerObject(int playerID)方法
    public GameObject GetPlayerObject(int playerID) {
        return activePlayersDtny.TryGetValue(playerID, out GameObject player) ? player : null;
    }
    #endregion

    #region UnlockAndSpawnPlayer(int playerID)
    //解鎖並生成腳色
    public void UnlockAndSpawnPlayer(int playerID) {
        if (!unlockedPlayerIDsHashSet.Contains(playerID))
        {
            unlockedPlayerIDsHashSet.Add(playerID);
            Debug.Log($"角色 {playerID} 解鎖成功！");
        }

        GameObject playerObject = SpawnPlayer(playerID, new Vector3(0, 0,0), Quaternion.identity, playerParent);
        activePlayersDtny[playerID] = playerObject;     
        GameObject playerUIObject = SpawnPlayer(playerID, new Vector3(0, 0, 0), Quaternion.identity, playerPreviewParent);
        UIManager.Instance.activeUIPlayersDtny[playerID] = playerUIObject;

        playerStatesDtny[playerID].UnlockSkill(1);                           //解鎖技能1
        playerStatesDtny[playerID].UnlockSkill(2);
        playerStatesDtny[playerID].UnlockSkill(3);
        playerStatesDtny[playerID].SetSkillAtSlot(0, 1);            //裝備技能1在技能槽1(index=0)
    }

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

    #region DeactivateAllPlayer()方法
    public void DeactivateAllPlayer() {
        foreach (var playerID in unlockedPlayerIDsHashSet)
        {
            GetPlayerObject(playerID).SetActive(false);
            GetPlayerObject(playerID).transform.position = new Vector2(0, 0);
        }
    }
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

    //class PlayerStats內含方法：
    //方法1：GetSkillInSkillPoolDtny(int skillID)　         return SkillData 
    //方法2：GetSkillAtSkillSlot(int slotIndex)             return SkillData 
    //方法3：UnlockSkill(int skillID)                       void
    //方法4：SetSkillAtSlot(int slotIndex, int skillID)     void
    #region class PlayerStats 建構
    [System.Serializable]
    public class PlayerStats
    {
        #region 變數
        public int playerID;
        public string playerName;
        public int level;
        public int maxHealth;
        public int attackPower;
        public float moveSpeed;
        public int currentEXP;
        public MoveStrategyType moveStrategyType;

        public int currentHealth;

        public GameObject playerPrefab;
        public GameObject damageTextPrefab;

        public Dictionary<int, SkillData> skillPoolDtny = new Dictionary<int, SkillData>(); // 
        public List<int> unlockedSkillIDList = new List<int>();
        public List<int> equippedSkillIDList = new List<int>();
        #endregion
        #region 深拷貝
        public PlayerStats(PlayerStatData.PlayerStats original) {
            Debug.Log($"[PlayerStats] 創建 {original.playerID} 的 PlayerStats");
            playerID = original.playerID;
            playerName = original.playerName;
            level = original.level;
            maxHealth = original.maxHealth;
            attackPower = original.attackPower;
            moveSpeed = original.moveSpeed;
            moveStrategyType = original.moveStrategyType;

            playerPrefab = original.playerPrefab;
            damageTextPrefab = original.damageTextPrefab;

            currentHealth = maxHealth;

            skillPoolDtny = new Dictionary<int, SkillData>();
            foreach (var skill in original.skillPoolList)
            {
                skillPoolDtny[skill.skillID] = new SkillData(skill);

                if (skill.skillID == 1)  // 只檢查 SkillID = 1
                {
                    Debug.Log($"[PlayerStats] 正在初始化技能 {skill.skillID}");
                    if (skill.targetDetectPrefab == null)
                    {
                        Debug.LogWarning($"⚠ [EquipTargetDetectPrefab] 技能 ID {skill.skillID} 的 targetDetectPrefab 為 null，請確認角色 {playerID} 是否正確設定！");
                        return ;
                    }
                }

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
            public float cooldownTime;

            public GameObject skillPrefab;
            public GameObject targetDetectPrefab;

            public SkillData(PlayerStatData.PlayerStats.SkillData original) {
                skillID = original.skillID;
                skillName = original.skillName;
                currentLevel = original.currentLevel;
                cooldownTime = original.cooldownTime;

                skillPrefab = original.skillPrefab;
                targetDetectPrefab = original.targetDetectPrefab;
            }
        }
        #endregion

        #region GetSkillInSkillPoolDtny(int skillID)
        public SkillData GetSkillInSkillPoolDtny(int skillID) {
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
            return skillID != -1 ? GetSkillInSkillPoolDtny(skillID) : null;
        }
        #endregion

        #region UnlockSkill(int skillID)
        public void UnlockSkill(int skillID) {
            // 確保技能存在於技能池
            if (!skillPoolDtny.ContainsKey(skillID))
            {
                Debug.LogWarning($"⚠ [UnlockSkill] 嘗試解鎖不存在於技能池的技能 ID: {skillID}");
                return;
            }

            // 避免重複解鎖
            if (unlockedSkillIDList.Contains(skillID))
            {
                Debug.Log($"[UnlockSkill] 技能 ID {skillID} 已解鎖，無需重複解鎖");
                return;
            }

            // 解鎖技能
            unlockedSkillIDList.Add(skillID);
            Debug.Log($"✅ [UnlockSkill] 解鎖技能 ID {skillID}");
        }
        #endregion

        //包含：
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
                    Debug.LogWarning($"試圖裝備skillPoolDtny沒有的技能 ID: {skillID}");
                    return;
                }
                if (!unlockedSkillIDList.Contains(skillID))
                {
                    Debug.LogWarning($"試圖裝備未解鎖的技能 ID: {skillID}");
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
            GameObject battlePlayerObject = Instance.GetPlayerObject(playerID);
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
            switch (slotIndex)
            {
                case 0: if (player.skillSlot1DetectPrefab) Destroy(player.skillSlot1DetectPrefab); player.skillSlot1DetectPrefab = null; break;
                case 1: if (player.skillSlot2DetectPrefab) Destroy(player.skillSlot2DetectPrefab); player.skillSlot2DetectPrefab = null; break;
                case 2: if (player.skillSlot3DetectPrefab) Destroy(player.skillSlot3DetectPrefab); player.skillSlot3DetectPrefab = null; break;
                case 3: if (player.skillSlot4DetectPrefab) Destroy(player.skillSlot4DetectPrefab); player.skillSlot4DetectPrefab = null; break;
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
                case 0: player.skillSlot1DetectPrefab = detectorObject; break;
                case 1: player.skillSlot2DetectPrefab = detectorObject; break;
                case 2: player.skillSlot3DetectPrefab = detectorObject; break;
                case 3: player.skillSlot4DetectPrefab = detectorObject; break;
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
                case 0: player.skillSlot1CooldownTime = newSkill.cooldownTime; break;
                case 1: player.skillSlot2CooldownTime = newSkill.cooldownTime; break;
                case 2: player.skillSlot3CooldownTime = newSkill.cooldownTime; break;
                case 3: player.skillSlot4CooldownTime = newSkill.cooldownTime; break;
                default: Debug.LogError($"[SetSkillDetector] 無效的技能槽索引: {slotIndex}"); break;
            }
        }
        #endregion

        #endregion
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
}

