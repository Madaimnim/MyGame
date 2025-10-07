using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using System.Linq;

public class EnemyStateSystem :SubSystemBase
{
    public IReadOnlyDictionary<int, EnemyStatsTemplate> EnemyStatsTemplateDtny => _enemyStatsTemplateDtny;
    public IReadOnlyCollection<Enemy> BattleEnemyList => _battleEnemyList;
    public IReadOnlyCollection<int> UnlockedIdList => _unlockedIdList;
    public List<EnemyStatsRuntime> EnemyStatsRuntimeList { get; private set; } = new List<EnemyStatsRuntime>();
    public EnemySkillSystem SkillSystem { get; private set; }
    public EnemySpawnSystem SpawnSystem { get; private set; }

    // 事件
    public event Action<int> OnEnemyUnlocked;


    private readonly Dictionary<int, EnemyStatsTemplate> _enemyStatsTemplateDtny = new();
    private readonly List<Enemy> _battleEnemyList = new List<Enemy>();
    private readonly HashSet<int> _unlockedIdList = new();

    public EnemyStateSystem(GameManager gm) : base(gm) { }

    public override void Initialize() {
        SpawnSystem = new EnemySpawnSystem(
               new DefaultEnemyFactory(),
               new DropBounceSpawnEffect(),
               new CoroutineRunnerAdapter(GameManager)
           );
        SkillSystem = new EnemySkillSystem();
    }

    public Enemy SpawnEnemy(int id, Vector3 spawnPos) => SpawnSystem.SpawnEnemy(id, spawnPos);



    public void UnlockEnemy(int id) {
        if (!IDValidator.IsEnemyID(id)) {   Debug.LogWarning($"EnemyID不合法"); return; }
        _unlockedIdList.Add(id);

        //發事件
        OnEnemyUnlocked?.Invoke(id);
    }


    public void RegisterEnemy(Enemy enemy) {
        if (!_battleEnemyList.Contains(enemy)) _battleEnemyList.Add(enemy);
    }
    public void UnregisterEnemy(Enemy enemy) {
            if (_battleEnemyList.Contains(enemy)) _battleEnemyList.Remove(enemy);
    }

    public void SetEnemyStatesTemplateDtny(EnemyStatData enemyStatData) {
        _enemyStatsTemplateDtny.Clear();
        foreach (var stat in enemyStatData.enemyStatsTemplateList)
        {
            if (!IDValidator.IsEnemyID(stat.StatsData.Id)) { Debug.LogWarning($"EnemyID不合法: {stat.StatsData.Id}"); continue; }
            if (_enemyStatsTemplateDtny.ContainsKey(stat.StatsData.Id)){Debug.LogWarning($"重複的EnemyID:{stat.StatsData.Id}");continue;}
            
            _enemyStatsTemplateDtny[stat.StatsData.Id] = stat;
        }
    }
}

