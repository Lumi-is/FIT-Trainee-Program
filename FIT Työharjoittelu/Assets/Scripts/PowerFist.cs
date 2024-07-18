using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PowerFist : MonoBehaviour
{
    public float Speed;    
    public float speedThreshold;
    Vector3 Frame1Pos;
    Vector3 Frame2Pos;

    Vector3 HitDirection;

    public bool isCharged;

    public bool canAttack;

    [Range(0,100)]
    [SerializeField] float HitForce;

    // Start is called before the first frame update
    void Start()
    {
        //Debug
        isCharged = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Speed > speedThreshold && isCharged)
        {
            canAttack = true;
        } else {
            canAttack = false;
        }
    }

    void FixedUpdate()
    {
        Frame1Pos = Frame2Pos; 
        Frame2Pos = transform.position;

        HitDirection = Frame2Pos - Frame1Pos;

        Speed = Vector3.Distance(Frame2Pos, Frame1Pos) * Time.deltaTime * 10000;
    }

    void OnTriggerEnter(Collider col)
    {
        if(canAttack && col.gameObject.tag == "Enemy")
        {
            Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
            Enemy enemy = col.gameObject.GetComponent<Enemy>();
            rb.velocity = HitDirection.normalized * Speed * HitForce;
            enemy.TakeDamage(100, Speed);
        }
    }
}
