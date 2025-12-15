using System.Collections.Generic;

public static class PlayerBehaviourTreeFactory {

    public static BehaviorTree Create(
        PlayerBehaviourTreeType type,
        AIComponent aiComponent,
        MoveComponent moveComponent,
        SkillComponent skillComponent,
        MoveStrategyBase moveStrategy
    ) {
        return type switch {
            PlayerBehaviourTreeType.NearTargetAttackFirst =>
                CreateNearTargetAttackTree(aiComponent, moveComponent, skillComponent, moveStrategy),

            PlayerBehaviourTreeType.DefensiveAttack =>
                CreateDefensiveTree(aiComponent, moveComponent, skillComponent, moveStrategy),

            _ => throw new System.Exception("Unknown PlayerBehaviourTreeType")
        };
    }

    private static BehaviorTree CreateNearTargetAttackTree(AIComponent aiComponent,MoveComponent moveComponent,SkillComponent skillComponent,MoveStrategyBase moveStrategy) {
        var behaviourTree = new BehaviorTree();
        var moveSequence = new Sequence(new List<Node> {new Action_MoveAttempt(aiComponent, moveStrategy, moveComponent, skillComponent, 2f)});
        var root = new Selector(new List<Node> {new Action_AttackAllSlots(skillComponent, aiComponent),moveSequence});

        behaviourTree.SetRoot(root);
        return behaviourTree;
    }

    private static BehaviorTree CreateDefensiveTree(AIComponent aiComponent,MoveComponent moveComponent,SkillComponent skillComponent,MoveStrategyBase moveStrategy) {
        var behaviourTree = new BehaviorTree();

        // Todo
        // 根據需求添加更多節點和邏輯

        return behaviourTree;
    }
}
