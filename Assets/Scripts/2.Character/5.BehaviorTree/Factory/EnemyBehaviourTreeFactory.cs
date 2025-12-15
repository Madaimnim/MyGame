using System.Collections.Generic;

public static class EnemyBehaviourTreeFactory {
    public static BehaviorTree Create(EnemyBehaviourTreeType type, AIComponent aiComponent,MoveComponent moveComponent,SkillComponent skillComponent,MoveStrategyBase moveStrategy    ) {
        return type switch {
            EnemyBehaviourTreeType.RushWall =>CreateRushWallTree(aiComponent, moveComponent, skillComponent, moveStrategy),
            EnemyBehaviourTreeType.AdvanceAttack =>CreateAdvanceAttackTree(aiComponent, moveComponent, skillComponent, moveStrategy),
            EnemyBehaviourTreeType.BackAwayAttack =>CreateBackAwayAttackTree(aiComponent, moveComponent, skillComponent, moveStrategy),
            EnemyBehaviourTreeType.Boss =>CreateBossTree(aiComponent, moveComponent, skillComponent, moveStrategy),
            _ => throw new System.Exception("Unknown EnemyBehaviourTreeType")
        };
    }

    // 無視玩家，直衝城牆（途中不攻擊）
    private static BehaviorTree CreateRushWallTree(AIComponent aiComponent,MoveComponent moveComponent,SkillComponent skillComponent,MoveStrategyBase moveStrategy) {
        var behaviourTree = new BehaviorTree();
        
        //todo
        var moveNode = new Action_MoveAttempt(aiComponent,moveStrategy,moveComponent,skillComponent,moveTime: 999f); // 幾乎一直走

        behaviourTree.SetRoot(moveNode);
        return behaviourTree;
    }
 
    // 有目標就攻擊，沒目標就前進
    private static BehaviorTree CreateAdvanceAttackTree(AIComponent aiComponent,MoveComponent moveComponent,SkillComponent skillComponent,MoveStrategyBase moveStrategy) {
        var behaviourTree = new BehaviorTree();

        //todo
        var moveSequence = new Sequence(new List<Node> {new Action_MoveAttempt(aiComponent, moveStrategy, moveComponent, skillComponent, 2f)});
        var root = new Selector(new List<Node> {new Action_AttackAllSlots(skillComponent, aiComponent),moveSequence});

        behaviourTree.SetRoot(root);
        return behaviourTree;
    }

    // 血量低條件成立時
    private static BehaviorTree CreateBackAwayAttackTree(AIComponent aiComponent,MoveComponent moveComponent,SkillComponent skillComponent,MoveStrategyBase moveStrategy) {
        var behaviourTree = new BehaviorTree();

        //todo
        var backAwayMove = new Action_MoveAttempt(aiComponent,moveStrategy,moveComponent,skillComponent,1.5f);
        var root = new Selector(new List<Node> {new Action_AttackAllSlots(skillComponent, aiComponent),backAwayMove});

        behaviourTree.SetRoot(root);
        return behaviourTree;
    }

    // Boss
    private static BehaviorTree CreateBossTree(AIComponent aiComponent,MoveComponent moveComponent,SkillComponent skillComponent,MoveStrategyBase moveStrategy) {
        var behaviourTree = new BehaviorTree();

        // todo先用最基本版本，之後一定會拆 Phase
        var root = new Selector(new List<Node> {new Action_AttackAllSlots(skillComponent, aiComponent),new Action_MoveAttempt(aiComponent, moveStrategy, moveComponent, skillComponent, 2f)});

        behaviourTree.SetRoot(root);
        return behaviourTree;
    }
}
