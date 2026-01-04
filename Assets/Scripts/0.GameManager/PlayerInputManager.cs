using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Linq;

public class PlayerInputManager : MonoBehaviour, IInputProvider {
    public static PlayerInputManager Instance { get; private set; }

    //-------------------------------事件--------------------------------------------------------------
    public event Action<Transform> OnBattlePlayerSelected;
    
    private KeyCode[] _skillKeys = {
    KeyCode.Q,
    KeyCode.W,
    KeyCode.E,
    KeyCode.R
};
    private readonly List<Player> _selectedPlayerList = new List<Player>();
    private bool _canControl = false;

    // 拖曳框選
    public LineRenderer LineRenderer;
    private Vector2 _dragStartPos;
    private bool _isDragging;

    [Header("是否強制開啟手動控制（測試用）")]
    [SerializeField] private bool allowManualControl = false;
    public bool IsTestingController => _canControl == true;

    public void OnPlayerCanControlChanged(bool canControl) => _canControl = canControl;

    public void Awake() {
        // 單例
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LineRenderer.enabled = false;
        if (GameManager.Instance != null)
            GameManager.Instance.GameStateSystem.OnPlayerCanControlChanged += OnPlayerCanControlChanged;
    }

    public void Update() {
        if (allowManualControl) _canControl = true;

        if (!_canControl) return;
        //ClickPlayer();
        //HandleSelectionBox();

        HandleMoveInput();
        HandleSkillInput();
    }

    //-------------------------------玩家點擊選取------------------------------------------------------
    private void ClickPlayer() {
        if (!Input.GetMouseButtonDown(0)) return;
    
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int layerMask = LayerMask.GetMask("Player");
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, layerMask);
    
        if (hit == null) return;
        var player = hit.GetComponentInParent<Player>();
        SelectPlayer(player);
    }
    #region 拖曳框選功能
    private void HandleSelectionBox() {
        if (Input.GetMouseButtonDown(0)) {
            _isDragging = true;
            _dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            LineRenderer.enabled = true;
        }

        if (_isDragging) {
            Vector2 current = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            StretchSelectionBox(_dragStartPos, current);
        }

        if (Input.GetMouseButtonUp(0)) {
            if (_isDragging) {
                _isDragging = false;
                LineRenderer.enabled = false;
                Vector2 end = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                SelectPlayersInBox(_dragStartPos, end);
            }
        }
    }

    private void StretchSelectionBox(Vector2 start, Vector2 end) {
        Vector3[] corners = new Vector3[4];
        corners[0] = new Vector3(start.x, start.y, 0);
        corners[1] = new Vector3(start.x, end.y, 0);
        corners[2] = new Vector3(end.x, end.y, 0);
        corners[3] = new Vector3(end.x, start.y, 0);

        LineRenderer.positionCount = 4;
        LineRenderer.SetPositions(corners);
    }

    private void SelectPlayersInBox(Vector2 start, Vector2 end) {
        ResetAllBattlePlayerIntent();
        _selectedPlayerList.Clear();

        Vector2 min = Vector2.Min(start, end);
        Vector2 max = Vector2.Max(start, end);
        Collider2D[] hits = Physics2D.OverlapAreaAll(min, max, LayerMask.GetMask("Player"));

        foreach (var hit in hits) {
            var p = hit.GetComponentInParent<Player>();
            if (p != null)
                _selectedPlayerList.Add(p);
        }

        foreach (var p in _selectedPlayerList) {
            p.SetInputProvider(this);
            p.SelectIndicator.SetActive(true);
        }

        // 框選後，鏡頭跟隨最接近滑鼠的角色
        if (_selectedPlayerList.Count > 0) {
            Vector2 mouseReleasePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Player nearest = _selectedPlayerList
                .OrderBy(p => Vector2.Distance(mouseReleasePos, p.transform.position))
                .First();

            CameraManager.Instance?.Follow(nearest.transform);
        }
    }
    #endregion

    public void SelectPlayer(Player selectedPlayer) {
        _selectedPlayerList.Clear();
        _selectedPlayerList.Add(selectedPlayer);

        ResetAllBattlePlayerIntent();

        selectedPlayer.SetInputProvider(this);
        selectedPlayer.SelectIndicator.SetActive(true);
        CameraManager.Instance.Follow(selectedPlayer.transform);

        OnBattlePlayerSelected?.Invoke(selectedPlayer.transform);
    }

    //-------------------------------Input 輸入處理-----------------------------------------------------
    private void HandleMoveInput() {
        if (_selectedPlayerList.Count == 0) return;
        //
        //float moveX = Input.GetAxisRaw("Horizontal");
        //float moveY = Input.GetAxisRaw("Vertical");
        //
        //// WASD 移動（方向輸入）
        //if (moveX != 0 || moveY != 0) {
        //    Vector2 dir = new Vector2(moveX, moveY).normalized;
        //
        //    foreach (var player in _selectedPlayerList)
        //        SetIntentMove(player.MoveComponent, direction: dir);
        //
        //    return;
        //}
        //
        //// 若沒有方向輸入，則清空移動意圖（除非有目标位置）
        //foreach (var player in _selectedPlayerList)
        //    if (!player.MoveComponent.IntentTargetPosition.HasValue)
        //        SetIntentMove(player.MoveComponent, direction: Vector2.zero);

        // 滑鼠右鍵 → 設定移動目標位置
        if (Input.GetMouseButtonDown(1)) {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            foreach (var player in _selectedPlayerList)
                SetIntentMove(player.MoveComponent, targetPosition: mouseWorldPos);

            VFXManager.Instance.Play("ClickGround01", mouseWorldPos);
        }
    }


    private void HandleSkillInput() {
        if (_selectedPlayerList.Count == 0) return;
        foreach (var player in _selectedPlayerList) {
            // 限制技能數量為 QWER
            int maxSkillCount = Mathf.Min(_skillKeys.Length,player.SkillComponent.SkillSlots.Length);

            for (int i = 0; i < maxSkillCount; i++) {
                //QWER 技能鍵判斷
                if (!Input.GetKeyDown(_skillKeys[i])) continue;

                var slot = player.SkillComponent.SkillSlots[i];
                if (!slot.HasSkill) {Debug.LogWarning($"技能槽 {i} 沒有技能，無法施放。");return;}

                var detector = slot.Detector;
                if (detector == null) {Debug.LogWarning($"技能槽 {i} 無 DetectStrategy。");return;}

                // 技能目標決定
                Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 targetPosition;
                Transform targetTransform = null;

                if (detector.IsInRange(mouseWorldPos)) {
                    Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, LayerMask.GetMask("Enemy"));

                    if (hit != null) {
                        targetTransform = hit.transform;
                        targetPosition = hit.transform.position;
                    }
                    else {
                        targetPosition = mouseWorldPos;
                    }
                }
                else {
                    targetPosition = detector.GetClosestPoint(mouseWorldPos);
                }

                SetIntentSkill(player.SkillComponent, i, targetPosition, targetTransform);
                break; // 一次只處理一個技能
            }
        }
    }

    //-------------------------------Intent 邏輯---------------------------------------------------------
    private void ResetAllBattlePlayerIntent() {
        if (GameManager.Instance == null) {
            Debug.Log("沒有 GameManager，無法 ResetAllBattlePlayerIntent");
            return;
        }

        foreach (var kvp in PlayerUtility.AllPlayers) {
            var p = kvp.Value;
            p.SetInputProvider(null);
            p.SelectIndicator.SetActive(false);

            SetIntentMove(p.MoveComponent);
            SetIntentSkill(p.SkillComponent, -1);
        }
    }

    public void SetIntentMove(MoveComponent moveComponent, Vector2? direction = null, Vector2? targetPosition = null, Transform targetTransform = null) {
        moveComponent.IntentTargetTransform = targetTransform;
        moveComponent.IntentTargetPosition = targetPosition;
        moveComponent.IntentDirection = direction ?? Vector2.zero;
    }

    public void SetIntentSkill(SkillComponent skillComponent, int slotIndex, Vector2? targetPosition = null, Transform targetTransform = null) {
        skillComponent.IntentSlotIndex = slotIndex;
        skillComponent.IntentTargetTransform = targetTransform;
        skillComponent.IntentTargetPosition = targetPosition ?? Vector2.zero;
    }
}
