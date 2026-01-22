using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollider : MonoBehaviour
{
    private GroundHitProxy _ground;

    private void Awake() {
        _ground = GetComponentInChildren<GroundHitProxy>();

        if (_ground != null) {
            _ground.OnGroundHit += OnGroundColliderHit;
            Debug.Log("GroundHit­q¾\¦¨¥\");
        }

    }

    private void OnGroundColliderHit(Collider2D other) {
        Debug.Log($"{other.transform.name}");
    }
}
