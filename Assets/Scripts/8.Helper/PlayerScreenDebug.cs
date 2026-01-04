using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DebugItem {
    [HideInInspector]
    public string KeyContains;

    public bool Enable = true;
}

public class PlayerScreenDebug : MonoBehaviour {
    private static PlayerScreenDebug _instance;

    // 真正顯示的資料
    private Dictionary<string, string> _debugMap = new();

    [Header("總開關")]
    public bool EnableDebug = true;

    [Header("Debug 目標（手動輸入 PlayerId）")]
    [Tooltip("-1 = 不顯示任何角色")]
    [SerializeField] private int manualDebugPlayerId = -1;

    // Player / Enemy 端只讀用
    public static int DebugPlayerId = -1;

    [Header("顯示狀態控制（自動產生，只能勾）")]
    public List<DebugItem> DebugItems = new();

    [Header("UI 設定")]
    public int FontSize = 30;
    public Vector2 StartPos = new Vector2(10, 10);
    private float LineHeight => FontSize * 1.2f;

    [Header("顏色設定")]
    public Color TrueColor = Color.blue;
    public Color FalseColor = Color.red;
    public Color DefaultColor = Color.white;

    // =========================
    // 預設 Debug Key（唯一來源）
    // =========================
    private static readonly string[] DefaultDebugKeys =
    {
        // 基本狀態
        "IsDead",
        "IsKnocked",
        "IsGrounded",
        "IsInitialHeight",
        "IsInGravityFall",

        // 行為狀態
        "IsMoving",
        "IsAttackingIntent",
        "IsPlayingAttackAnimation",
        "IsMoveAnimationOpen",

        // 控制 / 鎖定
        "IsControlLocked",
        "IsSkillDashing",

        // 派生狀態
        "CanMove",
        "CanAttack",
        "CanRecoverHeight",
    };

    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Inspector 初始化 / 修改時自動補齊 DebugItems
    private void Reset() => BuildDefaultDebugItems();
    private void OnValidate() => BuildDefaultDebugItems();

    private void BuildDefaultDebugItems() {
        if (DebugItems == null)
            DebugItems = new List<DebugItem>();

        HashSet<string> exist = new();
        foreach (var item in DebugItems) {
            if (!string.IsNullOrEmpty(item.KeyContains))
                exist.Add(item.KeyContains);
        }

        foreach (var key in DefaultDebugKeys) {
            if (exist.Contains(key)) continue;

            DebugItems.Add(new DebugItem {
                KeyContains = key,
                Enable = false
            });
        }
    }

    private void LateUpdate() {
        UpdateDebugTarget();
    }

    // =========================
    // Debug 目標判定（核心）
    // =========================
    private void UpdateDebugTarget() {
        // -1：強制不顯示
        if (manualDebugPlayerId < 0) {
            DebugPlayerId = -1;
            return;
        }

        // 嘗試從 PlayerUtility 取得 Player
        if (PlayerUtility.AllPlayers != null &&
            PlayerUtility.AllPlayers.TryGetValue(manualDebugPlayerId, out var player) &&
            player != null &&
            player.Rt != null) {
            DebugPlayerId = manualDebugPlayerId;
        }
        else {
            // ID 不存在 / 尚未生成 / 已銷毀
            DebugPlayerId = -1;
        }
    }

    // =========================
    // 對外 API（Player / Enemy 用）
    // =========================
    public static void Set(string key, object value) {
        if (_instance == null) return;
        if (!_instance.EnableDebug) return;

        _instance._debugMap[key] = value?.ToString() ?? "null";
    }

    public static void Clear() {
        if (_instance == null) return;
        _instance._debugMap.Clear();
    }

    // =========================
    // GUI
    // =========================
    private void OnGUI() {
        if (!EnableDebug) return;
        if (DebugPlayerId == -1) return;

        GUIStyle style = new GUIStyle(GUI.skin.label) {
            fontSize = FontSize,
            wordWrap = false,
            clipping = TextClipping.Overflow
        };

        int line = 0;

        foreach (var pair in _debugMap) {
            // Player ID 過濾
            if (!pair.Key.Contains($"Player {DebugPlayerId}"))
                continue;

            // Inspector 勾選過濾
            if (!IsItemEnabled(pair.Key))
                continue;

            // 顏色
            style.normal.textColor = DefaultColor;
            if (pair.Value == "True") style.normal.textColor = TrueColor;
            else if (pair.Value == "False") style.normal.textColor = FalseColor;

            GUI.Label(
                new Rect(
                    StartPos.x,
                    StartPos.y + line * LineHeight,
                    1000,
                    LineHeight
                ),
                $"{pair.Key}: {pair.Value}",
                style
            );

            line++;
        }
    }

    // =========================
    // Inspector 勾選控制
    // =========================
    private bool IsItemEnabled(string key) {
        foreach (var item in DebugItems) {
            if (string.IsNullOrEmpty(item.KeyContains))
                continue;

            if (key.Contains(item.KeyContains))
                return item.Enable;
        }

        return false;
    }
}
