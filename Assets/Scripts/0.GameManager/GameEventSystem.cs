using System;
using UnityEngine;

[DefaultExecutionOrder(-50)]   // 讓 GameEventSystem 先於大多數腳本執行
public class GameEventSystem : MonoBehaviour
{

    public static GameEventSystem Instance { get; private set; }
    //戰鬥
    public Action<int, int, IDamageable> Event_HpChanged;
    public Action<Player> Event_OnPlayerDie;

    public Action Event_OnWallBroken;
    //playerStatsRuntime
    public Action<int, Player> Event_SkillInfoChanged;
    public Action<PlayerSkillRuntime,Transform> Event_SkillLevelUp;
    //Todo 冷卻更新需呼叫，尚未實作
    public Action<int, float, float, Player> Event_SkillCooldownChanged;
    //SkillSystem
    public Action<int, int> Event_SkillUnlocked;

    //UI
    public Action<PlayerStatsRuntime> Event_UICurrentPlayerChanged;


    //場景
    public Action Event_BattleStart;

    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

}
