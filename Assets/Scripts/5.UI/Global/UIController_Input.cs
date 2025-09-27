using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-50)]
public class UIController_Input : MonoBehaviour
{
    public static UIController_Input Instance { get; private set; }
    [Header("#Esc鍵、I鍵開關的對象--------------------------------------------------------------------")]

    public bool isUIInputEnabled = false;

    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject); 掛在主物件才需要
    }

    private void Update() {
        if (!isUIInputEnabled) return;

        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("按下了I鍵或Ese鍵");
            if (!UIManager.Instance.menuUIPanel.activeSelf)
            {
                Debug.Log("執行開啟菜單");
                UIManager.Instance.OpenUIPanel(UIManager.Instance.menuUIPanel); // OpenUIPanel
            }
            else
            {
                UIManager.Instance.CloseTopUIPanel();
            }
        }
    }
}
