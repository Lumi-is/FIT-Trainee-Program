using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordVariables : MonoBehaviour
{
    Vector3 Frame1Pos; 
    Vector3 Frame2Pos;

    public float Speed; 

    void Start()
    {
        Frame1Pos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Frame1Pos = Frame2Pos; 
        Frame2Pos = transform.position;

        Speed = Vector3.Distance(Frame2Pos, Frame1Pos) * 60;
    }
}
