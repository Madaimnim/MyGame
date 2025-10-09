using UnityEngine;

public class Action_Idle : Node
{

    public Action_Idle() {
    
    }
    public override NodeState Evaluate(float updateInterval) {

        return NodeState.SUCCESS;
    }
}
