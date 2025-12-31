using System.Collections.Generic;
using UnityEngine;

public class Box_Detector : SkillDetectorBase {
    private float _rangeX;   // 前方距離（長度）
    private float _rangeY;   // 上下範圍（高度的一半）

    public Box_Detector(float rangeX, float rangeY) {
        _rangeX= Mathf.Round(rangeX * 100f) / 100f;
        _rangeY= Mathf.Round(rangeY * 100f) / 100f;
    }

    public override void Initialize(Transform self) => _self = self;

    public override void DetectTargetsTick(IReadOnlyList<IInteractable> targetList) {
        _targetTransform = null; //每次 Tick 前清空舊目標

        float minDist = float.MaxValue;
        foreach (var interactable in targetList) {
            if (interactable == null || interactable.Equals(null)) continue;

            Vector2 targetPos = interactable.BottomTransform.position;
            if (IsInRange(targetPos)) {
                float dist = Vector2.Distance(_self.position, targetPos);
                
                if (dist < minDist) {
                    minDist = dist;
                    _targetTransform = interactable.BottomTransform;
                }
            }
        }
    }

    public override bool IsInRange(Vector2 targetPos) {
        Vector2 selfPos = _self.position;

        var diffX = Mathf.Abs(targetPos.x - selfPos.x);
        var diffY = Mathf.Abs(targetPos.y - selfPos.y);

        if (diffX <= _rangeX && diffY <= _rangeY) return true;
        return false;
    }

    // 回傳最近點（Clamp 在矩形邊界內）
    public override Vector2 GetClosestPoint(Vector2 worldPos) {
        Vector2 selfPos = _self.position;

        float clampedX = Mathf.Clamp(worldPos.x, selfPos.x - _rangeX, selfPos.x + _rangeX);
        float clampedY = Mathf.Clamp(worldPos.y, selfPos.y - _rangeY, selfPos.y + _rangeY);

        return new Vector2(clampedX, clampedY);
    }

    public override GameObject SpawnRangeObject(Transform parent) {
        var detectorSpriteSpawner = new DetectorSpriteSpawner();
        return detectorSpriteSpawner.CreateRectangleObject(_rangeX, _rangeY);
    }
}
