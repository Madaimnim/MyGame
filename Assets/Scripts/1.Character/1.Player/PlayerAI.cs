using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAI : MonoBehaviour, IAttackable, IMoveable,IInputProvider
{
    [Header("AI樹更新頻率")]
    public float updateInterval = 0.1f;
    private float updateTimer = 0f;
    private BehaviorTree behaviorTree;
    private Player player;

    private PendingSkillSlot currentPendingSkillSlot; // 當前暫存技能
    private class PendingSkillSlot
    {
        public int slotIndex;
        public PlayerSkillRuntime skillData;
        public GameObject detector;
    }

    public PlayerSkillSpawner skillSpawner;

    #region 生命週期
    private void Awake() {}
    private void Start() {
        player = GetComponent<Player>();
        behaviorTree = GetComponent<BehaviorTree>();
        SetBehaviorTree();
    }
    private void Update() {
        if (!player.CharAIComponent.CanRunAI) return;
        player.UpdateSkillCooldowns(); // Player 負責管理自己的技能槽冷卻
        RunBehaviorTree();
    }
    #endregion

    //執行技能樹
    private void RunBehaviorTree() {
        if (updateTimer <= 0f)
        {
            behaviorTree.Tick();
            updateTimer = updateInterval;
        }
        updateTimer -= Time.deltaTime;
    }

    //行為樹：判斷是否可使用技能
    public bool CanUseSkill(int slotIndex) {
        if (!IsIndexCorrect(slotIndex)) return false;                             //錯誤SlotIndex
        
        var detectorPrefab = player.GetSkillSlotDetector(slotIndex);
        if (detectorPrefab == null)  return false;                                          //沒有偵測器預製體
        var targetDetector = detectorPrefab.GetComponent<TargetDetector>();

        return targetDetector != null && targetDetector.hasTarget && player.Runtime.SkillSlots[slotIndex].CooldownTimer <= 0;
    }

    //IInputProvider
    public Vector2 GetMoveDirection() {
        // 行為樹運算，決定方向
        return Vector2.zero;
    }

    public bool IsAttackPressed() {
        // 行為樹決定是否釋放技能
        return false;
    }

    public bool CanMove() {
        return player.CharMovementComponent.CanMove;
    }

    //行為樹：暫存技能資訊
    #region UseSkill(int slotIndex)
    public void UseSkill(int slotIndex) {
        var skillData = player.Runtime.GetSkillDataRuntimeAtSlot(slotIndex);
        if (skillData == null) return;
        var detector = player.GetSkillSlotDetector(slotIndex);
        if (detector == null) return;
        TargetDetector targetDetector = detector.GetComponent<TargetDetector>();
        if (targetDetector == null)  return;
        if (targetDetector.targetTransform == null) return;

        // 記錄當前要釋放的技能
        currentPendingSkillSlot = new PendingSkillSlot
        {
            slotIndex = slotIndex,
            skillData = skillData,
            detector = detector
        };

        if (player.CharAnimationComponent.IsPlayingAttackAnimation) return;
        else
            PlayAttackAnimation(slotIndex, skillData, targetDetector);
    }
    #endregion

    //撥放攻擊動畫
    #region PlayAttackAnimation(int slotIndex,PlayerSkillRuntime skillData)
    private void PlayAttackAnimation(int slotIndex,PlayerSkillRuntime skillData,TargetDetector targetDetector) {
        // 翻轉角色朝向
        bool isTargetOnLeft = targetDetector.targetTransform.position.x < transform.position.x;
        transform.localScale = new Vector3(
            isTargetOnLeft ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );

        // 播放動畫
        if(player.CharMovementComponent.IsMoving)
            player.CharAnimationComponent.Play($"MoveSkill{skillData.SkillId}");
        else
            player.CharAnimationComponent.Play($"Skill{skillData.SkillId}");
    }
    #endregion

    //動畫事件生成技能
    #region  AnimationEvent_SpawnerSkill() 
    public void AnimationEvent_SpawnerSkill() {
        if (currentPendingSkillSlot == null) return;
        skillSpawner.SpawnSkill(currentPendingSkillSlot.slotIndex, currentPendingSkillSlot.skillData, currentPendingSkillSlot.detector);
    }
    #endregion

    //Todo:Move
    #region  Move()
    public void Move() { 
    
    }
    #endregion

    //初始化行為樹
    #region SetBehaviorTree()
    private void SetBehaviorTree() {
        behaviorTree.SetRoot(new Selector(new List<Node>
        {
            new Action_Attack(this, 3),
            new Action_Attack(this,2),
            new Action_Attack(this,1),
            new Action_Attack(this,0),
            //new Action_Move(this),
            new Action_Idle()
        }));
    }
    #endregion

    #region IsIndexCorrect(int slotIndex)
    private bool IsIndexCorrect(int slotIndex) {
        return slotIndex >= 0 && slotIndex < player.GetSkillSlotsLength();
    }
    #endregion

}
