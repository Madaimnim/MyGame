using System.Collections.Generic;

public static class EnemyBehaviourTreeFactory {
    public static BehaviorTree Create(EnemyBehaviourTreeType type, Enemy enemy) {
        return type switch {
            EnemyBehaviourTreeType.RushWall=>CreateRushWallTree(enemy),
            EnemyBehaviourTreeType.AdvanceAttack =>CreateAdvanceAttackTree(enemy),
            EnemyBehaviourTreeType.BackAwayAttack =>CreateBackAwayAttackTree(enemy),
            EnemyBehaviourTreeType.Boss =>CreateBossTree(enemy),
            _ => throw new System.Exception("Unknown EnemyBehaviourTreeType")
        };
    }

    // 有目標就攻擊，沒目標就前進
    private static BehaviorTree CreateAdvanceAttackTree(Enemy enemy) {
        var behaviourTree = new BehaviorTree();

        //todo
        var moveSequence = new Sequence(new List<Node> { new Action_MoveAttempt(enemy.AIComponent, 2f) });
        var root = new Selector(new List<Node> { new Action_AttackAllSlots(enemy.AIComponent), moveSequence });

        behaviourTree.SetRoot(root);
        return behaviourTree;
    }

    //todo 無視玩家，直衝城牆（途中不攻擊）
    private static BehaviorTree CreateRushWallTree(Enemy enemy) {
        var behaviourTree = new BehaviorTree();
        
        var moveNode = new Action_MoveAttempt(enemy.AIComponent, moveTime: 999f); // 幾乎一直走

        behaviourTree.SetRoot(moveNode);
        return behaviourTree;
    }


    // todo血量低條件成立時
    private static BehaviorTree CreateBackAwayAttackTree(Enemy enemy) {
        var behaviourTree = new BehaviorTree();

        var backAwayMove = new Action_MoveAttempt(enemy.AIComponent, 1.5f);
        var root = new Selector(new List<Node> {new Action_AttackAllSlots(enemy.AIComponent),backAwayMove});

        behaviourTree.SetRoot(root);
        return behaviourTree;
    }

    // todo Boss
    private static BehaviorTree CreateBossTree(Enemy enemy) {
        var behaviourTree = new BehaviorTree();

        // 先用最基本版本，之後一定會拆 Phase
        var root = new Selector(new List<Node> {new Action_AttackAllSlots(enemy.AIComponent),new Action_MoveAttempt(enemy.AIComponent, 2f)});

        behaviourTree.SetRoot(root);
        return behaviourTree;
    }
}
