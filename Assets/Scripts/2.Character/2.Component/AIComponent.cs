using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//只負責BehaviourTreeTick、間隔時間等
public class AIComponent
{
    public bool CanRunAI { get; private set; } = false;

    public TargetDetector GetMoveDetector() => MoveComponent.MoveDetector;

    public MoveComponent MoveComponent { get; private set; }
    public CombatComponent CombatComponent { get; private set; }
    public Transform Transform { get; private set; }
    public MoveStrategyBase MoveStrategy { get; private set; }

    private BehaviorTree _behaviorTree;

    private float _updateInterval = 0.1f;           // AI 決策間隔（秒）
    private float _updateTimer = 0f;

    public AIComponent(MoveComponent moveComponent,CombatComponent combatComponentonent, Transform transform, MoveStrategyBase moveStrategy) {
        MoveComponent = moveComponent;
        CombatComponent = combatComponentonent;
        Transform = transform;
        MoveStrategy = moveStrategy;
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
}
