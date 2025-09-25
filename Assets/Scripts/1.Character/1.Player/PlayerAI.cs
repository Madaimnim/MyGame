using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAI : MonoBehaviour, IAttackable, IMoveable,IInputProvider
{
    [Header("AI���s�W�v")]
    public float updateInterval = 0.1f;
    private float updateTimer = 0f;
    private BehaviorTree behaviorTree;
    private Player player;

    private PendingSkillSlot currentPendingSkillSlot; // ��e�Ȧs�ޯ�
    private class PendingSkillSlot
    {
        public int slotIndex;
        public PlayerSkillRuntime skillData;
        public GameObject detector;
    }

    public PlayerSkillSpawner skillSpawner;

    #region �ͩR�g��
    private void Awake() {}
    private void Start() {
        player = GetComponent<Player>();
        behaviorTree = GetComponent<BehaviorTree>();
        SetBehaviorTree();
    }
    private void Update() {
        if (!player.CharAIComponent.CanRunAI) return;
        player.UpdateSkillCooldowns(); // Player �t�d�޲z�ۤv���ޯ�ѧN�o
        RunBehaviorTree();
    }
    #endregion

    //����ޯ��
    private void RunBehaviorTree() {
        if (updateTimer <= 0f)
        {
            behaviorTree.Tick();
            updateTimer = updateInterval;
        }
        updateTimer -= Time.deltaTime;
    }

    //�欰��G�P�_�O�_�i�ϥΧޯ�
    public bool CanUseSkill(int slotIndex) {
        if (!IsIndexCorrect(slotIndex)) return false;                             //���~SlotIndex
        
        var detectorPrefab = player.GetSkillSlotDetector(slotIndex);
        if (detectorPrefab == null)  return false;                                          //�S���������w�s��
        var targetDetector = detectorPrefab.GetComponent<TargetDetector>();

        return targetDetector != null && targetDetector.hasTarget && player.Runtime.SkillSlots[slotIndex].CooldownTimer <= 0;
    }

    //IInputProvider
    public Vector2 GetMoveDirection() {
        // �欰��B��A�M�w��V
        return Vector2.zero;
    }

    public bool IsAttackPressed() {
        // �欰��M�w�O�_����ޯ�
        return false;
    }

    public bool CanMove() {
        return player.CharMovementComponent.CanMove;
    }

    //�欰��G�Ȧs�ޯ��T
    #region UseSkill(int slotIndex)
    public void UseSkill(int slotIndex) {
        var skillData = player.Runtime.GetSkillDataRuntimeAtSlot(slotIndex);
        if (skillData == null) return;
        var detector = player.GetSkillSlotDetector(slotIndex);
        if (detector == null) return;
        TargetDetector targetDetector = detector.GetComponent<TargetDetector>();
        if (targetDetector == null)  return;
        if (targetDetector.targetTransform == null) return;

        // �O����e�n���񪺧ޯ�
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

    //��������ʵe
    #region PlayAttackAnimation(int slotIndex,PlayerSkillRuntime skillData)
    private void PlayAttackAnimation(int slotIndex,PlayerSkillRuntime skillData,TargetDetector targetDetector) {
        // ½�ਤ��¦V
        bool isTargetOnLeft = targetDetector.targetTransform.position.x < transform.position.x;
        transform.localScale = new Vector3(
            isTargetOnLeft ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );

        // ����ʵe
        if(player.CharMovementComponent.IsMoving)
            player.CharAnimationComponent.Play($"MoveSkill{skillData.SkillId}");
        else
            player.CharAnimationComponent.Play($"Skill{skillData.SkillId}");
    }
    #endregion

    //�ʵe�ƥ�ͦ��ޯ�
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

    //��l�Ʀ欰��
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
