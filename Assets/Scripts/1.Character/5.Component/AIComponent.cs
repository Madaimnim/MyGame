using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIComponent : IInputProvider
{
    public bool CanRunAI { get; private set; } = false;

    private SkillComponent _skillComponent;
    private MoveComponent _moveComponent;
    private BehaviorTree _behaviorTree;
    private MoveStrategyType _moveStrategyType;
    private MoveStrategyBase _moveStrategy;

    private float _updateInterval = 0.1f;           // AI 決策間隔（秒）
    private float _updateTimer = 0f;

    public AIComponent(SkillComponent skillComponent, MoveComponent charMoveComponent,int skillSlotCount) {
        _skillComponent = skillComponent;
        _moveComponent = charMoveComponent;

        _behaviorTree = new BehaviorTree();
        SetBehaviorTree();
        SetMoveStrategy();
    }

    public void TickTree() {
        RunBehaviorTree();
    }

    private void SetBehaviorTree() {
        var children = new List<Node>();
        children.Add(new Action_AttackAllSlots(_skillComponent));
        children.Add(new Action_Move(_moveComponent));
        children.Add(new Action_Idle());
        _behaviorTree.SetRoot(new Selector(children));
    }
    public void SetMoveStrategy() {
        switch (_moveStrategyType) // 根據 Enum 設置策略
        {
            case MoveStrategyType.Straight:
                _moveStrategy = new StraightMoveStrategy();
                break;
            case MoveStrategyType.Random:
                _moveStrategy = new RandomMoveStrategy();
                break;
            case MoveStrategyType.FollowPlayer:
                _moveStrategy = new FollowPlayerMoveStrategy();
                break;
            default:
                Debug.LogWarning($"未定義的移動策略");
                break;
        }
    }

    private void RunBehaviorTree() {
        if (_updateTimer <= 0f)
        {
            _behaviorTree.Tick();
            _updateTimer = _updateInterval;
        }
        _updateTimer -= Time.deltaTime;
    }

    public void EnableAI() => CanRunAI = true;
    public void DisableAI() => CanRunAI = false;

    public Vector2 GetMoveDirection() {
        if (CanRunAI) return Vector2.zero;
        return Vector2.zero;
    }


    public void SetIntentMoveDirection() {
      
    }

    public void SetIntentSkillSlot() {
    
    }

    public void SetIntentTargetPosition() {
  
    }
}
