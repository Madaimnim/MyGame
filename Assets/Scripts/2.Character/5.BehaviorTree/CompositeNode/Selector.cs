using System.Collections.Generic;

// Selector（優先權選擇器）
// 每一幀都「重新評估」所有子節點的優先順序，不記憶任何 RUNNING 狀態（無記憶）

//行為規則：
// 1. 由上而下依序 Evaluate 子節點
// 2. 第一個回傳 SUCCESS 或 RUNNING 的節點立即中止搜尋
// 3. 若所有子節點皆 FAILURE，才回傳 FAILURE
//
// 使用時機：
// 移動途中可即時插入高優先行為（例如攻擊）
// 需要即時反應、允許行為被中斷的情境

// RUNNING 不代表「鎖定此行為」
// 下一幀仍會從第一個子節點重新判斷

public class Selector : Node {
    private List<Node> _children;

    public Selector(List<Node> children) {
        _children = children;
    }

    public override NodeState Evaluate(float updateInterval) {
        foreach (var child in _children) {
            var result = child.Evaluate(updateInterval);

            if (result == NodeState.SUCCESS || result == NodeState.RUNNING)
                return result;
        }

        return NodeState.FAILURE;
    }
}
