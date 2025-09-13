using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;

public class PlayerInputController : MonoBehaviour
{
    public static PlayerInputController Instance { get; private set; }
    public GameObject selectionIndicatorPrefab;  // 選框預製體
    private GameObject currentSelectionIndicator;// 當前選框（標示當前控制的角色）

    public bool isBattleInputEnabled = false;

    private Dictionary<int, GameObject> deployedPlayersDtny = new Dictionary<int, GameObject>(); // 直接從 PlayerStateManager 取得
    private List<int> deployedPlayerIDsList = new List<int>(); // 存放目前可用的角色 ID
    private int currentPlayerIndex = -1;  // 當前選擇的角色索引

    [SerializeField] private CinemachineVirtualCamera virtualCam;

    //生命週期
    #region 
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
    private void Start() {
        virtualCam = FindObjectOfType<CinemachineVirtualCamera>();
    }

    private void Update() {
        if (!isBattleInputEnabled) return;
        HandlePlayerSwitch();
    }
    private void FixedUpdate() {
        if (!isBattleInputEnabled) return;
        InputMove(); // 正確用 FixedUpdate 處理剛體移動
    }
    #endregion

    //初始化角色清單，並預選第一個角色
    #region InitailPlayerList()
    public void InitailPlayerList() {
        UpdatePlayerList(); // 初始化角色列表
        if (deployedPlayerIDsList.Count > 0)
        {
            SelectPlayer(0);  // 預設選擇第一個角色
            Debug.Log($"PlayerInputController已初始化當前角色控制為{deployedPlayersDtny[1].name}");
        }
    }
    #endregion

    // 更新玩家列表
    #region UpdatePlayerList()
    private void UpdatePlayerList() {
        deployedPlayersDtny = PlayerStateManager.Instance.deployedPlayersDtny; // 直接引用 Dictionary
        deployedPlayerIDsList = new List<int>(deployedPlayersDtny.Keys); // 取得所有可用的角色 ID

        if (deployedPlayerIDsList.Count == 0)
        {
            Debug.LogWarning("目前沒有可控制的角色！");
        }
    }
    #endregion

    // 處理角色切換
    #region  HandlePlayerSwitch()
    // 處理角色切換
    private void HandlePlayerSwitch() {
        // 數字鍵 1-9 切換角色（根據當前角色數量自適應）
        for (int i = 0; i < deployedPlayerIDsList.Count && i < 9; i++)
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
            // 這裡只是 Debug UI 狀態，不再 return
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("點擊到 UI");
            }

            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 只檢測 Player 層
            int layerMask = LayerMask.GetMask("Player");

            // 用 OverlapPointAll 抓取所有 Collider
            Collider2D[] hits = Physics2D.OverlapPointAll(mouseWorldPos, layerMask);

            if (hits.Length == 0)
            {
                //Debug.Log($"沒有命中任何 Player 層物件");
            }
            else
            {
                foreach (var hit in hits)
                {
                    Debug.Log($"命中物件: {hit.gameObject.name}, Layer: {LayerMask.LayerToName(hit.gameObject.layer)}");

                    // 嘗試匹配 deployedPlayersDtny
                    foreach (var kvp in deployedPlayersDtny)
                    {
                        if (hit.gameObject == kvp.Value)
                        {
                            int index = deployedPlayerIDsList.IndexOf(kvp.Key);
                            if (index != -1)
                            {
                                SelectPlayer(index);
                            }
                        }
                    }
                }
            }
        }
    }
    #endregion

    // 選擇角色
    #region SelectPlayer(int index)
    private void SelectPlayer(int index) {
        if (index < 0 || index >= deployedPlayerIDsList.Count) return;

        if (currentPlayerIndex != index) // 只有當角色變更時才更新
        {
            currentPlayerIndex = index;
            UpdateSelectionIndicator();
        }

        // [新增] 設定 Cinemachine Camera 的跟隨目標
        int playerID = deployedPlayerIDsList[currentPlayerIndex];
        if (deployedPlayersDtny.ContainsKey(playerID))
        {
            GameObject currentPlayer = deployedPlayersDtny[playerID];
            CameraManager.Instance.Follow(currentPlayer.transform);
        }

    }
    #endregion

    // 更新選框的位置
    #region UpdateSelectionIndicator()
    private void UpdateSelectionIndicator() {
        if (!deployedPlayersDtny.ContainsKey(deployedPlayerIDsList[currentPlayerIndex]))
        {
            Debug.LogWarning("玩家 ID 不存在於 deployedPlayersDtny！");
            return;
        }
        int playerID = deployedPlayerIDsList[currentPlayerIndex];
        Player player = PlayerStateManager.Instance.GetDeployedPlayerObject(playerID).GetComponent<Player>();

        if (player == null)
        {
            Debug.LogWarning("取得的 Player 為 null！");
            return;
        }

        if (player.selectIndicatorPoint == null)
        {
            Debug.LogWarning("Player 的 selectIndicatorPoint 為 null！");
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
    #endregion

    //  控制當前角色移動
    #region InputMove()
    private void InputMove() {
        if (deployedPlayerIDsList.Count == 0) return; // 確保場上有角色


        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;

        int currentId = deployedPlayerIDsList[currentPlayerIndex];
        if (deployedPlayersDtny.ContainsKey(deployedPlayerIDsList[currentPlayerIndex]))
        {
            var currentPlayerObject = deployedPlayersDtny[currentId];
            var player = currentPlayerObject.GetComponent<Player>();

            if (player.isDead) return;

            if (player != null)
            {
                player.TryMove(moveDirection);
            }
            else
                Debug.Log("角色身上沒有PlayerMove");



            if (Mathf.Abs(moveX) > 0.01f)
            {
                Transform t = currentPlayerObject.transform;
                Vector3 s = t.localScale;
                float mag = Mathf.Abs(s.x);                  // 保留原本的絕對大小
                s.x = (moveX < 0f) ? -mag : mag;             // 左負右正
                t.localScale = s;
            }
        }
    }
    #endregion
}
