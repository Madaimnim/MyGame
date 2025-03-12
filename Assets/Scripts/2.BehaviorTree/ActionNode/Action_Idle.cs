using UnityEngine;

public class Action_Idle : Node
{
    private BehaviorTree behaviorTree;

    public Action_Idle(BehaviorTree behaviorTree) {
        this.behaviorTree = behaviorTree;
    }
    public override NodeState Evaluate() {

        behaviorTree.IdleInvoke();
        return NodeState.SUCCESS;
    }
}
