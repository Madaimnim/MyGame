using System.Collections.Generic;
using UnityEngine;

public class ExpManager : MonoBehaviour
{
    public static ExpManager Instance { get; private set; }

    private readonly HashSet<PlayerExpUp> players = new HashSet<PlayerExpUp>();

    void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;   // 每關一個，不要 DontDestroyOnLoad
    }
    void OnDestroy() { if (Instance == this) Instance = null; }

    public void Register(PlayerExpUp p) { 
        if (p != null) players.Add(p); 
    }
    public void Unregister(PlayerExpUp p) {
        if (p != null) players.Remove(p); 
    }

    // 給所有已註冊玩家同額經驗
    public void ExpToAllPlayer(int exp) {
        if (exp <= 0 || players.Count == 0) return;
        foreach (var p in players) p.AddExp(exp);
    }
}
