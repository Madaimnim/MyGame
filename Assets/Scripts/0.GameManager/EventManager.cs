using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    public Action<IDamageable> Event_OnPlayerDie;
    public Action Event_OnWallBroken;
    public Action<int, int, IDamageable> Event_HpChanged;
    public Action<int, float, float, PlayerAI> Event_SkillCooldownChanged;
    public Action<int, PlayerAI> Event_SkillInfoChanged;
    public Action Event_BattleStart;

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

}
