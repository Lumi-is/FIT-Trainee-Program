using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InstantKillArea : MonoBehaviour
{
    [SerializeField] PlayerBody player;
    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            player.Die();
        } else if (col.gameObject.tag == "Enemy") {
            Destroy(col.gameObject);
        }
    }
}
