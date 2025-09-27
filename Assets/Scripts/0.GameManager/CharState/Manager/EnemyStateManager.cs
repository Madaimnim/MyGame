using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class EnemyStateManager : MonoBehaviour
{

    public static EnemyStateManager Instance { get; private set; }
    public GameObject enemyParent;
    public Vector3 stageSpawnPosition;

    // 靜態資料 (Template Dictionary)
    private Dictionary<int, EnemyStatsTemplate> enemyStatsTemplateDtny = new Dictionary<int, EnemyStatsTemplate>();

    // 動態生成的敵人清單
    private readonly List<EnemyStatsRuntime> activeEnemyStats = new List<EnemyStatsRuntime>();
    private readonly List<Enemy> activeEnemies = new List<Enemy>();

    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //設置靜態資料
    public void SetEnemyStatesDtny(EnemyStatData enemyStatData) {
        enemyStatsTemplateDtny.Clear();
        foreach (var stat in enemyStatData.enemyStatsTemplateList)
        {
            if (enemyStatsTemplateDtny.ContainsKey(stat.StatsData.Id))
            {
                Debug.LogError($"重複的 Enemy ID {stat.StatsData.Id}");
                continue;
            }
            enemyStatsTemplateDtny[stat.StatsData.Id] = stat;
        }
    }
     

    // 生成敵人
    public GameObject SpawnEnemy(int enemyID, Vector3 position, Quaternion rotation, GameObject parentObject) {
        if (!enemyStatsTemplateDtny.TryGetValue(enemyID, out var template) || template.VisualData.CharPrefab == null)
        {
            Debug.LogError($"[EnemyStateManager] 無法生成敵人 {enemyID}，Prefab 為 null");
            return null;
        }

        // 為每個敵人創建一份 Runtime
        var runtime = new EnemyStatsRuntime(template);

        GameObject enemyPrefab = Instantiate(template.VisualData.CharPrefab, position, rotation, parentObject.transform);
        Enemy enemy = enemyPrefab.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Initialize(runtime);
            RegisterEnemy(enemy);
        }
        else
        {
            Debug.LogWarning($"[EnemyStateManager] Prefab {enemyID} 沒有 Enemy 元件");
        }
        return enemyPrefab;
    }

    public void RegisterEnemy(Enemy enemy) {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
            activeEnemyStats.Add(enemy.Runtime);
        }
    }
    public void UnregisterEnemy(Enemy enemy) {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            activeEnemyStats.Remove(enemy.Runtime);
        }
    }
    public EnemyStatsRuntime CreateRuntime(int enemyID) {
        if (!enemyStatsTemplateDtny.TryGetValue(enemyID, out var template)){ Debug.LogError($"找不到 Enemy Template ID {enemyID}");  return null;  }
        return new EnemyStatsRuntime(template);
    }

    // 取靜態資料 (例如 prefab、名稱)
    public EnemyStatsTemplate GetStatsTemplate(int enemyId) {
        if (enemyStatsTemplateDtny.TryGetValue(enemyId, out var template))
            return template;
        Debug.LogError($"找不到 Enemy Template ID {enemyId}");
        return null;
    }
}

