using System.Collections;
using UnityEngine;

public enum HitShakeType {
    Shake,      // 目前敵人用
    PushBack,    // 目前玩家用
    SkillFailure
}

public class HitShakeVisual : MonoBehaviour {

    private HitShakeConfig _hitShakeConfig;

    private Coroutine _shakeCoroutine;

    // ★ 只記 X，不記 Y
    private float _initialLocalX;

    private void Awake() {
        _initialLocalX = transform.localPosition.x;
        _hitShakeConfig = GameSettingManager.Instance.HitShakeConfig;

        if (_hitShakeConfig == null) {
            Debug.LogError($"{name} 的 HitShakeVisual 沒有指定 HitShakeConfig！");
        }
    }

    public void Play(HitShakeType hitShakeType, float hitDirectionX) {
        if (_hitShakeConfig == null) return;

        if (_shakeCoroutine != null) {
            StopCoroutine(_shakeCoroutine);
            ResetX();   //  只回 X
        }

        switch (hitShakeType) {
            case HitShakeType.Shake:
                _shakeCoroutine = StartCoroutine(ShakeRoutine(
                        hitDirectionX,
                        Mathf.Min(_hitShakeConfig.Amplitude, _hitShakeConfig.maxAmplitude),
                        _hitShakeConfig.Duration,
                        _hitShakeConfig.Frequency)
                );
                break;

            case HitShakeType.PushBack:
                _shakeCoroutine = StartCoroutine(PushBackRoutine(
                        hitDirectionX,
                        _hitShakeConfig.PushBackDistance,
                        _hitShakeConfig.PushBackDuration)
                );
                break;
            case HitShakeType.SkillFailure:
                _shakeCoroutine = StartCoroutine(ShakeRoutine(
                        hitDirectionX,
                        Mathf.Min(_hitShakeConfig.Amplitude/2, _hitShakeConfig.maxAmplitude/2),
                        _hitShakeConfig.Duration,
                        _hitShakeConfig.Frequency)
                );
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

            ApplyOffsetX(offsetX);
            yield return null;
        }

        ResetX();
        _shakeCoroutine = null;
    }

    private IEnumerator PushBackRoutine(float hitDirX, float distance, float duration) {
        float elapsed = 0f;
        float sign = -Mathf.Sign(hitDirX);

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float eased = 1f - Mathf.Pow(1f - t, 2f);
            float offsetX = eased * distance * sign;

            ApplyOffsetX(offsetX);
            yield return null;
        }

        ResetX();
        _shakeCoroutine = null;
    }

    // ===== 小工具 =====

    private void ApplyOffsetX(float offsetX) {
        var pos = transform.localPosition;
        transform.localPosition = new Vector3(_initialLocalX + offsetX,pos.y,pos.z);   // 永遠保留即時高度
    }

    private void ResetX() {
        var pos = transform.localPosition;
        transform.localPosition = new Vector3(_initialLocalX,pos.y,pos.z);
    }
}
