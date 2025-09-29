using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;
using System.Linq;

// ���ѹ缾�a�ޯ�P���A���ֳt�s���u����
public static class PlayerUtility
{
    public static IReadOnlyDictionary<int ,PlayerStatsRuntime> All
      => GameManager.Instance.PlayerStateSystem.PlayerStatsRuntimeDtny;

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
