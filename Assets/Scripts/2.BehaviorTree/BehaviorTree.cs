using System;
using UnityEngine;

public class BehaviorTree:MonoBehaviour
{
    #region 私有變數
    private Node rootNode;
    #endregion

    #region 公開SetRoot()
    public void SetRoot(Node root) {
        rootNode = root;
    }
    #endregion

    #region 公開Tick()方法
    public void Tick() {
        rootNode?.Evaluate();
    }
    #endregion
}

public abstract class Node
{
    protected NodeState state;  // 儲存節點狀態
    public abstract NodeState Evaluate();
}

public enum NodeState
{
    SUCCESS,   // 成功
    FAILURE,   // 失敗
    RUNNING    // 持續執行
}
