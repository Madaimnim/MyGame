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
