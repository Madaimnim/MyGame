using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    public LayerMask targetLayers;
    public bool hasTarget = false;
    public Transform targetTransform;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & targetLayers) != 0)
        {
            hasTarget = true;
            targetTransform = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & targetLayers) != 0)
        {
            hasTarget = false;
            targetTransform = null;
        }
    }
}
