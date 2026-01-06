using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Text_StageClear : MonoBehaviour
{
    private Vector3 _initialLocalPosition;

    private void Awake() {
        _initialLocalPosition = transform.localPosition;
    }

    private void OnEnable() {
        TextPopupManager.Instance.TextBounceBounceEffect(transform);

        // 訂閱 RewardSystem展開進度
        UIManager.Instance.UI_StageClearController.UI_RewardSystem.OnExpandHeightChanged += OnRewardExpandHeightChanged;
    }
    private void OnDisable() {
        transform.localPosition = _initialLocalPosition;

        if (UIManager.Instance != null) UIManager.Instance.UI_StageClearController.UI_RewardSystem.OnExpandHeightChanged -= OnRewardExpandHeightChanged;
        
    }
    private void OnRewardExpandHeightChanged(float heightOffset) {
        //  不算、不猜、不查
        transform.localPosition =_initialLocalPosition + Vector3.up * heightOffset/2;
    }

}
