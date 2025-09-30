using System;
using UnityEngine;

[System.Serializable]
public class CharHealthComponent
{
    private IHealthData _healthData;

    public bool IsDead { get; private set; }
    //�ƥ�
    public event Action<int, int> OnHpChanged;
    public event Action OnDie;


    public CharHealthComponent(IHealthData healthData) {
        _healthData = healthData ?? throw new ArgumentNullException(nameof(healthData));
        _healthData.CurrentHp = _healthData.MaxHp; // ��l�ƺ���
        IsDead = false;
    }

    public void TakeDamage(int dmg) {
        if (IsDead) return; // �����N���A�B�z

        _healthData.CurrentHp = Mathf.Clamp(_healthData.CurrentHp - dmg, 0, _healthData.MaxHp);
        //�o�ƥ�
        OnHpChanged?.Invoke(_healthData.CurrentHp, _healthData.MaxHp);
        if (_healthData.CurrentHp <= 0)
        {
            IsDead = true;
            //�o�ƥ�
            OnDie?.Invoke();
        }
    }

    public void Heal(int amount) {
        _healthData.CurrentHp = Mathf.Clamp(_healthData.CurrentHp + amount, 0, _healthData.MaxHp);
        //�o�ƥ�
        OnHpChanged?.Invoke(_healthData.CurrentHp, _healthData.MaxHp);
    }

    public void ResetCurrentHp() {
        IsDead = false;
        _healthData.CurrentHp = _healthData.MaxHp;
        //�o�ƥ�
        OnHpChanged?.Invoke(_healthData.CurrentHp, _healthData.MaxHp);
    }

}
