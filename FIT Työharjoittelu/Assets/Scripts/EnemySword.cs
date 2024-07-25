using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemySword : MonoBehaviour
{
    public Enemy enemy;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PlayerKatana"))
        {
            enemy.AttackBlocked();   
        } else if(other.gameObject.CompareTag("Player"))
        {
            PlayerBody player = other.gameObject.GetComponent<PlayerBody>();
            player.Die();
        }
    }
}
