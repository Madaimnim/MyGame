using UnityEngine;

public class Action_Move : Node
{
    private MoveComponent _moveComponent;
    public Action_Move(MoveComponent moveComponent) {
        _moveComponent = moveComponent;
    }
    public override NodeState Evaluate() {
        if (_moveComponent.CanMove) 
        {
            _moveComponent.Move();
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE;
    }
}
