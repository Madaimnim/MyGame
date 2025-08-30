using System.Collections.Generic;
using UnityEngine;

public class ExpManager : MonoBehaviour
{
    public static ExpManager Instance { get; private set; }

    private readonly HashSet<PlayerExpUp> players = new HashSet<PlayerExpUp>();

    void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;   // �C���@�ӡA���n DontDestroyOnLoad
    }
    void OnDestroy() { if (Instance == this) Instance = null; }

    public void Register(PlayerExpUp p) { 
        if (p != null) players.Add(p); 
    }
    public void Unregister(PlayerExpUp p) {
        if (p != null) players.Remove(p); 
    }

    // ���Ҧ��w���U���a�P�B�g��
    public void ExpToAllPlayer(int exp) {
        if (exp <= 0 || players.Count == 0) return;
        foreach (var p in players) p.AddExp(exp);
    }
}
