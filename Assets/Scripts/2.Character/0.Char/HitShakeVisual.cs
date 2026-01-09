using System.Collections;
using UnityEngine;
public enum HitShakeType {
    Shake,      // 目前敵人用
    PushBack    // 目前玩家用
}
public class HitShakeVisual : MonoBehaviour {
    [Header("Config檔")]
    private HitShakeConfig _hitShakConfig;

    private Coroutine _shakeCoroutine;
    private Vector3 _initialLocalPos;
    private Transform _visualRootTransform;

    private void Awake() {
        _initialLocalPos = transform.localPosition;
        _hitShakConfig = GameSettingManager.Instance.HitShakeConfig;
        _visualRootTransform = GetComponentInParent<IVisualFacing>().VisulaRootTransform;

        if (_hitShakConfig == null) {
            Debug.LogError($"{name} 的 HitShakeVisual 沒有指定 HitShakeConfig！");
        }
    }

    /// <summary>
    /// 播放受擊抖動
    /// </summary>
    /// <param name="hitDirectionX">-1 or +1，代表攻擊來向</param>
    public void Play(HitShakeType hitShakeType, float hitDirectionX) {
        if (_hitShakConfig == null) return;

        if (_shakeCoroutine != null) {
            StopCoroutine(_shakeCoroutine);
            transform.localPosition = _initialLocalPos;
        }

        switch (hitShakeType) {
            case HitShakeType.Shake:
                _shakeCoroutine = StartCoroutine(
                    ShakeRoutine(hitDirectionX,Mathf.Min(_hitShakConfig.Amplitude, _hitShakConfig.maxAmplitude),_hitShakConfig.Duration,_hitShakConfig.Frequency));
                break;

            case HitShakeType.PushBack:
                _shakeCoroutine = StartCoroutine(
                    PushBackRoutine(hitDirectionX,_hitShakConfig.PushBackDistance,_hitShakConfig.PushBackDuration));
                break;
        }
    }


    private IEnumerator ShakeRoutine(float hitDirX, float amplitude, float duration, int frequency) {
        float elapsed = 0f;
        float sign = -Mathf.Sign(hitDirX);

        while (elapsed < duration) {
            elapsed += Time.deltaTime;

            float progress = elapsed / duration;
            float damping = 1f - progress;

            float offsetX =Mathf.Sin(progress * Mathf.PI * frequency)* amplitude* damping* sign;

            transform.localPosition = _initialLocalPos + new Vector3(offsetX, 0f, 0f);
            yield return null;
        }

        transform.localPosition = _initialLocalPos;
        _shakeCoroutine = null;
    }

    private IEnumerator PushBackRoutine(float hitDirX, float distance, float duration) {
        float elapsed = 0f;
        float sign = -Mathf.Sign(hitDirX);
        float startX = _initialLocalPos.x;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float eased = 1f - Mathf.Pow(1f - t, 2f);
            float offsetX = eased * distance * sign;

            transform.localPosition = new Vector3(startX + offsetX, _initialLocalPos.y, _initialLocalPos.z);
            yield return null;
        }

        transform.localPosition = _initialLocalPos;
        _shakeCoroutine = null;
    }
}
