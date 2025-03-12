using System.Collections.Generic;
using UnityEngine;

public class RandomSelector : Node
{
    private List<Node> children;
    private Node runningNode = null;

    public RandomSelector(List<Node> children) {
        this.children = children;
    }

    public override NodeState Evaluate() {
        // ✅ 若有正在執行的節點，繼續執行它
        if (runningNode != null)
        {
            NodeState result = runningNode.Evaluate();
            if (result == NodeState.RUNNING)
                return NodeState.RUNNING;

            runningNode = null; // ✅ `Running` 結束，清除記錄
        }

        // ✅ 隨機選擇一個新節點執行
        if (children.Count == 0)
            return NodeState.FAILURE;

        int index = Random.Range(1,children.Count+1);
        runningNode = children[index];
        return runningNode.Evaluate();
    }
}
