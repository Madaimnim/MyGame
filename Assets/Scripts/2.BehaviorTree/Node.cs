public abstract class Node
{
    public enum NodeState
    {
        SUCCESS,   // ���\
        FAILURE,   // ����
        RUNNING    // �������
    }

    protected NodeState state;              //�x�s�`�I���A
    public abstract NodeState Evaluate();
}