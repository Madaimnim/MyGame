using UnityEngine;

public class Action_Move : Node
{
    public Action_Move() {
    }
    public override NodeState Evaluate() {
        if (true) 
        { 
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE;
    }
}
