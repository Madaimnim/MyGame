using System;
using UnityEngine;

public class CharExpComponent
{
    private IExpData _expData;

    public event Action<int, int> OnExpChanged;
    public event Action<int> OnLevelUp;
    public event Action<int> OnExpGained;

    public int CurrentExp => _expData.CurrentExp;
    public int CurrentLevel => _expData.CurrentLevel;

    public CharExpComponent(IExpData expData) {
        _expData = expData ?? throw new ArgumentNullException(nameof(expData));
        if (_expData.ExpTable == null || _expData.ExpTable.Length == 0) Debug.LogWarning("ExpTable尚未初始化");
    }

    public void AddExp(int amount) {
        if (amount <= 0) return;
        _expData.CurrentExp+= amount;
        //發事件
        OnExpGained?.Invoke(amount);

        while (_expData.CurrentLevel < _expData.ExpTable.Length &&
               _expData.CurrentExp >= _expData.ExpTable[_expData.CurrentLevel])
        {
            _expData.CurrentExp -= _expData.ExpTable[_expData.CurrentLevel];
            _expData.CurrentLevel++;
            //發事件
            OnLevelUp?.Invoke(_expData.CurrentLevel);
        }

        int expToNext = (_expData.CurrentLevel < _expData.ExpTable.Length)
            ? _expData.ExpTable[_expData.CurrentLevel] : int.MaxValue;
        //發事件
        OnExpChanged?.Invoke(_expData.CurrentExp, expToNext);
    }


    public void ResetExp() {
        _expData.CurrentExp = 0;
        _expData.CurrentLevel = 1;
        //發事件
        OnExpChanged?.Invoke(0, _expData.ExpTable[0]);
    }

}
