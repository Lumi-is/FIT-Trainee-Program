using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemySword : MonoBehaviour
{
    public Enemy enemy;
    public bool isUsed = true;



    void OnCollisionEnter(Collision other)
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
