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

    private SkillCastMode _skillCastMode;
    private Player _holdingPlayer;
    private int _holdingSlotIndex;

    // 拖曳框選
    public LineRenderer LineRenderer;
    private Vector2 _dragStartPos;
    private bool _isDragging;

    //顯示outline狀態的對象
    private Enemy _hoverEnemy;
    private Enemy _dangerEnemy;

    public void OnPlayerCanControlChanged(bool canControl) => _canControl = canControl;
    private bool TryGetSkillSlotFromKey(KeyCode key, out int slotIndex) {
        slotIndex = -1;
        switch (key) {
            case KeyCode.Q: slotIndex = 1; return true;
            case KeyCode.W: slotIndex = 2; return true;
            case KeyCode.E: slotIndex = 3; return true;
            case KeyCode.R: slotIndex = 4; return true;
            default: return false;
        }
    }
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
        if (GameSettingManager.Instance != null) _skillCastMode=GameSettingManager.Instance.SkillCastMode;
    }

    public void Update() {
        if (!_canControl) return;
        //ClickPlayer();
        //HandleSelectionBox();
        UpdateHoverEnemy();   // 新增

        HandleMoveInput();
        HandleSkillInput();
    }

    //=================================玩家點擊選取======================================
    #region 腳色選擇
    private void ClickPlayer() {
        if (!Input.GetMouseButtonDown(0)) return;
    
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int layerMask = LayerMask.GetMask("Player");
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, layerMask);
    
        if (hit == null) return;
        var player = hit.GetComponentInParent<Player>();
        SelectPlayer(player);
    }
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
    //===============================Input 輸入處理========================================
    private void HandleMoveInput() {
        if (_selectedPlayerList.Count == 0) return;

        if (Input.GetMouseButtonDown(1)) {
            foreach (var player in _selectedPlayerList) {
                HandleRightClick(player);
            }
        }
    }
    private void HandleRightClick(Player player) {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var skillComp = player.SkillComponent;
        if (skillComp.SkillSlots.Length == 0) return;

        var slot = skillComp.SkillSlots[0]; // 普攻擊能槽 = Slot0

        // 沒技能→移動
        if (!slot.HasSkill || slot.Detector == null) {         
            SetIntentMove(player.MoveComponent, targetPosition: mouseWorldPos);
            VFXManager.Instance.Play("ClickGround01", mouseWorldPos);
            return;
        }

        // 有點到敵人->判斷距離
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, LayerMask.GetMask("Enemy"));
        Enemy enemy = hit ? hit.GetComponentInParent<Enemy>() : null;
        if (enemy != null) {

            // 清舊的 Danger
            if (_dangerEnemy != null && _dangerEnemy != enemy)_dangerEnemy.EffectComponent.HideOutline(); 
            _dangerEnemy = enemy;
            _dangerEnemy.EffectComponent.ShowTargetOutline();


            Vector2 enemyGroundPos = enemy.transform.position;
            skillComp.SetContinuousAttack(true);
            SetIntentSkill(skillComp, 0, enemyGroundPos, enemy.transform);
            return;
        }

        // 其他情況→移動
        skillComp.SetContinuousAttack(false);
        ClearTargetOutline();
        SetIntentSkill(skillComp, -1);
        SetIntentMove(player.MoveComponent, targetPosition: mouseWorldPos);
        VFXManager.Instance.Play("ClickGround01", mouseWorldPos);
    }

    #region 鍵盤施放技能
    //對每顆_skillKeys裡的按鍵
    private void HandleSkillInput() {
        if (_selectedPlayerList.Count == 0) return;

        foreach (var player in _selectedPlayerList) {
            foreach (KeyCode key in _skillKeys) {
                if (!TryGetSkillSlotFromKey(key, out int slotIndex)) continue;
                if (slotIndex >= player.SkillComponent.SkillSlots.Length) continue;

                var slot = player.SkillComponent.SkillSlots[slotIndex];
                if (!slot.HasSkill || slot.Detector == null) continue;

                switch (_skillCastMode) {
                    case SkillCastMode.Instant:
                        HandleInstantCast(key, player, slotIndex);
                        break;

                    case SkillCastMode.HoldRelease:
                        HandleHoldReleaseCast(key, player, slotIndex);
                        break;
                }
            }
        }
    }
    private void HandleInstantCast(KeyCode key,Player player,int slotIndex) {
        if (!Input.GetKeyDown(key)) return;
        player.SkillComponent.SetContinuousAttack(false);//任何按鍵攻擊，關閉連續攻擊
        ResolveAndCastSkill(player, slotIndex);
    }
    private void HandleHoldReleaseCast(KeyCode key, Player player, int slotIndex) {
        var skillComp = player.SkillComponent;

        //KeyDown：開始 Hold ----------
        if (Input.GetKeyDown(key)) {
            // 若已有其他技能在 Hold → 強制取消
            if (_holdingPlayer != null &&
                (_holdingPlayer != player || _holdingSlotIndex != slotIndex)) {
                CancelCurrentHold();
            }

            _holdingPlayer = player;
            _holdingSlotIndex = slotIndex;

            skillComp.SetDetectRangeVisible(slotIndex, true);
            return;
        }

        // KeyUp：只有持有者才能施放 ----------
        if (Input.GetKeyUp(key)) {
            if (_holdingPlayer != player || _holdingSlotIndex != slotIndex)
                return;

            skillComp.SetDetectRangeVisible(slotIndex, false);
            player.SkillComponent.SetContinuousAttack(false);//任何按鍵攻擊，關閉連續攻擊
            ResolveAndCastSkill(player, slotIndex);

            _holdingPlayer = null;
            _holdingSlotIndex = -1;
        }
    }

    //鍵盤施放
    private void ResolveAndCastSkill(Player player, int slotIndex) {
        var slot = player.SkillComponent.SkillSlots[slotIndex];
        var detector = slot.Detector;

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, LayerMask.GetMask("Enemy"));
        Enemy enemy = null;
        if (hit != null) {
            enemy = hit.GetComponentInParent<Enemy>();
        }

        Vector2 targetPosition;
        Transform targetTransform = null;

        if (enemy != null) {
            Vector2 enemyGroundPos = enemy.transform.position;

            if (detector.IsInRange(enemyGroundPos)) {
                targetTransform = enemy.transform;
                targetPosition = enemyGroundPos;
            }
            else {
                targetPosition = detector.GetClosestPoint(mouseWorldPos);
            }

            SetIntentSkill(player.SkillComponent, slotIndex, targetPosition, targetTransform);
        }
        else {
            // 沒點到敵人，用滑鼠位置(在範圍內用畫鼠位置、範圍外取最近點)
            if (detector.IsInRange(mouseWorldPos)) targetPosition = mouseWorldPos;
            else targetPosition = detector.GetClosestPoint(mouseWorldPos);
            SetIntentSkill(player.SkillComponent, slotIndex, targetPosition, targetTransform);
        }
    }

    //取消上一個Hold住的按鍵
    private void CancelCurrentHold() {
        if (_holdingPlayer == null) return;

        var skillComp = _holdingPlayer.SkillComponent;
        skillComp.SetDetectRangeVisible(_holdingSlotIndex, false);

        _holdingPlayer = null;
        _holdingSlotIndex = -1;
    }
    #endregion

    //===================================Intent 邏輯==================================
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
            p.SkillComponent.SetContinuousAttack(false);//任何按鍵攻擊，關閉連續攻擊
        }
        ClearTargetOutline();
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

    //===============================Outline處理==============================
    private void UpdateHoverEnemy() {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, LayerMask.GetMask("Enemy"));
        Enemy newHover = hit ? hit.GetComponentInParent<Enemy>() : null;

        // 如果 Hover 對象沒變 → 不做事
        if (_hoverEnemy == newHover) return;

        // 舊 Hover 要關掉（但 Danger 不動）
        if (_hoverEnemy != null && _hoverEnemy != _dangerEnemy) {
            _hoverEnemy.EffectComponent.HideOutline();
        }

        _hoverEnemy = newHover;

        // 新 Hover 顯示（但 Danger 優先）
        if (_hoverEnemy != null && _hoverEnemy != _dangerEnemy) {
            _hoverEnemy.EffectComponent.ShowHoverOutline();
        }
    }
    private void ClearTargetOutline() {
        if (_dangerEnemy != null) {
            _dangerEnemy.EffectComponent.HideOutline();
            _dangerEnemy = null;
        }
    }
}
