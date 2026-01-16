using System;
using UnityEngine;

public class EnergyComponent {
    public float CurrentEnergy { get; private set; }
    public float MaxEnergy { get; private set; }

    //UI 每次刷新用
    public event Action<float, float> OnEnergyChanged;

    public EnergyComponent(float maxEnergy = 3, float initialEnergy = 0f) {
        MaxEnergy = maxEnergy;
        CurrentEnergy = Mathf.Clamp(initialEnergy, 0, MaxEnergy);
    }

    public bool HasEnoughEnergy(float cost) {
        return CurrentEnergy >= cost;
    }
    public void Consume(float cost) {
        if (cost <= 0) return;

        CurrentEnergy = Mathf.Max(0, CurrentEnergy - cost);

        OnEnergyChanged?.Invoke(CurrentEnergy, MaxEnergy);
    }
    public void Gain(float amount) {
        if (amount <= 0f) return;

        CurrentEnergy = Mathf.Min(MaxEnergy, CurrentEnergy + amount);
        OnEnergyChanged?.Invoke(CurrentEnergy, MaxEnergy);
    }

    // ===== Debug / Reset =====
    public void Set(float value) {
        CurrentEnergy = Mathf.Clamp(value, 0, MaxEnergy);

        OnEnergyChanged?.Invoke(CurrentEnergy, MaxEnergy);
    }
}
