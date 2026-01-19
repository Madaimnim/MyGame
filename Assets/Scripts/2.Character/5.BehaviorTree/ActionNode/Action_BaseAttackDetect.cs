using UnityEngine;

public class Action_BaseAttackDetect : Node
{
    private AIComponent _ai;
    private CombatComponent _combatComponent;

    public Action_BaseAttackDetect(AIComponent ai) {
        _ai = ai;
        _combatComponent= _ai.CombatComponent;
        
    }

    public override NodeState Evaluate(float updateInterval) {
        bool success = false;
        var detector = _combatComponent.BaseAttackSlot.Detector;

        if (!detector.HasTarget) 
            return NodeState.FAILURE;
        else
            success = _combatComponent.SetIntentBaseAttack(detector.TargetTransform);
        
        // Update detector state
        //Transform target = _ai.GetNearestTargetInBaseAttackRange();
        //if (target == null) {
        //    return NodeState.FAILURE;
        //}
        
        //Debug.Log($"SetIntentBaseAttack to {target.name}, success: {success}");
        return success ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
