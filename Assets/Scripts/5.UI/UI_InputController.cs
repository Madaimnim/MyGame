using UnityEngine;
using System.Collections.Generic;
using System;

public class UI_InputController: MonoBehaviour
{
    [Header("是否啟用 UI 鍵盤輸入")]
    private bool IsUIInputEnabled = false;
    //事件
    public event Action OnToggleMenuButton;

    private void Awake() {}

    private void Update() {
        if (!IsUIInputEnabled) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log($"Escape detected! listener count = {OnToggleMenuButton?.GetInvocationList().Length}");
            OnToggleMenuButton?.Invoke();
        }

    }

    public void EnableUIInput(bool enable) {
        IsUIInputEnabled = enable;
    }
}
