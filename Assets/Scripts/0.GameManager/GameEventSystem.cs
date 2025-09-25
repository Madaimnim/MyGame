using System;
using UnityEngine;

[DefaultExecutionOrder(-50)]   // 讓 GameEventSystem 先於大多數腳本執行
public class GameEventSystem : MonoBehaviour
{

    public static GameEventSystem Instance { get; private set; }

    public Action<IDamageable> Event_OnPlayerDie;
    public Action Event_OnWallBroken;
    public Action<int, Player> Event_SkillInfoChanged;
    public Action<PlayerSkillRuntime,Transform> Event_SkillLevelUp;

    public Action<int, int, IDamageable> Event_HpChanged;
    public Action<int, float, float, Player> Event_SkillCooldownChanged;

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
