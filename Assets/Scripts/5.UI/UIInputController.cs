using UnityEngine;
using System.Collections.Generic;

public class UIInputController : MonoBehaviour
{
    public static UIInputController Instance { get; private set; }
    [Header("#Esc鍵、I鍵開關的對象--------------------------------------------------------------------")]

    public bool isInputEnabled = false;

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update() {
        if (!isInputEnabled) return;

        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (!UIManager.Instance.menuUIPanel.activeSelf)
            {
                UIManager.Instance.OpenUIPanel(UIManager.Instance.menuUIPanel); // ✅ 改成 OpenUIPanel
            }
            else
            {
                UIManager.Instance.CloseTopUIPanel();
            }
        }
    }
}
