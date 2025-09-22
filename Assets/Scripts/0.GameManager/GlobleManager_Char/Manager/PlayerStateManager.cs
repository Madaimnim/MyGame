using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class PlayerStateManager : CharStateManagerBase<PlayerStatsRuntime,PlayerSkillRuntime>
{
    public static PlayerStateManager Instance { get; private set; }

    [Header("父物件設定")]
    public GameObject playerParent;
    public GameObject playerPreviewParent;
    public Vector3 stageSpawnPosition;

    [Header("角色管理")]
    public HashSet<int> unlockedPlayerIDsHashSet = new HashSet<int>();      //UI專用，用來檢查以解鎖的腳色Id
    public Dictionary<int, GameObject> deployedPlayersGameObjectDtny = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> uiPlayersGameObjectDtny = new Dictionary<int, GameObject>();

    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetPlayerStatesDtny(PlayerStatData playerStatData) {
        var runtimeList = new List<PlayerStatsRuntime>();
        foreach (var stat in playerStatData.playerStatsTemplateList)
            runtimeList.Add(new PlayerStatsRuntime(stat));
        SetStates(runtimeList);
    }
    protected override bool IsValidId(int id) => IDValidator.IsPlayerID(id);

    //生成腳色
    public bool UnlockPlayer(int playerId) {
        if (!IsValidId(playerId))
        {
            Debug.LogError($"❌ UnlockPlayer: {playerId} 不是合法的 ID");
            return false;
        }
        return unlockedPlayerIDsHashSet.Add(playerId); // HashSet 會自動避免重複
    }
    public void SpawnBothPlayers(int playerId) {
        SpawnBattlePlayer(playerId);
        SpawnUIPlayer(playerId);
    }
    public GameObject SpawnBattlePlayer(int playerId) {
        if (!statesDtny.TryGetValue(playerId, out var playerStatsRuntime)) return null;

        var playerObj= SpawnPlayerObject(playerStatsRuntime, playerParent.transform);
        if (playerObj != null) 
            deployedPlayersGameObjectDtny[playerId] = playerObj;
        
        playerStatsRuntime.BattlePlayerObject = playerObj;
        return playerObj;
    }
    public GameObject SpawnUIPlayer(int playerId) {
        if (!statesDtny.TryGetValue(playerId, out var playerStatsRuntime)) return null;

        var playerObj = SpawnPlayerObject(playerStatsRuntime, playerPreviewParent.transform);
        if (playerObj != null)
        {
            uiPlayersGameObjectDtny[playerId] = playerObj;
            UIManager.Instance.activeUIPlayersDtny[playerId] = playerObj;
        }
        playerStatsRuntime.UiPlayerObject = playerObj;
        return playerObj;
    }
    private GameObject SpawnPlayerObject(PlayerStatsRuntime playerStatsRuntime, Transform parent) {
        if (playerStatsRuntime.CharPrefab == null) return null;

        GameObject playerObj = Instantiate(playerStatsRuntime.CharPrefab, Vector3.zero, Quaternion.identity, parent);
        playerObj.SetActive(false);
        playerObj.transform.localPosition = Vector3.zero;

        var player = playerObj.GetComponent<Player>();
        if (player != null)
            player.Initialize(playerStatsRuntime);
        else
            Debug.LogError($"❌ SpawnPlayerObject: {playerObj.name} 沒有 Player 組件");

        return playerObj;
    }

    //技能槽設置
    public void SetupPlayerSkillSlot(int playerId, int slotIndex, int skillId) {
        if (!statesDtny.TryGetValue(playerId, out var playerStatsRuntime))
        {
            Debug.LogError($"[SetupPlayerSkillSlot] 找不到玩家 ID:{playerId}");
            return;
        }
        playerStatsRuntime.SetSkillAtSlot(slotIndex, skillId);
        var skillDataRuntime = playerStatsRuntime.GetSkillDataRuntimeForId(skillId);

        // PlayerStateManager 只通知 Player 物件
        deployedPlayersGameObjectDtny[playerId]?.GetComponent<Player>()?.SetSkillSlot(slotIndex, playerStatsRuntime.GetSkillDataRuntimeForId(skillId));
        uiPlayersGameObjectDtny[playerId]?.GetComponent<Player>()?.SetSkillSlot(slotIndex, playerStatsRuntime.GetSkillDataRuntimeForId(skillId));
    }

    public void AddExpToAllPlayers(int exp) {
        if (exp <= 0) return;
        foreach (var kv in statesDtny)
        {
            kv.Value.GainExp(exp);
        }
    }

    //啟用/停用角色
    public void ActivateAllPlayer() {
        float offsetY = -1;
        Vector3 spawnPos = stageSpawnPosition;

        foreach (var kv in deployedPlayersGameObjectDtny)
        {
            if (!kv.Value) continue;

            kv.Value.SetActive(true);
            StartCoroutine(SpawnWithDrop(kv.Value, spawnPos));
            spawnPos.y += offsetY;
        }
    }
    private IEnumerator SpawnWithDrop(GameObject player, Vector3 groundPos) {
        Vector3 startPos = groundPos + new Vector3(0, 10f, 0);
        player.transform.position = startPos;

        float t = 0;
        float duration = 0.5f;

        // 下落
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            player.transform.position = Vector3.Lerp(startPos, groundPos, t * t);
            yield return null;
        }

        // 彈一下
        Vector3 bouncePos = groundPos + new Vector3(0, 0.3f, 0);
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / 0.2f;
            player.transform.position = Vector3.Lerp(groundPos, bouncePos, Mathf.Sin(t * Mathf.PI));
            yield return null;
        }

        player.transform.position = groundPos;
    }
    public void DeactivateAllPlayer() {
        foreach (var kv in deployedPlayersGameObjectDtny)
        {
            if (!kv.Value) continue;
            kv.Value.SetActive(false);
            kv.Value.transform.position = Vector2.zero;
        }
    }

    //API：查詢
    public GameObject GetBattlePlayerObject(int playerID) {
        return statesDtny.TryGetValue(playerID, out var stats) ? stats.BattlePlayerObject : null;
    }
    public GameObject GetUIPlayerObject(int playerID) {
        return statesDtny.TryGetValue(playerID, out var stats) ? stats.UiPlayerObject : null;
    }
}

