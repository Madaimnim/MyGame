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

    private void Update() {
        if (scrollRect != null)
        {
            // 计算新的滚动位置
            float newPosition = scrollRect.verticalNormalizedPosition - (scrollSpeed * Time.deltaTime);

            // 限制滚动范围
            if (newPosition <= 0f)
            {
                if (loopScroll)
                {
                    newPosition = 1f; // 回到顶部，循环滚动
                }
                else
                {
                    newPosition = 0f; // 停止滚动
                }
            }

            // 设置滚动位置
            scrollRect.verticalNormalizedPosition = newPosition;
        }
    }
}
