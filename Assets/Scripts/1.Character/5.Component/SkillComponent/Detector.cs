using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Detector
{
    public bool HasTarget => _targetDetector.HasTarget;
    public Transform TargetTransform => _targetDetector?.TargetTransform;
    public Collider2D GetDetectorCollider() => _detectorObj.GetComponent<Collider2D>();

    private GameObject _detectorObj;
    private TargetDetector _targetDetector;

    public Detector(GameObject prefab, Transform parent, string name) {
        if (prefab == null) throw new ArgumentNullException(nameof(prefab));

        _detectorObj = UnityEngine.Object.Instantiate(prefab, parent);
        _detectorObj.name = name;
        _detectorObj.transform.localPosition = Vector2.zero;

        _targetDetector = _detectorObj.GetComponent<TargetDetector>();
    }

    public void Destroy() {
        if (_detectorObj != null)
        {
            UnityEngine.Object.Destroy(_detectorObj);
            _detectorObj = null;
            _targetDetector = null;
        }
    }
}
