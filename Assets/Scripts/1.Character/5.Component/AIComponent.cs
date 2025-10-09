using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIComponent : IInputProvider
{
    public bool CanRunAI { get; private set; } = false;

    public Transform SelfTransform { get; private set; }
    public TargetDetector GetMoveDetector() => _moveComponent.MoveDetector;

    private SkillComponent _skillComponent;
    private MoveComponent _moveComponent;

    private BehaviorTree _behaviorTree;
    private MoveStrategyBase _moveStrategy;

    private float _updateInterval = 0.1f;           // AI 決策間隔（秒）
    private float _updateTimer = 0f;

    public AIComponent(SkillComponent skillComponent, MoveComponent charMoveComponent,Transform self, MoveStrategyType strategyType) {
        _skillComponent = skillComponent;
        _moveComponent = charMoveComponent;
        SelfTransform = self;

        _behaviorTree = new BehaviorTree();

        SetMoveStrategy(strategyType);
        SetBehaviorTree();
    }

    public void Tick() {
        TickTree();
    }

    private void TickTree() {
        if (_updateTimer <= 0f)
        {
            _behaviorTree.Tick(_updateInterval);
            _updateTimer = _updateInterval;
        }
        _updateTimer -= Time.deltaTime;
    }
    private void SetBehaviorTree() {
        // 第一層：移動流程 (先等待再移動)
        var moveSequenceNodes = new List<Node>();
        moveSequenceNodes.Add(new Action_Wait(0.5f));
        moveSequenceNodes.Add(new Action_Move(this, _moveStrategy, _moveComponent, _skillComponent, 2f));
        var moveSequence = new Sequence(moveSequenceNodes) ;
        
        // 第二層：整體選擇（先攻擊，否則執行移動流程）
        var rootNodes = new List<Node>();
        rootNodes.Add(new Action_AttackAllSlots(_skillComponent, this));
        rootNodes.Add(moveSequence);

        _behaviorTree.SetRoot(new Selector(rootNodes));
    }
    private void SetMoveStrategy(MoveStrategyType type) {
        _moveStrategy = type switch
        {
            MoveStrategyType.Follow => new FollowMoveStrategy(),
            MoveStrategyType.Random => new RandomMoveStrategy(),
            MoveStrategyType.Straight => new StraightMoveStrategy(),
            MoveStrategyType.Stay => new StayMoveStrategy(),
            MoveStrategyType.Flee => new FleeMoveStrategy(),
        };
    }

    public void EnableAI() => CanRunAI = true;
    public void DisableAI() => CanRunAI = false;

    //-------------------------------Intent設定--------------------------------------------------------------------------------
    public void ResetPlayerIntent(MoveComponent moveComponent, SkillComponent skillComponent) {
        SetIntentMove(moveComponent);
        SetIntentSkill(skillComponent, -1);
    }
    public void SetIntentMove(MoveComponent moveComponent,Vector2? direction = null, Vector2? targetPosition = null, Transform targetTransform = null) {
        moveComponent.IntentTargetTransform = targetTransform;
        moveComponent.IntentTargetPosition = targetPosition;
        moveComponent.IntentDirection = direction ?? Vector2.zero;
    }
    public void SetIntentSkill(SkillComponent skillComponent,int slotIndex, Vector2? targetPosition = null, Transform targetTransform = null) {
        skillComponent.IntentSlotIndex = slotIndex;
        skillComponent.IntentTargetTransform = targetTransform;
        skillComponent.IntentTargetPosition = targetPosition ?? Vector2.zero;
    }
}
