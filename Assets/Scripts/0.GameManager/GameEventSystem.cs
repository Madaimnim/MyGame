using System;
using UnityEngine;

[DefaultExecutionOrder(-50)]   // �� GameEventSystem ����j�h�Ƹ}������
public class GameEventSystem : MonoBehaviour
{

    public static GameEventSystem Instance { get; private set; }
    //�԰�
    public Action<int, int, IDamageable> Event_HpChanged;
    public Action<Player> Event_OnPlayerDie;

    public Action Event_OnWallBroken;
    //playerStatsRuntime
    public Action<int, Player> Event_SkillInfoChanged;
    public Action<PlayerSkillRuntime,Transform> Event_SkillLevelUp;
    //Todo �N�o��s�ݩI�s�A�|����@
    public Action<int, float, float, Player> Event_SkillCooldownChanged;
    //SkillSystem
    public Action<int, int> Event_SkillUnlocked;

    //UI
    public Action<PlayerStatsRuntime> Event_UICurrentPlayerChanged;


    //����
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
