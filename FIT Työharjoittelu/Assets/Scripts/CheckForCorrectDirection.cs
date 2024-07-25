using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForCorrectDirection : MonoBehaviour
{

    [SerializeField] BladeModeScript CutScript;
    // Start is called before the first frame update

    void OnTriggerEnter(Collider other)
    {
        if((CutScript.layerMask.value & 1<< other.gameObject.layer) != 0<<other.gameObject.layer && CutScript.canAttack)
        {
            if(CutScript.bladeHitCorrectly){
                if(other.gameObject.GetComponent<Enemy>() != null)
                {
                    CutScript.DoDamage(other.gameObject.GetComponent<Enemy>());
                    CutScript.StartSlicing(other.gameObject.GetComponent<Enemy>());
                    CutScript.StartSliceCooldown(0.1f);

                } else if(other.gameObject.tag == "Boss")
                {
                    CutScript.SendHapticImpulse(0.2f);
                } else {
                    CutScript.Slice();
                    CutScript.StartSliceCooldown(0.1f);
                    //Play sfx
                }
            } else {
                CutScript.bladeHitIncorrectly = true;
                CutScript.StartSliceCooldown(0.1f); 
            }
        }
    }
}
