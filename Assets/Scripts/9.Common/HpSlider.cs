using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HpSlider : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text text;
    [SerializeField] private string title = "HP";
    private HealthComponent _health;   // 唯一依賴

    public void Bind(HealthComponent health) {
        Unbind();

        _health = health;
        if (_health == null) return;

        // 訂閱事件
        _health.OnHpChanged += OnHpChanged;
        _health.OnDie += OnDieDoSomething;
    }

    public void Unbind() {
        if (_health == null) return;

        _health.OnHpChanged -= OnHpChanged;
        _health.OnDie -= OnDieDoSomething;
        _health = null;
    }

    private void OnDestroy() {
        Unbind();
    }


    private void OnHpChanged(int current, int max) {
        UpdateUI(current, max);
    }

    private void OnDieDoSomething() {
        // 可選：死亡時隱藏
    }


    private void UpdateUI(int current, int max) {
        if (slider != null) {
            slider.maxValue = max;
            slider.value = current;
        }

        if (text != null) {
            text.text = $"{title}: {current} / {max}";
        }
    }
}
