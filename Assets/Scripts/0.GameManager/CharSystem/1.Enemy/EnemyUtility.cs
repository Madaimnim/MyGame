using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;
using System.Linq;

// 提供對玩家技能與狀態的快速存取工具類
public static class EnemyUtility
{

    public static EnemyStatsTemplate Get(int id)
    => GameManager.Instance.EnemyStateSystem.EnemyStatsTemplateDtny[id];

}
