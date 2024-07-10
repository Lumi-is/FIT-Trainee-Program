using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class SwordOffsetManager : MonoBehaviour
{
    public float p_xOffset;
    public float p_yOffset;
    public float p_zOffset;

    void Update()
    {
        transform.localPosition = new Vector3(p_xOffset, p_yOffset, p_zOffset);
    }
}
