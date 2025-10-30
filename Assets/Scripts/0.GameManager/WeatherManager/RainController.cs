using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RainController : MonoBehaviour {
    [Header(" 雨粒子設定")]
    public ParticleSystem rainSystem;
    public ParticleSystem splashSystem;
    public Volume weatherVolume;

    [Header(" 雨量設定")]
    public int minParticles = 2;
    public int maxParticles = 100;

    [Header(" 雨速設定")]
    public float minSpeed = 15f;
    public float maxSpeed = 20f;

    [Header(" 漸變設定 (秒)")]
    public float transitionTime = 2f;
    public float delayBeforeIncrease = 3f;

    [Header(" 燈光/後製效果設定")]
    public float darkenExposure = -0.5f;
    public Color rainColorFilter = new Color(0.7f, 0.8f, 1f);

    private ParticleSystem.MainModule rainMain;
    private ParticleSystem.MainModule splashMain;

    private Coroutine transitionRoutine;
    private bool isRaining = false;

    private ColorAdjustments colorAdj;
    private float baseExposure;
    private Color baseFilterColor;

    private void Awake() {
        // 主雨系統初始化
        if (rainSystem == null)
            rainSystem = GetComponent<ParticleSystem>();
        rainMain = rainSystem.main;
        rainMain.maxParticles = minParticles;
        rainMain.startSpeed = minSpeed;
        rainSystem.Stop();

        // 濺水系統初始化
        if (splashSystem != null) {
            splashMain = splashSystem.main;
            splashMain.maxParticles = minParticles;
            splashSystem.Stop();
        }

        // Volume 初始化
        if (weatherVolume != null) {
            weatherVolume.profile.TryGet(out colorAdj);
            if (colorAdj != null) {
                baseExposure = colorAdj.postExposure.value;
                baseFilterColor = colorAdj.colorFilter.value;
            }
        }
    }

    public void StartRain() {
        if (isRaining) return;
        isRaining = true;

        rainSystem.Play();
        splashSystem?.Play();

        if (transitionRoutine != null) StopCoroutine(transitionRoutine);
        transitionRoutine = StartCoroutine(StartRainRoutine());
    }

    private IEnumerator StartRainRoutine() {
        rainMain.maxParticles = minParticles;
        rainMain.startSpeed = minSpeed;
        if (splashSystem != null)
            splashMain.maxParticles = minParticles;

        if (delayBeforeIncrease > 0f)
            yield return new WaitForSeconds(delayBeforeIncrease);

        // 👉 下雨階段（由小變大 + 變暗）
        yield return TransitionRain(minParticles, maxParticles, minSpeed, maxSpeed, false, true);
    }

    public void StopRain() {
        if (!isRaining) return;
        isRaining = false;

        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        // 👉 停雨階段（由大變小 + 漸亮）
        transitionRoutine = StartCoroutine(TransitionRain(
            rainMain.maxParticles,
            minParticles,
            rainMain.startSpeed.constant,
            minSpeed,
            true,
            false
        ));
    }

    /// <summary>
    /// 控制雨與 Volume 的同步漸變。
    /// </summary>
    private IEnumerator TransitionRain(
        float startParticles, float targetParticles,
        float startSpeed, float targetSpeed,
        bool stopAfter,
        bool becomingRain // 👉 true = 正在變成下雨, false = 正在停雨
    ) {
        float timer = 0f;

        while (timer < transitionTime) {
            timer += Time.deltaTime;
            float t = timer / transitionTime;

            // 主雨變化
            rainMain.maxParticles = Mathf.RoundToInt(Mathf.Lerp(startParticles, targetParticles, t));
            rainMain.startSpeed = Mathf.Lerp(startSpeed, targetSpeed, t);

            // 濺水同步變化
            if (splashSystem != null)
                splashMain.maxParticles = Mathf.RoundToInt(Mathf.Lerp(startParticles, targetParticles, t));

            // --- Volume 效果 ---
            if (colorAdj != null) {
                if (becomingRain) {
                    // 變成下雨 → 亮度漸暗、偏藍
                    colorAdj.postExposure.Override(Mathf.Lerp(baseExposure, darkenExposure, t));
                    colorAdj.colorFilter.Override(Color.Lerp(baseFilterColor, rainColorFilter, t));
                }
                else {
                    // 停雨 → 亮度漸亮、恢復原色
                    colorAdj.postExposure.Override(Mathf.Lerp(darkenExposure, baseExposure, t));
                    colorAdj.colorFilter.Override(Color.Lerp(rainColorFilter, baseFilterColor, t));
                }
            }

            yield return null;
        }

        // 結尾狀態
        rainMain.maxParticles = Mathf.RoundToInt(targetParticles);
        rainMain.startSpeed = targetSpeed;
        if (splashSystem != null)
            splashMain.maxParticles = Mathf.RoundToInt(targetParticles);

        if (colorAdj != null) {
            if (becomingRain) {
                colorAdj.postExposure.Override(darkenExposure);
                colorAdj.colorFilter.Override(rainColorFilter);
            }
            else {
                colorAdj.postExposure.Override(baseExposure);
                colorAdj.colorFilter.Override(baseFilterColor);
            }
        }

        if (stopAfter) {
            rainSystem.Stop();
            splashSystem?.Stop();
        }
    }
}
