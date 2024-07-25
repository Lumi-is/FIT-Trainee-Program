using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.VFX;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor.Rendering;

public class BladeModeScript : MonoBehaviour
{
    [SerializeField] PlayerPowerUps playerPowerUps;

    public bool hasAcquiredPowerUp = false;

    [SerializeField] LevelManager levelManager;

    public Transform cutPlane;

    public Material crossMaterial;
    public Material newMat;

    public LayerMask layerMask;
    public LayerMask otherLayerMask;

    public bool bladeHitCorrectly;

    public bool bladeHitIncorrectly;

    public bool canAttack = true; 

    public bool BladeInside;

    private bool CuttingSuccesful;

    [Header("Audio")]
    [SerializeField] AudioSource damageSound1;
    [SerializeField] AudioSource damageSound2;
    [SerializeField] AudioSource damageSound3;
    int dmgSoundListIndex = 0;

    [SerializeField] AudioSource killSound;


    //[SerializeField] private float RotationThreshold;
    //public bool xRotationConstant;
    //public bool yRotationConstant;
    //public bool zRotationConstant;

    [Header("Variables")]
    [SerializeField] SwordVariables swordVariables;

    public float speedThreshold;

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
        if(BladeInside && CuttingSuccesful)
        {
            RightController.SendHapticImpulse(1,0.1f);
        } 

        rayCastDir = (rayCastEnd.position - rayCastStart.position).normalized;
        float raysCastDistance = Vector3.Distance (rayCastStart.transform.position, rayCastEnd.transform.position);

        bool bladeRaycast = Physics.Raycast(rayCastStart.position, rayCastDir, out RaycastHit hit, raysCastDistance);

        if(bladeRaycast && ((layerMask.value & 1<< hit.transform.gameObject.layer) != 0<<hit.transform.gameObject.layer))
        {
            vfx.SetBool("canSpawn",true);
            vfx.transform.position = hit.point;
            Vector3 hitNormal = hit.normal;
            vfx.SetVector3("hitNormal", hitNormal);

            Quaternion quaternion = Quaternion.FromToRotation(Vector3.forward, hitNormal);
            vfx.SetVector3("hitNormalAngle", new Vector3(quaternion.eulerAngles.x, quaternion.eulerAngles.y, quaternion.eulerAngles.z));
        }
        else
        {
            vfx.SetBool("canSpawn",false);
        }
        
    }

    public void Slice()
    {
        Collider[] hits = Physics.OverlapBox(cutPlane.position, new Vector3(0.5f, 0.01f, 0.5f), cutPlane.rotation, layerMask);

        if (hits.Length <= 0)
            return;

        hasAcquiredPowerUp = false;

        for (int i = 0; i < hits.Length; i++)
        {
            hits[i].GetComponent<Renderer>().material = newMat;

            if(hits[i].tag == "DamageBoost" && !hasAcquiredPowerUp) {
                playerPowerUps.DamageBoostAmount = playerPowerUps.DamageBoostAmount + 0.25f;
                hasAcquiredPowerUp = true;
                levelManager.LoadNextLevel(1);
            } else if(hits[i].tag == "SpeedBoost" && !hasAcquiredPowerUp){
                hasAcquiredPowerUp = true;
                playerPowerUps.SpeedBoostAmount = playerPowerUps.SpeedBoostAmount + 0.25f;
                levelManager.LoadNextLevel(1);
            } else if(hits[i].tag == "JumpBoost" && !hasAcquiredPowerUp) {
                hasAcquiredPowerUp = true;
                playerPowerUps.JumpBoostAmount = playerPowerUps.JumpBoostAmount + 1f;
                levelManager.LoadNextLevel(1);
            } else if(hits[i].tag == "DashBoost" && !hasAcquiredPowerUp) {
                hasAcquiredPowerUp = true;
                playerPowerUps.DashBoostAmount = playerPowerUps.DashBoostAmount + 0.1f;
                levelManager.LoadNextLevel(1);
            } else if(hits[i].tag == "HitForceBoost" && !hasAcquiredPowerUp) {
                hasAcquiredPowerUp = true;
                playerPowerUps.HitforceBoostAmount = playerPowerUps.HitforceBoostAmount + 0.5f;
                levelManager.LoadNextLevel(1);
            } else if(hits[i].tag == "UndyingBoost" && !hasAcquiredPowerUp) {
                hasAcquiredPowerUp = true;
                playerPowerUps.Undying = true;
                levelManager.LoadNextLevel(1);
            }  //Add kb

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
        float t = -1.5f;
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



    void Debug()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }


    

    void OnTriggerEnter(Collider other)
    {
        if((layerMask.value & 1<< other.gameObject.layer) != 0<<other.gameObject.layer && canAttack && swordVariables.Speed > speedThreshold)
        {
            if(!bladeHitIncorrectly){
                bladeHitCorrectly = true;
            } else {
                StartCoroutine(SliceCooldown(0.1f));
                //Incorrect direction cooldown
            }
        }

        if((otherLayerMask.value & 1<< other.gameObject.layer) != 0<<other.gameObject.layer)
        {
            if(CuttingSuccesful)
            {
                if(other.transform.parent.gameObject.GetComponent<Enemy>() != null)
                {
                    Enemy enemy = other.transform.parent.gameObject.GetComponent<Enemy>();
                    if(enemy.canSlice)
                    {
                        Slice();
                        killSound.Play();
                        dmgSoundListIndex = 0;
                    }  
                } else {
                    Slice();
                    killSound.Play();
                    dmgSoundListIndex = 0;
                }          
            } 

            CuttingSuccesful = false;
            BladeInside = false;            
        }
    }

    public void StartSliceCooldown(float Cooldown)
    {
        StartCoroutine(SliceCooldown(Cooldown));
    }

    IEnumerator SliceCooldown(float Cooldown)
    {
        canAttack = false;
        yield return new WaitForSeconds(Cooldown);
        canAttack = true;
        bladeHitCorrectly = false;
        bladeHitIncorrectly = false;
    }

    public void StartSlicing(Enemy enemy)
    {
        if(enemy.HP <= 0)
        {
            BladeInside = true;
            StartCoroutine(SliceAction());
        }
    }

    IEnumerator SliceAction()
    {
        if(swordVariables.Speed > speedThreshold)
        {
            CuttingSuccesful = true;
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(SliceAction());
        } else {
            CuttingSuccesful = false;
            yield break;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if((layerMask.value & 1<< other.gameObject.layer) != 0<<other.gameObject.layer)
        {
            if(CuttingSuccesful)
            {
                if(other.gameObject.GetComponent<Enemy>() != null)
                {
                    Enemy enemy = other.gameObject.GetComponent<Enemy>();
                    if(enemy.canSlice)
                    {
                        Slice();
                        killSound.Play();
                        dmgSoundListIndex = 0;
                    }  
                } else {
                    Slice();
                    killSound.Play();
                    dmgSoundListIndex = 0;
                }          
            } 

            CuttingSuccesful = false;
            BladeInside = false;            
        }
    }

    public void DoDamage(Enemy enemy)
    {
        if(enemy.HP > 0 && enemy != null)
        {
            RightController.SendHapticImpulse(1,0.2f);
            enemy.TakeDamage(swordVariables.Speed*playerPowerUps.DamageBoostAmount, 1, true, new Vector3(0,0,0));
            if(dmgSoundListIndex == 0)
            {
                damageSound1.Play();
                dmgSoundListIndex++;
            } else if(dmgSoundListIndex == 1){
                damageSound2.Play();
                dmgSoundListIndex++;
            } else {
                damageSound3.Play();
                dmgSoundListIndex = 0;
            }
        }
    }
}
