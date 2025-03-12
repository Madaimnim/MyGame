using UnityEngine;

public class Condition_Cooldown : Node
{
    private BehaviorTree behaviorTree;
    public Condition_Cooldown(BehaviorTree behaviorTree) {
        this.behaviorTree = behaviorTree;
    }
    public override NodeState Evaluate() {
        if (behaviorTree.isCooldownComplete)
        {
            return NodeState.SUCCESS; 
        }
        return NodeState.FAILURE; 
    }
}
