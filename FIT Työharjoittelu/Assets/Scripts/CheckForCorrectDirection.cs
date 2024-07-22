using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForCorrectDirection : MonoBehaviour
{

    [SerializeField] BladeModeScript CutScript;
    // Start is called before the first frame update

    void OnTriggerEnter(Collider other)
    {
        if((CutScript.layerMask.value & 1<< other.gameObject.layer) != 0<<other.gameObject.layer && CutScript.canSlice)
        {
            if(CutScript.bladeHitCorrectly){
                CutScript.DoDamage(other.gameObject.GetComponent<Enemy>());
                CutScript.StartSlicing(other.gameObject.GetComponent<Enemy>());
                CutScript.StartSliceCooldown(0.1f);
            } else {
                CutScript.bladeHitIncorrectly = true;
                CutScript.StartSliceCooldown(0.1f); 
            }
        }
    }
}
