using UnityEngine;

public class Action_Attack : Node
{
    private BehaviorTree behaviorTree;
    public Action_Attack(BehaviorTree behaviorTree) {
        this.behaviorTree = behaviorTree;
    }
    public override NodeState Evaluate() {

        if (behaviorTree.canChangeAnim)
        {
            switch (true)
            {
                case var _ when behaviorTree.isInAttack01Range:
                    behaviorTree.Attack01Invoke();
                    break;
                case var _ when behaviorTree.isInAttack02Range:
                    behaviorTree.Attack02Invoke();
                    break;
                case var _ when behaviorTree.isInAttack03Range:
                    behaviorTree.Attack03Invoke();
                    break;
                case var _ when behaviorTree.isInAttack04Range:
                    behaviorTree.Attack04Invoke();
                    break;
                default:
                    break;
            }

            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
