using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXQualityManager : MonoBehaviour
{
    [SerializeField] VisualEffect Trail;

    // Update is called once per frame
    void Update()
    {
        Trail.SetFloat("spawnRate",1.0f / Time.deltaTime);
    }
}
