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

    bool TrailKillDelayActive = false;

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
        } else if(!TrailKillDelayActive){
            StartCoroutine(TrailKillDelay());
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

    IEnumerator TrailKillDelay()
    {
        TrailKillDelayActive = true;
        yield return new WaitForSeconds(1);
        if(Speed < TrailSpeedThreshold)
        {
            Trail.SetBool("canSpawn",false);
        }
        TrailKillDelayActive = false;
    }
}
