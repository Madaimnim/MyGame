using UnityEngine;
using UnityEngine.UI;

public class AutoScrollView : MonoBehaviour
{
    [Header("Scroll View 组件")]
    public ScrollRect scrollRect; // 需要指定 ScrollRect

    [Header("滚动速度 (0.1~1.0，值越大滚动越快)")]
    [Range(0.01f, 1f)]
    public float scrollSpeed = 0.1f; // 可在 Inspector 调整滚动速度

    [Header("是否循环滚动")]
    public bool loopScroll = false; // 是否循环滚动

    private bool isInitialized = false;

    private void Start() {
        // 强制确保 `Content` 一开始在顶部
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }

    private void LateUpdate() {
        // 确保 `ScrollRect` 在所有 Unity 内部计算后仍然保持在顶部
        if (!isInitialized && scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
            isInitialized = true;
        }
    }

    private void Update() {
        if (scrollRect != null && isInitialized)
        {
            float newPosition = scrollRect.verticalNormalizedPosition - (scrollSpeed * Time.deltaTime);

            if (newPosition <= 0f)
            {
                if (loopScroll)
                {
                    newPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition, 1f, Time.deltaTime * 2f);
                }
                else
                {
                    newPosition = 0f;
                }
            }

            scrollRect.verticalNormalizedPosition = newPosition;
        }
    }
}
