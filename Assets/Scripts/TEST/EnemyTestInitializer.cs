using UnityEngine;

public class EnemyTestInitializer : MonoBehaviour
{
    public EnemyStatsTemplate EnemyStatsTemplate;
    public EnemyStatsRuntime Rt;

    private void Awake()
    {
        Rt = new EnemyStatsRuntime(EnemyStatsTemplate);
    }

    private void SetEnemys()
    {
        var enemys = FindObjectsOfType<Enemy>();
        foreach (var e in enemys)
        {
            if (e.MoveComponent == null)
            {
                var runner = new CoroutineRunnerAdapter(e);

                e.Initialize(Rt);
            }
            //裝配測試技能
            var skillId = 1;
            if (Rt.SkillPool.TryGetValue(skillId, out var skill))
                e.SkillComponent.EquipSkill(0, skillId, skill.VisualData.DetectPrefab);
        }
    }

    private void Start()
    {
        SetEnemys();
    }
}