using UnityEngine;

public class Action_Move : Node
{
    private IMoveable moveCharacter;
    public Action_Move(IMoveable moveCharacter) {
        this.moveCharacter=moveCharacter;
    }
    public override NodeState Evaluate() {
        if (true) 
        {
            moveCharacter.Move();
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE;
    }
}
