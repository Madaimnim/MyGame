using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance { get; private set; }

    [Header("·í«e¸gÅç­È")]
    [SerializeField] private int currentExp = 0;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public int GetCurrentExp() {
        return currentExp;
    }

    public void AddExp(int value) {
        currentExp += value;
    }

    public void ResetExp() {
        currentExp = 0;
    }
}
