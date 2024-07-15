using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.VFX;
using UnityEngine.XR.Interaction.Toolkit;

public class BladeModeScript : MonoBehaviour
{
    public bool bladeMode;

    public Vector3 zoomOffset;

    public float zoomFOV = 15;

    public Transform cutPlane;

    public Material crossMaterial;
    public Material newMat;

    public LayerMask layerMask;

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

    [SerializeField] XRBaseController RightController;


    [Header("Raycast")]
    [SerializeField] private VisualEffect vfx;
    [SerializeField] private Transform rayCastStart;
    [SerializeField] private Transform rayCastEnd;
    [SerializeField] private Vector3 rayCastDir;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //cutPlane.gameObject.SetActive(false);
    }

    void Update()
    {
        if(BladeInside)
        {
            RightController.SendHapticImpulse(1,0.1f);
            Slowdown = true;
        } else {
            Slowdown = false;
        }

        rayCastDir = (rayCastEnd.position - rayCastStart.position).normalized;
        float raysCastDistance = Vector3.Distance (rayCastStart.transform.position, rayCastEnd.transform.position);

        bool bladeRaycast = Physics.Raycast(rayCastStart.position, rayCastDir, out RaycastHit hit, raysCastDistance);

        if(bladeRaycast && ((layerMask.value & 1<< hit.transform.gameObject.layer) != 0<<hit.transform.gameObject.layer))
        {
            vfx.gameObject.SetActive(true);
            vfx.transform.position = hit.point;
        }
        else
        {
            vfx.gameObject.SetActive(false);
        }
    }

    public void Slice()
    {
        Collider[] hits = Physics.OverlapBox(cutPlane.position, new Vector3(0.1f, 0.01f, 0.1f), cutPlane.rotation, layerMask);

        if (hits.Length <= 0)
            return;

        for (int i = 0; i < hits.Length; i++)
        {
            hits[i].GetComponent<Renderer>().material = newMat;

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
        rb.useGravity = false;

        StartCoroutine(Dissolve(go));

        rb.AddExplosionForce(100, go.transform.position, 20);

        Destroy(go, 2f);
    }

    IEnumerator Dissolve(GameObject go)
    {
        float t = -1f;
        while(t < 2)
        {
            t += Time.deltaTime;
            if(go != null)
                go.GetComponent<Renderer>().material.SetFloat("_DissolveTime",t);
            yield return null;
        }

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
