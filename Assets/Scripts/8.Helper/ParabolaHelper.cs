using UnityEngine;

public static class ParabolaHelper
{
    /// <summary>
    /// �w�������t�סA�D�X���ߪ��u�R���ؼЪ�������t�סC
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

        // --- �����Z���P��V ---
        Vector3 planarDir = new Vector3(targetPosition.x - position.x, 0, targetPosition.z - position.z);
        float distance = planarDir.magnitude;
        if (distance < 0.001f) return false;

        Vector3 dir = planarDir.normalized;
        float g = gravity;
        float v = horizontalSpeed;

        // ---�p��۹�����t�ס]�ߪ��u�t�� vs �ؼгt�ס^---
        Vector3 projVelocity = dir * v;
        Vector3 relativeVel = projVelocity - new Vector3(targetVelocity.x, 0, targetVelocity.z);
        float relSpeed = relativeVel.magnitude;
        if (relSpeed < 0.01f) return false; // �X�G�L�k�l�W

        // ---�w���R���ɶ� ---
        float t = distance / relSpeed;
        if (t <= 0f) return false;

        // ---�w���ؼЦb�R���ɪ���m ---
        Vector3 predictedTarget = targetPosition + targetVelocity * t;

        // --- ���׮t ---
        float dy = targetHeight - height;

        // --- �p�⫫����t�ס]�t�w����m�^---
        verticalSpeed = ((dy) + 0.5f * g * t * t) / t;

        // --- ��������G�t�פӧC�ζZ���ӻ� -> 45�� fallback---
        if (float.IsNaN(verticalSpeed) || Mathf.Abs(verticalSpeed) > v * 5f || t > 5f) {
            // �ϥγ̤j�g�{���סG45�ס]tan(45)=1 -> vy = vx�^
            verticalSpeed = v;
            Debug.LogWarning($"[ParabolaHelper] �����t�׹L�C�ζZ���L���A�ϥ�45�X���X�t�C�Z��={distance:F2}, �����t��={v:F2}");
        }

        // --- �w������ ---
        verticalSpeed = Mathf.Clamp(verticalSpeed, -v * 3f, v * 3f);
        return true;
    }
}
