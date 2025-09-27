using UnityEngine;
using UnityEngine.UI;

public class UI_Loading : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMPro.TextMeshProUGUI progressText;

    private void OnEnable() {
        progressBar.value = 0;
    }

    private void Start() {
        var sceneSystem = GameManager.Instance.GameSceneSystem;
        sceneSystem.OnSceneLoadProgress += UpdateProgress;
        sceneSystem.OnSceneLoaded += HideLoading;
    }

    private void OnDestroy() {
        var sceneSystem = GameManager.Instance.GameSceneSystem;
        sceneSystem.OnSceneLoadProgress -= UpdateProgress;
        sceneSystem.OnSceneLoaded -= HideLoading;
    }

    private void UpdateProgress(float progress) {
        progressBar.value = progress;
        progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
    }

    private void HideLoading(string _) {
        gameObject.SetActive(false); // 載入完就關閉UI
    }
}
