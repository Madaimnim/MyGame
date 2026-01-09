using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using System.Collections;

public class SkillHeightComponent {
    private Transform _sprTransform;


    private bool _canGravityFall = false;
    private float _gravity;
    private float _verticalVelocity = 0f;

    //地面微小偏差容許值
    const float GROUND_EPSILON = 0.001f;

    public SkillHeightComponent(Transform sprTransform,bool canGravityFall) {
        _sprTransform= sprTransform;
        _canGravityFall= canGravityFall;

        _gravity = GameSettingManager.Instance.PhysicConfig.GravityScale;
    }

    public void FixedTick() {
        if (_canGravityFall) {
            GravityFall();

            if (_sprTransform.localPosition.y <= GROUND_EPSILON) {
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
        float currentHeight = _sprTransform.localPosition.y + _verticalVelocity * dt;
        if (currentHeight <= 0f) currentHeight = 0f;
        UpdateHeight(currentHeight);
        _verticalVelocity -= _gravity * dt;
    }

    //更新高度
    public void UpdateHeight(float y) {
        _sprTransform.localPosition = new Vector3(_sprTransform.localPosition.x, y, _sprTransform.localPosition.z);
    }
}
