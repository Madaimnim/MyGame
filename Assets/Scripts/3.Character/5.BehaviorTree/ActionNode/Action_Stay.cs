using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Stay : Node
{
    private float _waitTime;
    private float _timer;
    public Action_Stay(float waitTime) {
        _waitTime = waitTime;
        _timer = 0f;
    }
    public override NodeState Evaluate(float updateInterval) {
        _timer += updateInterval;

        if (_timer >= _waitTime)
        {
            _timer = 0f;
            return NodeState.SUCCESS; // 等待完成 → 結束此行為
        }

        return NodeState.RUNNING; // 等待中
    }

}
