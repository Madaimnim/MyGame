using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;
using System.Linq;

// ���ѹ缾�a�ޯ�P���A���ֳt�s���u����
public static class EnemyUtility
{

    public static EnemyStatsTemplate Get(int id)
    => GameManager.Instance.EnemyStateSystem.EnemyStatsTemplateDtny[id];

}
