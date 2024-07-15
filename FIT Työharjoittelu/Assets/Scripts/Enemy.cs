using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStatPreset StatPreset;

    [SerializeField] private Vector3 SpawnPosiion;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = SpawnPosiion;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
