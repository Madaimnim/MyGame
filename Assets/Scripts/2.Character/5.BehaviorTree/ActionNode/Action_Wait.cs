using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Wait : Node
{
    private float _stayTime;
    private float _timer;
    public Action_Wait(float stayTime) {
        _stayTime = stayTime;
        _timer = 0f;
    }
    public override NodeState Evaluate(float updateInterval) {
        _timer += updateInterval;

        if (_timer >= _stayTime)
        {
            _timer = 0f;
            return NodeState.SUCCESS; 
        }

        return NodeState.RUNNING;
    }
}
