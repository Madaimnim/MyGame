using System.Collections.Generic;
using UnityEngine;

// 行為一旦 RUNNING，就會被「鎖住」直到完成

// 一旦某個子節點回傳 RUNNING，就「鎖定」該節點，直到它回傳 SUCCESS 或 FAILURE

// 行為規則：
// 1. 若已有 runningNode：
//    - 只 Evaluate 該節點
//    - RUNNING → 繼續鎖定
//    - SUCCESS / FAILURE → 解鎖，重新選擇
//
// 2. 若目前沒有 runningNode：
//    - 由上而下依序 Evaluate 子節點
//    - 第一個回傳 RUNNING 的節點會被鎖定
//    - 第一個回傳 SUCCESS 的節點立即成功
//
// 使用時機：
// 不可被中斷的行為流程
// (例如：施法、長動畫、過場、任務步驟）
// 「開始了就一定要做完」的行為
//
// 注意：
// 與 Selector 相反，此 Selector 不會即時反應新狀態

public class MemorySelector : Node
{
    private List<Node> children;

    // 記錄上一幀 Running的節點
    private Node runningNode = null; 

    public MemorySelector(List<Node> children) {
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
