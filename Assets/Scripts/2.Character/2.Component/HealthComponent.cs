using System;
using UnityEngine;

[System.Serializable]
public class HealthComponent
{
    private IHealthData _healthData;
    private StateComponent _stateComponent;

    public event Action<int, int> OnHpChanged;
    public event Action OnDie;


    public HealthComponent(IHealthData healthData,StateComponent stateComponent) {
        _healthData = healthData ?? throw new ArgumentNullException(nameof(healthData));

        _healthData.CurrentHp = _healthData.MaxHp;
        _stateComponent= stateComponent;
        OnHpChanged?.Invoke(_healthData.CurrentHp, _healthData.MaxHp);
    }

    public void TakeDamage(int dmg) {
        if (_stateComponent.IsDead) return; // 死掉就不再處理

        _healthData.CurrentHp = Mathf.Clamp(_healthData.CurrentHp - dmg, 0, _healthData.MaxHp);
        //發事件
        OnHpChanged?.Invoke(_healthData.CurrentHp, _healthData.MaxHp);
        if (_healthData.CurrentHp <= 0)
        {
            _stateComponent.SetIsDead(true);
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
        _stateComponent.SetIsDead(false);
        _healthData.CurrentHp = _healthData.MaxHp;
        //發事件
        OnHpChanged?.Invoke(_healthData.CurrentHp, _healthData.MaxHp);
    }
    //HealthComponent

}
