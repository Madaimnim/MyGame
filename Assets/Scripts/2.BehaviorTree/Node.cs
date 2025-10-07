public abstract class Node
{
    public enum NodeState
    {
        SUCCESS,   // 成功
        FAILURE,   // 失敗
        RUNNING    // 持續執行
    }

    protected NodeState state;              //儲存節點狀態
    public abstract NodeState Evaluate();
}