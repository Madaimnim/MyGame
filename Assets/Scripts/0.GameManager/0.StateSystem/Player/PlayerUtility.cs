using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;
using System.Linq;

// 提供對玩家技能與狀態的快速存取工具類
public static class PlayerUtility
{
    public static IReadOnlyDictionary<int ,PlayerStatsRuntime> AllRt
      => GameManager.Instance.PlayerStateSystem.PlayerStatsRuntimeDtny;
    public static IReadOnlyDictionary<int, GameObject>  AllObj(int id)
    => GameManager.Instance.PlayerStateSystem.BattleObjects;

    public static PlayerStatsRuntime Get(int id)
        => GameManager.Instance.PlayerStateSystem.PlayerStatsRuntimeDtny[id];

    public static PlayerStatsRuntime GetSafe(int id) {
        if (GameManager.Instance.PlayerStateSystem.PlayerStatsRuntimeDtny
            .TryGetValue(id, out var rt))
            return rt;
        return null;
    }


    public static bool Exists(int id)
        => GameManager.Instance.PlayerStateSystem.PlayerStatsRuntimeDtny.ContainsKey(id);

    public static List<int> GetUnlockedIds()
          => GameManager.Instance.PlayerStateSystem.UnlockedIdList.ToList();

}
