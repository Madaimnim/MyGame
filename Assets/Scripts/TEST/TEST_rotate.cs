using System.Collections;
using UnityEngine;

public class TEST_rotate : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float rotateAngle = 30f; // 每秒旋转的角度
    public float rotateDuration = 0.3f;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        StartCoroutine(RotateTest()); // 启动旋转协程
    }

    private IEnumerator RotateTest() {
        while (true)
        {
            Vector2 spriteCenter = spriteRenderer.bounds.center; // 获取 Sprite 世界中心点

            // **围绕 Sprite 自身中心旋转 `rotateAngle`**
            transform.RotateAround(spriteCenter, Vector3.forward, rotateAngle);

            yield return new WaitForSeconds(rotateDuration); // **每秒旋转一次**
        }
    }
}
