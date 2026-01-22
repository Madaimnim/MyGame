using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using System.Collections;

// 注意：SkillHeightComponent 不應影響 StateComponent
// 若未來要共用高度物理，請抽 HeightPhysics，不要直接合併
public class SkillHeightComponent {
    private Transform _heightTransform;


    private bool _canGravityFall = false;
    private float _gravity;
    private float _verticalVelocity = 0f;

    //地面微小偏差容許值
    const float GROUND_EPSILON = 0.001f;

    public SkillHeightComponent(Transform rootSpriteTransform,bool canGravityFall) {
        _heightTransform = rootSpriteTransform;
        _canGravityFall= canGravityFall;

        _gravity = GameSettingManager.Instance.PhysicConfig.GravityScale;
    }

    public void FixedTick() {
        if (_canGravityFall) {
            GravityFall();

            if (_heightTransform.localPosition.y <= GROUND_EPSILON) {
                UpdateHeight(0f);
                _verticalVelocity = 0f;
            }
        }
    }

    //浮空
    public void AddUpPower(float upPower) {
        _verticalVelocity += upPower;
    }

    //重力控制
    public void GravityFall() {
        float dt = Time.fixedDeltaTime;
        float currentHeight = _heightTransform.localPosition.y + _verticalVelocity * dt;
        if (currentHeight <= 0f) currentHeight = 0f;
        UpdateHeight(currentHeight);
        _verticalVelocity -= _gravity * dt;
    }

    //更新高度
    public void AddHeight(float y) {
        _heightTransform.localPosition = new Vector3(_heightTransform.localPosition.x, _heightTransform.localPosition.y+y, _heightTransform.localPosition.z);
    }
    public void UpdateHeight(float y) {
        _heightTransform.localPosition = new Vector3(_heightTransform.localPosition.x, y, _heightTransform.localPosition.z);
    }
}
