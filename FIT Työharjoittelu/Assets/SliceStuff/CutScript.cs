using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.Rendering;
using NUnit.Framework;

public class BladeModeScript : MonoBehaviour
{
    public bool bladeMode;

    public Vector3 zoomOffset;

    public float zoomFOV = 15;

    public Transform cutPlane;

    public Material crossMaterial;

    public LayerMask layerMask;
    ParticleSystem[] particles;

    public bool bladeHitCorrectly;

    public bool bladeHitIncorrectly;

    public bool canSlice = true; 

    public bool Slowdown;

    public bool BladeInside;

    private Quaternion RememberRotation;

    private bool CuttingSuccesful;

    [SerializeField] private float RotationThreshold;
    public bool xRotationConstant;
    public bool yRotationConstant;
    public bool zRotationConstant;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //cutPlane.gameObject.SetActive(false);

        particles = cutPlane.GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        if(BladeInside)
        {
            Slowdown = true;
        } else {
            Slowdown = false;
        }
        if (Input.GetMouseButtonDown(1))
        {
            Zoom(true);
        }

        if (Input.GetMouseButtonUp(1))
        {
            Zoom(false);
        }

        if (bladeMode)
        {

            if (Input.GetMouseButtonDown(0))
            {
                Slice();
            }
        }

        Debug();
    }

    public void Slice()
    {
        Collider[] hits = Physics.OverlapBox(cutPlane.position, new Vector3(5, 0.1f, 5), cutPlane.rotation, layerMask);

        if (hits.Length <= 0)
            return;

        for (int i = 0; i < hits.Length; i++)
        {
            SlicedHull hull = SliceObject(hits[i].gameObject, crossMaterial);
            if (hull != null)
            {
                GameObject bottom = hull.CreateLowerHull(hits[i].gameObject, crossMaterial);
                GameObject top = hull.CreateUpperHull(hits[i].gameObject, crossMaterial);
                AddHullComponents(bottom);
                AddHullComponents(top);
                Destroy(hits[i].gameObject);
            }
        }
    }

    public void AddHullComponents(GameObject go)
    {
        go.layer = 9;
        Rigidbody rb = go.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        MeshCollider collider = go.AddComponent<MeshCollider>();
        collider.convex = true;

        rb.AddExplosionForce(100, go.transform.position, 20);
    }

    public SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial = null)
    {
        // slice the provided object using the transforms of this object
        if (obj.GetComponent<MeshFilter>() == null)
            return null;

        return obj.Slice(cutPlane.position, cutPlane.up, crossSectionMaterial);
    }

    public void Zoom(bool state)
    {
        bladeMode = state;

        cutPlane.gameObject.SetActive(state);
    }



    void Debug()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    

    void OnTriggerEnter(Collider other)
    {
        if((layerMask.value & 1<< other.gameObject.layer) != 0<<other.gameObject.layer && canSlice)
        {
            if(!bladeHitIncorrectly){
                bladeHitCorrectly = true;
            } else {
                StartCoroutine(SliceCooldown(1));
            }
        }
    }

    public void StartSliceCooldown(float Cooldown)
    {
        StartCoroutine(SliceCooldown(Cooldown));
    }

    IEnumerator SliceCooldown(float Cooldown)
    {
        canSlice = false;
        yield return new WaitForSeconds(0.2f);
        canSlice = true;
        bladeHitCorrectly = false;
        bladeHitIncorrectly = false;
    }

    public void StartSlicing()
    {
        RememberRotation = transform.rotation;
        BladeInside = true;
        StartCoroutine(SliceAction());
    }

    IEnumerator SliceAction()
    {
        if(BladeInside)
        {
            yield return null;
            if(transform.rotation.x < RememberRotation.x + RotationThreshold && transform.rotation.x > RememberRotation.x - RotationThreshold)
            {
                xRotationConstant = true;
            } else {
                xRotationConstant = false;
            }
            if(transform.rotation.y < RememberRotation.y + RotationThreshold && transform.rotation.y > RememberRotation.y - RotationThreshold)
            {
                yRotationConstant = true;
            } else {
                yRotationConstant = false;
            }
            if(transform.rotation.z < RememberRotation.z + RotationThreshold && transform.rotation.z > RememberRotation.z - RotationThreshold)
            {
                zRotationConstant = true;
            } else {
                zRotationConstant = false;
            }


            if(xRotationConstant && yRotationConstant && zRotationConstant)
            {
                CuttingSuccesful = true;
                StartCoroutine(SliceAction());
            } else {
                CuttingSuccesful = false;
            }
        } else {
            yield break;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if((layerMask.value & 1<< other.gameObject.layer) != 0<<other.gameObject.layer)
        {
            if(CuttingSuccesful)
            {
               Slice(); 
            } 

            CuttingSuccesful = false;
            BladeInside = false;            
        }
    }
}

//canSlice = false;
//Slice();
//StartCoroutine(SliceCooldown());

/*
- Check whether the blade hitboxes are triggered in the correct order when hitting an enemy

- upon impact, start slowdown and a constant check for changes in the blade trajectory - threshold configurable for trajectory change required for failing

- VFX for cutting 

- after exiting the hitbox, do the slice
*/

