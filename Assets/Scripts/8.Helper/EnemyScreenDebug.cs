using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class EnemyDebugItem {
    [HideInInspector]
    public string KeyContains;
    public bool Enable = true;
}

public class EnemyScreenDebug : MonoBehaviour {
    private static EnemyScreenDebug _instance;

    // =========================
    // Runtime Debug 資料
    // =========================
    private readonly Dictionary<string, string> _debugMap = new();

    [Header("總開關")]
    public bool EnableDebug = true;

    [Header("Debug 目標（手動輸入 EnemyId）")]
    [Tooltip("-1 = 不顯示任何敵人（測試時只生成一隻）")]
    [SerializeField] private int manualEnemyId = -1;

    // Enemy 端只讀
    public static int DebugEnemyId = -1;

    [Header("顯示狀態控制（自動產生，只勾選）")]
    public List<EnemyDebugItem> DebugItems = new();

    [Header("UI 設定")]
    public int FontSize = 26;
    public Vector2 StartPos = new Vector2(10, 400);
    private float LineHeight => FontSize * 1.2f;

    [Header("顏色設定")]
    public Color TrueColor = Color.cyan;
    public Color FalseColor = Color.magenta;
    public Color DefaultColor = Color.white;

    // =========================
    // Debug Key（唯一來源）
    // =========================
    private static readonly string[] DefaultDebugKeys =
    {
        // 基本狀態
        "IsDead",
        "IsKnocked",
        "IsGrounded",
        "IsInitialHeight",
        "IsInGravityFall",

        // 行為
        "IsMoving",
        "IsAttackingIntent",
        "IsPlayingAttackAnimation",

        // 控制
        "IsControlLocked",
        "IsSkillDashing",

        // 派生
        "CanMove",
        "CanAttack",
        "CanRecoverHeight",
    };

    private void Awake() {
        if (_instance != null) {
            Destroy(this);
            return;
        }
        _instance = this;
    }

    // Inspector 初始化 / 修改時自動補齊
    private void Reset() => BuildDefaultDebugItems();
    private void OnValidate() => BuildDefaultDebugItems();

    private void BuildDefaultDebugItems() {
        if (DebugItems == null)
            DebugItems = new List<EnemyDebugItem>();

        HashSet<string> exist = new();
        foreach (var item in DebugItems) {
            if (!string.IsNullOrEmpty(item.KeyContains))
                exist.Add(item.KeyContains);
        }

        foreach (var key in DefaultDebugKeys) {
            if (exist.Contains(key)) continue;

            DebugItems.Add(new EnemyDebugItem {
                KeyContains = key,
                Enable = false
            });
        }
    }

    private void LateUpdate() {
        UpdateDebugTarget();
    }

    // =========================
    // Debug 目標選擇（核心）
    // =========================
    private void UpdateDebugTarget() {
        if (!EnableDebug || manualEnemyId < 0) {
            DebugEnemyId = -1;
            return;
        }

        if (GameManager.Instance == null ||
            GameManager.Instance.EnemyStateSystem == null) {
            DebugEnemyId = -1;
            return;
        }

        // 開發期規則：只會有一隻
        Enemy target = GameManager.Instance.EnemyStateSystem.BattleEnemyList
            .FirstOrDefault(e =>
                e != null &&
                e.Rt != null &&
                e.Rt.Id == manualEnemyId);

        DebugEnemyId = target != null ? manualEnemyId : -1;
    }

    // =========================
    // 對外 API（Enemy 用）
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
        if (DebugEnemyId == -1) return;

        GUIStyle style = new GUIStyle(GUI.skin.label) {
            fontSize = FontSize,
            wordWrap = false,
            clipping = TextClipping.Overflow
        };

        int line = 0;

        foreach (var pair in _debugMap) {
            if (!pair.Key.Contains($"Enemy {DebugEnemyId}"))
                continue;

            if (!IsItemEnabled(pair.Key))
                continue;

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
