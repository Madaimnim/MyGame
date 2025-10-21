using UnityEngine;

public class PlayerTestInitializer : MonoBehaviour
{
    public PlayerStatsTemplate PlayerStatsTemplate;
    public PlayerStatsRuntime Rt;

    private void Awake()
    {
        Rt = new PlayerStatsRuntime(PlayerStatsTemplate);
    }

    private void SetPlayers()
    {
        var players = FindObjectsOfType<Player>();
        foreach (var p in players)
        {
            var runner = new CoroutineRunnerAdapter(p);

            p.Initialize(Rt);

            //裝配測試技能
            var skillId = 1;
            if (Rt.SkillPool.TryGetValue(skillId, out var skill))
            {
                p.SkillComponent.EquipSkill(0, skillId, skill.VisualData.DetectPrefab);
            }

        }
    }

    private void Start()
    {
        SetPlayers();
    }
}