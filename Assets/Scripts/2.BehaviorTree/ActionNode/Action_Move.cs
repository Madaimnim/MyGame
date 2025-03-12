using UnityEngine;

public class Action_Move : Node
{
    private BehaviorTree behaviorTree;
    public Action_Move(BehaviorTree behaviorTree) {
        this.behaviorTree = behaviorTree;
    }
    public override NodeState Evaluate() {
        if (behaviorTree.canMove && behaviorTree.canChangeAnim) 
        { 
            behaviorTree.MoveInvoke();
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE;
    }
}
