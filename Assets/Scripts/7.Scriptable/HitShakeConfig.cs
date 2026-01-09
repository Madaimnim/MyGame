using UnityEngine;

[CreateAssetMenu(menuName = "GameConfig/HitShakeConfig")]
public class HitShakeConfig : ScriptableObject {

    [Header("抖動")]
    [InspectorName("最大抖動幅度")]public float Amplitude = 0.2f;
    [InspectorName("抖動時長度")]public float Duration = 0.5f;
    [InspectorName("抖動頻率")]public int Frequency = 12;

    [Header("上限防呆")]
    public float maxAmplitude = 1f;

    [Header("後推")]
    [InspectorName("推後距離")]
    public float PushBackDistance = 0.2f;

    [InspectorName("推後時長")]
    public float PushBackDuration = 0.5f;
}
