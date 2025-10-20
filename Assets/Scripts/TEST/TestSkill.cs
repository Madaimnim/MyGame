using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkill : MonoBehaviour
{
    public LayerMask TargetLayers;
    public Collider2D BottomCollider;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & TargetLayers) != 0)
        {

            IInteractable damageable = collision.GetComponent<IInteractable>();
            

   
        }
    }


}
