using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SwordVariables : MonoBehaviour
{
    Vector3 Frame1Pos;
    Vector3 Frame2Pos;

    //public Vector3 PosDifference;

    public float Speed;
    [SerializeField] private float TrailSpeedThreshold;
    [SerializeField] VisualEffect Trail;

    void Start()
    {
        Frame1Pos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Frame1Pos = Frame2Pos; 
        Frame2Pos = transform.position;

        Speed = Vector3.Distance(Frame2Pos, Frame1Pos) * Time.deltaTime * 10000;
    }

    void Update()
    {
        if(Speed > TrailSpeedThreshold)
        {
            Trail.SetBool("canSpawn",true);
        } else {
            Trail.SetBool("canSpawn",false);
        }

        //PosDifference = Frame2Pos - Frame1Pos;

        //Vector3 bladeRelative = bootleg.InverseTransformPoint(transform.position);

        //if(bladeRelative.z < relative)
        //{
        //    trail.SetBool("canSpawn", true);s
        //}
        //else
        //{
        //    trail.SetBool("canSpawn", false);
        //}
    }
}
