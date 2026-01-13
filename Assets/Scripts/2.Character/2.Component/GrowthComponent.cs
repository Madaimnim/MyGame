using System;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GrowthComponent
{
    public PlayerStatsRuntime _rt;
    private StatsComponent _statsComponent;

    public event Action<int> OnLevelUp;
    public event Action<int> OnExpGained;

    public int Exp => _rt.Exp;
    public int Level => _rt.Level;

    public GrowthComponent(PlayerStatsRuntime rt,StatsComponent statsComponent) {
        _rt = rt ?? throw new ArgumentNullException(nameof(rt));
        if (_rt.ExpTable == null || _rt.ExpTable.Length == 0) Debug.LogWarning("ExpTable尚未初始化");
        _statsComponent = statsComponent;
    }

    public void AddExp(int amount) {
        if (amount <= 0) return;

        _rt.Exp += amount;
        OnExpGained?.Invoke(amount);

        while (_rt.Level < _rt.ExpTable.Length &&_rt.Exp >= GetExpToNextLevel()) {
            _rt.Exp -= GetExpToNextLevel();
            _rt.Level++;
            LevelUp();
        }

        Debug.Log($"{_rt.Name} EXP +{amount} | " +$"Lv:{_rt.Level} | " +$"Exp:{_rt.Exp}/{GetExpToNextLevel()}");
    }


    private int GetExpToNextLevel() {
        if (_rt.Level >= _rt.ExpTable.Length) return int.MaxValue;

        return _rt.ExpTable[_rt.Level];
    }

    private void LevelUp() {
        OnLevelUp?.Invoke(_rt.Level);
        _statsComponent.BonusStats.Power += 1;
        // 加完一定要算一次
        _statsComponent.RecalculateFinalStats();
        //Debug.Log($"{_rt.Name} 升級了！目前等級:{_rt.Level},當前EXP:{_rt.Exp}");
    }

    private bool AddSkillUsageCount(PlayerSkillRuntime skill) {
        skill.SkillUsageCount++;
        if (skill.SkillUsageCount >= skill.NextLevelCount) return true;
        else return false;
    }
    private void SkillLevelUp(PlayerSkillRuntime skill) {
        skill.StatsData.Power++;
        skill.SkillLevel++;
        skill.NextLevelCount += skill.SkillLevel * 10;
    }
}
