using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInputController : MonoBehaviour
{
    public static PlayerInputController Instance { get; private set; }
    public GameObject selectionIndicatorPrefab;  // 選框預製體
    private GameObject currentSelectionIndicator;// 當前選框（標示當前控制的角色）

    public bool isBattleInputEnabled = false;

    private Dictionary<int, GameObject> playersDtny = new Dictionary<int, GameObject>(); // 直接從 PlayerStateManager 取得
    private List<int> playerIDsList = new List<int>(); // 存放目前可用的角色 ID
    private int currentPlayerIndex = -1;  // 當前選擇的角色索引

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update() {
        if (!isBattleInputEnabled) return;
        HandlePlayerSwitch();
    }
    private void FixedUpdate() {
        if (!isBattleInputEnabled) return;
        HandleMovement(); // ✅ 正確用 FixedUpdate 處理剛體移動
    }

    public void InitailPlayerList() {
        UpdatePlayerList(); // 初始化角色列表
        if (playerIDsList.Count > 0)
        {
            SelectPlayer(0);  // 預設選擇第一個角色
            Debug.Log($"PlayerInputController已初始化當前角色控制為{playersDtny[1].name}");
        }
    }

    // 🔹 動態更新玩家列表
    private void UpdatePlayerList() {
        playersDtny = PlayerStateManager.Instance.activePlayersDtny; // 直接引用 Dictionary
        playerIDsList = new List<int>(playersDtny.Keys); // 取得所有可用的角色 ID

        if (playerIDsList.Count == 0)
        {
            Debug.LogWarning("⚠ 目前沒有可控制的角色！");
        }
    }

    // 🔹 處理角色切換
    private void HandlePlayerSwitch() {
        // 數字鍵 1-9 切換角色（根據當前角色數量自適應）
        for (int i = 0; i < playerIDsList.Count && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectPlayer(i);
                return;
            }
        }

        // 滑鼠點擊角色切換
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject()) // 避免點擊 UI
            {
                Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (hit != null)
                {
                    foreach (var kvp in playersDtny)
                    {
                        if (hit.gameObject == kvp.Value)
                        {
                            int index = playerIDsList.IndexOf(kvp.Key);
                            if (index != -1)
                            {
                                SelectPlayer(index);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    // 🔹 選擇角色
    private void SelectPlayer(int index) {
        if (index < 0 || index >= playerIDsList.Count) return;

        if (currentPlayerIndex != index) // 只有當角色變更時才更新
        {
            currentPlayerIndex = index;
            UpdateSelectionIndicator();
        }
    }

    // 🔹 更新選框的位置
    private void UpdateSelectionIndicator() {
        if (!playersDtny.ContainsKey(playerIDsList[currentPlayerIndex]))
        {
            Debug.LogWarning("⚠ 玩家 ID 不存在於 playersDtny！");
            return;
        }
        int playerID = playerIDsList[currentPlayerIndex];
        Player player = PlayerStateManager.Instance.GetPlayerObject(playerID).GetComponent<Player>();

        if (player == null)
        {
            Debug.LogWarning("⚠ 取得的 Player 為 null！");
            return;
        }

        if (player.selectIndicatorPoint == null)
        {
            Debug.LogWarning("⚠ Player 的 selectIndicatorPoint 為 null！");
            return;
        }

        Transform indicatorPoint = player.selectIndicatorPoint.transform;

        if (currentSelectionIndicator == null)
        {
            currentSelectionIndicator = Instantiate(selectionIndicatorPrefab, indicatorPoint);
        }

        currentSelectionIndicator.transform.SetParent(indicatorPoint);
        currentSelectionIndicator.transform.localPosition = Vector3.zero;
    }


    // 🔹 控制當前角色移動
    private void HandleMovement() {
        if (playerIDsList.Count == 0) return; // 確保場上有角色

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;

        if (playersDtny.ContainsKey(playerIDsList[currentPlayerIndex]))
        {
            playersDtny[playerIDsList[currentPlayerIndex]].GetComponent<PlayerMove>().Move(moveDirection);
        }
    }
}
