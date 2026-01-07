using System.Collections.Generic;

public static class PlayerBehaviourTreeFactory {

    public static BehaviorTree Create(PlayerBehaviourTreeType type,Player player) {
        return type switch {
            PlayerBehaviourTreeType.NearTargetAttackFirst =>CreateNearTargetAttackTree(player),
            PlayerBehaviourTreeType.DefensiveAttack =>CreateDefensiveTree(player),
            _ => throw new System.Exception("Unknown PlayerBehaviourTreeType")
        };
    }

    private static BehaviorTree CreateNearTargetAttackTree(Player player) {
        var behaviourTree = new BehaviorTree();
        var moveSequence = new Sequence(new List<Node> {new Action_MoveAttempt(player.AIComponent, 2f)});
        var root = new Selector(new List<Node> {new Action_AttackAllSlots(player.AIComponent),moveSequence});

        behaviourTree.SetRoot(root);
        return behaviourTree;
    }

    private static BehaviorTree CreateDefensiveTree(Player player) {
        var behaviourTree = new BehaviorTree();

        // Todo
        // 根據需求添加更多節點和邏輯

        return behaviourTree;
    }
}
