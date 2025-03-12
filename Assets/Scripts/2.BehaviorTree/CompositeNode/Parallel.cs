using System.Collections.Generic;

public class Parallel : Node
{
    private List<Node> children;
    private int successThreshold; // ✅ 需要多少個成功才算成功
    private int failureThreshold; // ✅ 多少個失敗就算失敗

    public Parallel(List<Node> children, int successThreshold, int failureThreshold) {
        this.children = children;
        this.successThreshold = successThreshold;
        this.failureThreshold = failureThreshold;
    }

    public override NodeState Evaluate() {
        int successCount = 0;
        int failureCount = 0;
        bool anyRunning = false;

        foreach (var child in children)
        {
            NodeState result = child.Evaluate();
            if (result == NodeState.SUCCESS)
                successCount++;
            else if (result == NodeState.FAILURE)
                failureCount++;
            else if (result == NodeState.RUNNING)
                anyRunning = true;
        }

        if (successCount >= successThreshold)
            return NodeState.SUCCESS;
        if (failureCount >= failureThreshold)
            return NodeState.FAILURE;

        return anyRunning ? NodeState.RUNNING : NodeState.FAILURE;
    }
}
