using System.Collections.Generic;

public class Sequence : Node
{
    private List<Node> children;
    private Node runningNode = null; // ✅ 記錄 `Running` 的節點

    public Sequence(List<Node> children) {
        this.children = children;
    }

    public override NodeState Evaluate() {
        //若有 `Running` 的節點，先執行它
        if (runningNode != null)
        {
            NodeState result = runningNode.Evaluate();
            if (result == NodeState.RUNNING)
                return NodeState.RUNNING; // 仍然執行中

            runningNode = null; // ✅ `Running` 結束，清除記錄
            if (result == NodeState.FAILURE)
                return NodeState.FAILURE; // 直接失敗
        }

        // 持續從當前節點往後執行
        foreach (var child in children)
        {
            NodeState result = child.Evaluate();
            if (result == NodeState.RUNNING)
            {
                runningNode = child; // ✅ 記住這個 `Running` 的節點
                return NodeState.RUNNING;
            }
            else if (result == NodeState.FAILURE)
            {
                return NodeState.FAILURE; // 只要有失敗，整個 Sequence 失敗
            }
        }

        return NodeState.SUCCESS; // 所有子節點執行完畢
    }
}
