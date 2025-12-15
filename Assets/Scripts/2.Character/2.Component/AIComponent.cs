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

    public AIComponent(SkillComponent skillComponent, MoveComponent charMoveComponent,Transform self, MoveStrategyBase movestrategy) {
        _skillComponent = skillComponent;
        _moveComponent = charMoveComponent;
        SelfTransform = self;

        _moveStrategy= movestrategy;
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
    public void SetBehaviorTree(BehaviorTree behaviourTree) {
        _behaviorTree = behaviourTree;
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
