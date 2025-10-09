using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    private List<Node> children;

    // 記錄上一幀 Running的節點
    private Node runningNode = null; 

    public Selector(List<Node> children) {
        this.children = children;
    }

    public override NodeState Evaluate(float updateInterval) {
        if (runningNode != null)
        {
            NodeState result = runningNode.Evaluate(updateInterval);

            if (result == NodeState.RUNNING)
                return NodeState.RUNNING; 
            else
                runningNode = null; 
        }

        foreach (var child in children)
        {
            NodeState result = child.Evaluate(updateInterval);

            if (result == NodeState.RUNNING)
            {
                runningNode = child;  // 記住這個 `Running` 的節點
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
