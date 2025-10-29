using System.Collections.Generic;
using UnityEngine;

public abstract class SkillDetectStrategyBase {
    public bool HasTarget => _targetTransform != null;
    public Transform TargetTransform => _targetTransform;
    protected Transform _targetTransform;
    protected Transform _self;

    public SkillDetectStrategyBase() {}
    
    public abstract void Initialize(Transform self);
    public abstract void DetectTargetsTick(IReadOnlyList<IInteractable> detectedTargets);
    public abstract bool IsInRange(Vector2 mouseWorldPos);
    public abstract Vector2 GetClosestPoint(Vector2 mouseWorldPos);
    public abstract void DrawDebugGizmo();
}
