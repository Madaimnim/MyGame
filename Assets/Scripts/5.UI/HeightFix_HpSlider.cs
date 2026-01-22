using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightFix_HpSlider : MonoBehaviour
{
    [SerializeField] private Transform _spriteTransform;


    private void Awake() {}

    private void LateUpdate() {
        //transform.localPosition = new Vector3(transform.localPosition.x,_spriteTransform.localPosition.y , transform.localPosition.z);
    }
}
