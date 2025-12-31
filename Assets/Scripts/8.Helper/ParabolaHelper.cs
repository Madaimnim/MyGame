using UnityEngine;

public static class ParabolaHelper
{
    /// <summary>
    /// 已知水平速度，求出讓拋物線命中目標的垂直初速度。
    /// </summary>
    public static bool TryGetVerticalSpeed(
        float gravity,

        Vector3 position,
        float height,
        float horizontalSpeed,

        Vector3 targetPosition,
        float targetHeight,
        Vector3 targetVelocity,

        out float verticalSpeed
        ) {
        verticalSpeed = 0f;

        // --- 水平距離與方向 ---
        Vector3 planarDir = new Vector3(targetPosition.x - position.x, targetPosition.y - position.y);
        float distance = planarDir.magnitude;
        if (distance < 0.001f) return false;

        Vector3 dir = planarDir.normalized;
        float g = gravity;
        float v = horizontalSpeed;

        // ---計算相對水平速度（拋物線速度 vs 目標速度）---
        Vector3 projVelocity = dir * v;
        Vector3 relativeVel = projVelocity - new Vector3(targetVelocity.x, targetVelocity.y);
        float relSpeed = relativeVel.magnitude;
        if (relSpeed < 0.01f) return false; // 幾乎無法追上

        // ---預估命中時間 ---
        float t = distance / relSpeed;
        if (t <= 0f) return false;

        // ---預測目標在命中時的位置 ---
        Vector3 predictedTarget = targetPosition + targetVelocity * t;

        // --- 高度差 ---
        float dy = targetHeight - height;

        // --- 計算垂直初速度（含預測位置）---
        verticalSpeed = ((dy) + 0.5f * g * t * t) / t;

        // --- 極限條件：速度太低或距離太遠 -> 45度 fallback---
        if (float.IsNaN(verticalSpeed) || Mathf.Abs(verticalSpeed) > v * 5f || t > 5f) {
            // 使用最大射程角度：45度（tan(45)=1 -> vy = vx）
            verticalSpeed = v;
            //Debug.LogWarning($"[ParabolaHelper] 水平速度過低或距離過遠，使用45°角出速。距離={distance:F2}, 水平速度={v:F2}");
        }

        // --- 安全限制 ---
        verticalSpeed = Mathf.Clamp(verticalSpeed, -v * 3f, v * 3f);
        return true;
    }
}
