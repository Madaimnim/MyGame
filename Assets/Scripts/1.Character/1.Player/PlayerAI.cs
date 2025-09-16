using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerAI : MonoBehaviour, IAttackable, IMoveable
{
    [Header("AI樹更新頻率")]
    public PlayerSkillSpawner skillSpawner;
    public float updateInterval = 0.1f;
    private float updateTimer = 0f;
    private BehaviorTree behaviorTree;
    private Player player;

    //生命週期
    #region

    private void Awake() {}

    private void Start() {
        player = GetComponent<Player>();
        behaviorTree = GetComponent<BehaviorTree>();
        SetBehaviorTree();
    }

    private void Update() {
        if (!player.canRunAI) return;
        player.UpdateSkillCooldowns(); // Player 負責管理自己的技能槽冷卻
        RunBehaviorTree();
    }
    #endregion

    //執行技能樹
    #region RunBehaviorTree()
    private void RunBehaviorTree() {
        if (updateTimer <= 0f)
        {
            behaviorTree.Tick();
            updateTimer = updateInterval;
        }
        updateTimer -= Time.deltaTime;
    }
    #endregion

    //BehaviorTree判斷技能是否冷卻完成
    #region CanUseSkill(int skillSlot)
    public bool CanUseSkill(int slotIndex) {
        if (!IsIndexCorrect(slotIndex)) return false;                             //錯誤SlotIndex
        
        var detectorPrefab = player.GetSkillSlotDetector(slotIndex);
        if (detectorPrefab == null)  return false;                                          //沒有偵測器預製體
        var targetDetector = detectorPrefab.GetComponent<TargetDetector>();

        return targetDetector != null && targetDetector.hasTarget && player.GetSkillSlotCooldownTimer(slotIndex) <= 0;
    }

    public void UseSkill(int slotIndex) {
        var skillData = player.GetSkillSlotData(slotIndex);
        if (skillData == null) return;
        if (player.isPlayingAttackAnimation) return;
        else
            PlayAttackAnimation(slotIndex, skillData);
    }

    //確認SlotIndex是否正確
    private bool IsIndexCorrect(int slotIndex) {
        return slotIndex >= 0 && slotIndex < player.GetSkillSlotsLength();
    }
    #endregion



    //攻擊Todo
    #region 公開Attack()方法
    private void PlayAttackAnimation(int slotIndex,PlayerStateManager.PlayerStatsRuntime.SkillData skillData) {
        var skillObject = skillData.skillPrefab.GetComponent<SkillObject>();
        if (skillObject == null) return;
        var detector = player.GetSkillSlotDetector(slotIndex);
        TargetDetector targetDetector = detector.GetComponent<TargetDetector>();
        if (targetDetector == null || targetDetector.targetTransform == null) return; // 檢查 targetTransform

        // 翻轉角色朝向
        bool isTargetOnLeft = targetDetector.targetTransform.position.x < transform.position.x;
        transform.localScale = new Vector3(isTargetOnLeft ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x),
                                     transform.localScale.y,
                                     transform.localScale.z);

        player.animator.Play(Animator.StringToHash($"skill{skillData.skillID}"));
        StartCoroutine(skillSpawner.SpawnSkillAfterDelay(slotIndex, skillObject.attackSpawnDelayTime, skillData.skillPrefab, detector));
    }
    #endregion

    //技能升級
    #region SkillLevelUp(){}
    public void SkillLevelUp(int slotIndex) {
        player.playerStats.GetSkillAtSkillSlot(slotIndex).currentLevel++;
        player.playerStats.GetSkillAtSkillSlot(slotIndex).attack++;
        player.playerStats.GetSkillAtSkillSlot(slotIndex).nextSkillLevelCount += player.playerStats.GetSkillAtSkillSlot(slotIndex).currentLevel * 10;
        TextPopupManager.Instance.ShowSkillLevelUpPopup(player.playerStats.GetSkillAtSkillSlot(slotIndex).skillName, player.playerStats.GetSkillAtSkillSlot(slotIndex).currentLevel, transform);
        EventManager.Instance.Event_SkillInfoChanged?.Invoke(slotIndex, this);
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
}
