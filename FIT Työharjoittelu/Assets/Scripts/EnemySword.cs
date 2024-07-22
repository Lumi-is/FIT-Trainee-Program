using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemySword : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    public bool isUsed = true;



    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerKatana")
        {
            enemy.AttackBlocked();   
        } else if(other.gameObject.tag == "Player")
        {
            PlayerBody player = other.gameObject.GetComponent<PlayerBody>();
            player.Die();
        }
    }
}
