using UnityEngine;

public static class LayerUtility {
    public static bool CompareLayer(this Component component, string layerName) {
        return component.gameObject.layer == LayerMask.NameToLayer(layerName);
    }
}
