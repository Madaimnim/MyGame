using System.Collections.Generic;

public class Selector : Node
{
    private List<Node> children;
    private Node runningNode = null; // ✅ 記錄上一幀 `Running` 的節點

    public Selector(List<Node> children) {
        this.children = children;
    }

    public override NodeState Evaluate() {
        // ✅ 如果上一幀有 `Running` 的節點，優先執行它
        if (runningNode != null)
        {
            NodeState result = runningNode.Evaluate();
            if (result == NodeState.RUNNING)
                return NodeState.RUNNING; // 仍然 Running，停留在該節點
            else
                runningNode = null; // ✅ `Running` 結束，清除記錄
        }

        // ✅ 從頭遍歷子節點
        foreach (var child in children)
        {
            NodeState result = child.Evaluate();
            if (result == NodeState.RUNNING)
            {
                runningNode = child;  // ✅ 記住這個 `Running` 的節點
                return NodeState.RUNNING; // 停留在 `Running` 節點
            }
            else if (result == NodeState.SUCCESS)
            {
                return NodeState.SUCCESS;
            }
        }

        return NodeState.FAILURE; // 所有子節點都失敗
    }
}
