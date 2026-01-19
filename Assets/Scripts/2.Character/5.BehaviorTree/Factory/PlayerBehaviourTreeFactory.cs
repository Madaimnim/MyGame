using System.Collections.Generic;

public static class PlayerBehaviourTreeFactory {

    public static BehaviorTree Create(PlayerBehaviourTreeType type,Player player) {
        return type switch {
            PlayerBehaviourTreeType.MoveAttackType => CreateMoveAttackTree(player),
            PlayerBehaviourTreeType.DefensiveType =>CreateDefensiveTree(player),
            _ => throw new System.Exception("Unknown PlayerBehaviourTreeType")
        };
    }

    //移動攻擊模式
    private static BehaviorTree CreateMoveAttackTree(Player player) {
        var behaviourTree = new BehaviorTree();

        var root = new Selector(new List<Node> {
            new Action_BaseAttackDetect(player.AIComponent),
            new Action_MoveToPoint(player.AIComponent)});

        behaviourTree.SetRoot(root);
        return behaviourTree;
    }

    //Todo 
    private static BehaviorTree CreateDefensiveTree(Player player) {
        var behaviourTree = new BehaviorTree();

        // Todo
        // 根據需求添加更多節點和邏輯

        return behaviourTree;
    }
}
