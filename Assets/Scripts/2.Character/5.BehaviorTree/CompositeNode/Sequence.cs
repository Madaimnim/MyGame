using System.Collections.Generic;

//照順序每個子節點執行，直到有一個失敗或是全部成功，必須有Running記憶
public class Sequence : Node
{
    private List<Node> _children;
    private int _currentIndex = 0;

    public Sequence(List<Node> children) {
        _children = children;
    }

    public override NodeState Evaluate(float updateInterval) {
        while (_currentIndex < _children.Count)
        {
            NodeState result = _children[_currentIndex].Evaluate(updateInterval);

            switch (result)
            {
                case NodeState.RUNNING:
                    return NodeState.RUNNING;

                case NodeState.FAILURE:
                    _currentIndex = 0; // 重設
                    return NodeState.FAILURE;

                case NodeState.SUCCESS:
                    _currentIndex++;
                    break;
            }
        }

        // 全部執行完才算成功
        _currentIndex = 0;
        return NodeState.SUCCESS;
    }
}
