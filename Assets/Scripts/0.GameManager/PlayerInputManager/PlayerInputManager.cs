using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Linq;


public class PlayerInputManager : MonoBehaviour {
    public static PlayerInputManager Instance { get; private set; }

    //-------------------------------事件--------------------------------------------------------------
    public event Action<Transform> OnBattlePlayerSelected;
    public Player CurrentControlPlayer;

    private KeyCode[] _skillKeys = {
    KeyCode.Q,
    KeyCode.W,
    KeyCode.E,
    KeyCode.R
    };

    private readonly List<Player> _selectedPlayerList = new List<Player>();
    public bool CanControl { get; private set; } = false;
    public void SetCanControl(bool value)=> CanControl= value;

    private SkillCastMode _skillCastMode;
    private Player _holdingPlayer;
    private int _holdingSlotNumber;

    // 拖曳框選
    public LineRenderer LineRenderer;
    private Vector2 _dragStartPos;
    private bool _isDragging;


    //自動攻擊子狀態

    private bool _isPreparingAutoAttackMoveInput = false;

    private bool TryGetSkillSlotFromKey(KeyCode key, out int slotNumber) {
        slotNumber = -1;
        switch (key) {
            case KeyCode.Q: slotNumber = 1; return true;
            case KeyCode.W: slotNumber = 2; return true;
            case KeyCode.E: slotNumber = 3; return true;
            case KeyCode.R: slotNumber = 4; return true;
            default: return false;
        }
    }
    public void Awake() {
        // 單例
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }
        Instance = this;

        LineRenderer.enabled = false;
        if (GameSettingManager.Instance != null) _skillCastMode=GameSettingManager.Instance.SkillCastMode;
        GameEventSystem.Instance.Event_BattleStart += () => SetCanControl(true);


    }

    public void Update() {
        if (!CanControl) return;
        //ClickPlayer();
        //HandleSelectionBox();
        UpdateHoverEnemy();   // 新增

        HandleRightClick();
        if (_isPreparingAutoAttackMoveInput) {
            HandleAutoAttackMoveConfirm();
            return; // 準備狀態下，不吃其他輸入
        }

        HandleAutoAttackMoveKey();
        HandleKeyBoardInput();
    }

    //=================================玩家點擊選取======================================
    #region 腳色選擇
    //private void ClickPlayer() {
    //    if (!Input.GetMouseButtonDown(0)) return;
    //
    //    Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    int layerMask = LayerMask.GetMask("Player");
    //    Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, layerMask);
    //
    //    if (hit == null) return;
    //    var player = hit.GetComponentInParent<Player>();
    //    SelectPlayer(player);
    //}
    //private void HandleSelectionBox() {
    //    if (Input.GetMouseButtonDown(0)) {
    //        _isDragging = true;
    //        _dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        LineRenderer.enabled = true;
    //    }
    //
    //    if (_isDragging) {
    //        Vector2 current = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        StretchSelectionBox(_dragStartPos, current);
    //    }
    //
    //    if (Input.GetMouseButtonUp(0)) {
    //        if (_isDragging) {
    //            _isDragging = false;
    //            LineRenderer.enabled = false;
    //            Vector2 end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //
    //            SelectPlayersInBox(_dragStartPos, end);
    //        }
    //    }
    //}
    //
    //private void StretchSelectionBox(Vector2 start, Vector2 end) {
    //    Vector3[] corners = new Vector3[4];
    //    corners[0] = new Vector3(start.x, start.y, 0);
    //    corners[1] = new Vector3(start.x, end.y, 0);
    //    corners[2] = new Vector3(end.x, end.y, 0);
    //    corners[3] = new Vector3(end.x, start.y, 0);
    //
    //    LineRenderer.positionCount = 4;
    //    LineRenderer.SetPositions(corners);
    //}
    //
    //private void SelectPlayersInBox(Vector2 start, Vector2 end) {
    //    ResetAllBattlePlayerIntent();
    //    _selectedPlayerList.Clear();
    //
    //    Vector2 min = Vector2.Min(start, end);
    //    Vector2 max = Vector2.Max(start, end);
    //    Collider2D[] hits = Physics2D.OverlapAreaAll(min, max, LayerMask.GetMask("Player"));
    //
    //    foreach (var hit in hits) {
    //        var p = hit.GetComponentInParent<Player>();
    //        if (p != null)
    //            _selectedPlayerList.Add(p);
    //    }
    //
    //    foreach (var p in _selectedPlayerList) {
    //        p.SetInputProvider(this);
    //        p.SelectIndicator.SetActive(true);
    //    }
    //
    //    // 框選後，鏡頭跟隨最接近滑鼠的角色
    //    if (_selectedPlayerList.Count > 0) {
    //        Vector2 mouseReleasePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //
    //        Player nearest = _selectedPlayerList
    //            .OrderBy(p => Vector2.Distance(mouseReleasePos, p.transform.position))
    //            .First();
    //
    //        CameraManager.Instance?.Follow(nearest.transform);
    //    }
    //}
    #endregion

    public void SelectPlayer(Player selectedPlayer) {
        _selectedPlayerList.Clear();
        _selectedPlayerList.Add(selectedPlayer);

        ResetAllBattlePlayerIntent();

        //selectedPlayer.SelectIndicator.SetActive(true);       //先不用
        CameraManager.Instance.Follow(selectedPlayer.transform);

        CurrentControlPlayer = selectedPlayer;
        OnBattlePlayerSelected?.Invoke(selectedPlayer.transform);
    }

    //右鍵
    private void HandleRightClick() {
        if (_selectedPlayerList.Count == 0) return;
        if (!Input.GetMouseButtonDown(1)) return;
        var ai = CurrentControlPlayer.AIComponent;
        ai.StopAutoMoveAttack();

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var combatComp = CurrentControlPlayer.CombatComponent;
        var moveComp = CurrentControlPlayer.MoveComponent;
        var stateComp = CurrentControlPlayer.StateComponent;
        var aniComp= CurrentControlPlayer.AnimationComponent;
        var slot = combatComp.SkillSlots[0]; // 普攻擊能槽 = Slot0


        // 有點到敵人
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, LayerMask.GetMask("Enemy"));
        Enemy enemy = hit ? hit.GetComponentInParent<Enemy>() : null;
        if (enemy != null) {
            combatComp.SetIntentBaseAttack(enemy.transform);
            EnemyOutlineManager.Instance.SetTarget(enemy);
            return;
        }

        // 其他情況→移動
        if (stateComp.IsCastingSkill) combatComp.SpawnSkill();
        //if (stateComp.IsBaseAttacking) combatComp.SpawnBaseAttack();

        if(!CurrentControlPlayer.StateComponent.IsDead)
            aniComp.PlayIdle();
        EnemyOutlineManager.Instance.ClearTarget();
        combatComp.ClearBaseAttackTargetTransform();
        combatComp.ClearSkillIntent();

        moveComp.SetIntentMovePosition( inputPosition: mouseWorldPos);
        VFXManager.Instance.Play("ClickGround01", mouseWorldPos);
    }
    //鍵盤A
    private void HandleAutoAttackMoveKey() {
        if (!Input.GetKeyDown(KeyCode.A)) return;
        if (CurrentControlPlayer == null) return;


        var combat = CurrentControlPlayer.CombatComponent;

        // 進入準備狀態
        _isPreparingAutoAttackMoveInput = true;
        combat.SetBaseAttackDetectRangeVisible(true);
    }
    //左鍵確定 右鍵取消
    private void HandleAutoAttackMoveConfirm() {
        var player = CurrentControlPlayer;
        var combat = player.CombatComponent;
        var ai = player.AIComponent;

        // 左鍵 → 確認執行自動移動攻擊
        if (Input.GetMouseButtonDown(0)) {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 關閉普攻顯示
            combat.SetBaseAttackDetectRangeVisible(false);

            // 啟動 AI 行為
            ai.StartAutoMoveAttack(
                EnemyListManager.Instance.TargetList,
                mouseWorldPos,
                player.Rt.BaseAttackRuntime
            );

            VFXManager.Instance.Play("ClickGround01", mouseWorldPos);
            _isPreparingAutoAttackMoveInput = false;
            return;
        }

        // 右鍵 → 取消
        if (Input.GetMouseButtonDown(1)) {
            combat.SetBaseAttackDetectRangeVisible(false);
            _isPreparingAutoAttackMoveInput = false;
        }
    }

    //鍵盤QWER
    #region 鍵盤施放技能
    //========================對每顆_skillKeys裡的按鍵=================================
    private void HandleKeyBoardInput() {
        if (_selectedPlayerList.Count == 0) return;

        foreach (var player in _selectedPlayerList) {
            foreach (KeyCode key in _skillKeys) {
                if (!TryGetSkillSlotFromKey(key, out int slotNumber)) continue;
                if (slotNumber < 1 || slotNumber > player.CombatComponent.SkillSlots.Length) continue;

                var slot = player.CombatComponent.SkillSlots[slotNumber-1];
                if (!slot.HasSkill || slot.Detector == null) continue;

                switch (_skillCastMode) {
                    case SkillCastMode.Instant:
                        HandleInstantCast(key, player, slotNumber);
                        break;

                    case SkillCastMode.HoldRelease:
                        HandleHoldReleaseCast(key, player, slotNumber);
                        break;
                }
            }
        }
    }
    private void HandleInstantCast(KeyCode key,Player player,int slotNumber) {
        if (!Input.GetKeyDown(key)) return;

        ResolveAndCastSkill(player, slotNumber);
    }
    private void HandleHoldReleaseCast(KeyCode key, Player player, int slotNumber) {
        var combatComponent = player.CombatComponent;

        //KeyDown：開始 Hold ----------
        if (Input.GetKeyDown(key)) {

            // 若已有其他技能在 Hold → 強制取消
            if (_holdingPlayer != null &&
                (_holdingPlayer != player || _holdingSlotNumber != slotNumber)) {
                CancelCurrentHold();
            }

            _holdingPlayer = player;
            _holdingSlotNumber = slotNumber;

            combatComponent.SetSkillDetectRangeVisible(slotNumber, true);
            return;
        }

        // KeyUp：只有持有者才能施放 ----------
        if (Input.GetKeyUp(key)) {
            if (_holdingPlayer != player || _holdingSlotNumber != slotNumber)
                return;

            combatComponent.SetSkillDetectRangeVisible(slotNumber, false);
            ResolveAndCastSkill(player, slotNumber);

            _holdingPlayer = null;
            _holdingSlotNumber = -1;
        }
    }


    private void ResolveAndCastSkill(Player player, int slotNumber) {
        var slot = player.CombatComponent.SkillSlots[slotNumber-1];
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

            //if(player.StateComponent.IsCastingSkill)player.CombatComponent.SpawnSkill();              //即時釋放技能
            if(!player.CombatComponent.SetIntentSkill(slotNumber, targetPosition, targetTransform))
                player.HitShakeVisual.Play(HitShakeType.SkillFailure, targetPosition.x- player.transform.position.x);
        }
        else {
            // 沒點到敵人，用滑鼠位置(在範圍內用畫鼠位置、範圍外取最近點)
            if (detector.IsInRange(mouseWorldPos)) targetPosition = mouseWorldPos;
            else targetPosition = detector.GetClosestPoint(mouseWorldPos);

            //if (player.StateComponent.IsCastingSkill) player.CombatComponent.SpawnSkill();              //即時釋放技能
            if(!player.CombatComponent.SetIntentSkill(slotNumber, targetPosition, targetTransform))
                player.HitShakeVisual.Play(HitShakeType.SkillFailure, targetPosition.x - player.transform.position.x); ;
        }
    }    //鍵盤施放

    private void CancelCurrentHold() {
        if (_holdingPlayer == null) return;

        var combatComponent = _holdingPlayer.CombatComponent;
        combatComponent.SetSkillDetectRangeVisible(_holdingSlotNumber, false);

        _holdingPlayer = null;
        _holdingSlotNumber = -1;
    }                                   //取消上一個Hold住的按鍵
    #endregion

    //===================================Intent 邏輯==================================
    private void ResetAllBattlePlayerIntent() {
        if (GameManager.Instance == null) {
            Debug.Log("沒有 GameManager，無法 ResetAllBattlePlayerIntent");
            return;
        }

        foreach (var kvp in PlayerUtility.AllPlayers) {
            var p = kvp.Value;
            //p.SetInputProvider(null);
            p.SelectIndicator.SetActive(false);

            p.MoveComponent.ClearAllMoveIntent();
            p.CombatComponent.ClearSkillIntent();
            p.CombatComponent.ClearBaseAttackTargetTransform();
        }
        EnemyOutlineManager.Instance.ClearAll();
    }
    //===============================Outline處理==============================
    private void UpdateHoverEnemy() {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, LayerMask.GetMask("Enemy"));
        Enemy newHover = hit ? hit.GetComponentInParent<Enemy>() : null;

        EnemyOutlineManager.Instance.SetHover(newHover);
    }

}
