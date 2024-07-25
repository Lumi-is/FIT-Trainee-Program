using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundCheck : MonoBehaviour
{

    [SerializeField] LayerMask GroundLayer;
    public bool isGrounded = true;
    
    void OnTriggerEnter(Collider col)
    {
        if((GroundLayer.value & 1<< col.gameObject.layer) != 0<<col.gameObject.layer)
        {
            isGrounded = true;
        }
    }

    void OnTriggerStay(Collider col)
    {
        if((GroundLayer.value & 1<< col.gameObject.layer) != 0<<col.gameObject.layer)
        {
            isGrounded = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if((GroundLayer.value & 1<< col.gameObject.layer) != 0<<col.gameObject.layer)
        {
            isGrounded = false;
        }
    }
}
