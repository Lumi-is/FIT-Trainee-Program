using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class EnemySword : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    public bool isUsed = true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerKatana")
        {
            enemy.AttackBlocked();   
        }
    }
}
