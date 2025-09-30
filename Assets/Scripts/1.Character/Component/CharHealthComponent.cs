using System;
using UnityEngine;

[System.Serializable]
public class CharHealthComponent
{
    private IHealthData _healthData;

    public bool IsDead { get; private set; }
    //事件
    public event Action<int, int> OnHpChanged;
    public event Action OnDie;


    public CharHealthComponent(IHealthData healthData) {
        _healthData = healthData ?? throw new ArgumentNullException(nameof(healthData));
        _healthData.CurrentHp = _healthData.MaxHp; // 初始化滿血
        IsDead = false;
    }

    public void TakeDamage(int dmg) {
        if (IsDead) return; // 死掉就不再處理

        _healthData.CurrentHp = Mathf.Clamp(_healthData.CurrentHp - dmg, 0, _healthData.MaxHp);
        //發事件
        OnHpChanged?.Invoke(_healthData.CurrentHp, _healthData.MaxHp);
        if (_healthData.CurrentHp <= 0)
        {
            IsDead = true;
            //發事件
            OnDie?.Invoke();
        }
    }

    public void Heal(int amount) {
        _healthData.CurrentHp = Mathf.Clamp(_healthData.CurrentHp + amount, 0, _healthData.MaxHp);
        //發事件
        OnHpChanged?.Invoke(_healthData.CurrentHp, _healthData.MaxHp);
    }

    public void ResetCurrentHp() {
        IsDead = false;
        _healthData.CurrentHp = _healthData.MaxHp;
        //發事件
        OnHpChanged?.Invoke(_healthData.CurrentHp, _healthData.MaxHp);
    }

}
