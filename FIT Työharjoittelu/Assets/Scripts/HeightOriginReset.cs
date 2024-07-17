using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class HeightOriginReset : MonoBehaviour
{
    private XROrigin xrOrigin;
    // Start is called before the first frame update
    void Start()
    {
        xrOrigin = GetComponent<XROrigin>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
