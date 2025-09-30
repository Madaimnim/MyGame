using System;
using UnityEngine;

public class CharExpComponent
{
    private PlayerStatsRuntime _rt;

    public event Action<int, int> OnExpChanged;
    public event Action<int> OnLevelUp;
    public event Action<int> OnExpGained;

    public int Exp => _rt.Exp;
    public int Level => _rt.StatsData.Level;

    public CharExpComponent(PlayerStatsRuntime expData) {
        _rt = expData ?? throw new ArgumentNullException(nameof(expData));
        if (_rt.ExpTable == null || _rt.ExpTable.Length == 0) Debug.LogWarning("ExpTable尚未初始化");
    }

    public void AddExp(int amount) {
        if (amount <= 0) return;
        _rt.Exp+= amount;
        //發事件
        OnExpGained?.Invoke(amount);

        while (_rt.StatsData.Level < _rt.ExpTable.Length &&
               _rt.Exp >= _rt.ExpTable[_rt.StatsData.Level])
        {
            _rt.Exp -= _rt.ExpTable[_rt.StatsData.Level];
            _rt.StatsData.Level++;
            //發事件
            OnLevelUp?.Invoke(_rt.StatsData.Level);
        }

        int expToNext = (_rt.StatsData.Level < _rt.ExpTable.Length)
            ? _rt.ExpTable[_rt.StatsData.Level] : int.MaxValue;
        //發事件
        OnExpChanged?.Invoke(_rt.Exp, expToNext);
    }


    public void ResetExp() {
        _rt.Exp = 0;
        _rt.StatsData.Level = 1;
        //發事件
        OnExpChanged?.Invoke(0, _rt.ExpTable[0]);
    }

}
