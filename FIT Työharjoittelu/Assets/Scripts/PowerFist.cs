using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR.Interaction.Toolkit;

public class PowerFist : MonoBehaviour
{
    public PlayerPowerUps playerPowerUps;
    public float Speed;    
    public float speedThreshold;
    Vector3 Frame1Pos;
    Vector3 Frame2Pos;

    Vector3 HitDirection;

    public bool isCharged;

    public bool canAttack;

    [Range(0,100)]
    [SerializeField] float HitForce;

    [SerializeField] XRBaseController controller;
    [SerializeField] VisualEffect PunchImpactVFX;

    [Header("Audio")]
    [SerializeField] AudioSource punchSFX;

    // Start is called before the first frame update
    void Start()
    {
        //Debug
        //isCharged = true;
        PunchImpactVFX.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Speed > speedThreshold && isCharged)
        {
            canAttack = true;
        } else {
            canAttack = false;
        }

    }

    void FixedUpdate()
    {
        Frame1Pos = Frame2Pos; 
        Frame2Pos = transform.position;

        HitDirection = Frame2Pos - Frame1Pos;

        Speed = Vector3.Distance(Frame2Pos, Frame1Pos) * Time.deltaTime * 10000;
    }

    void OnTriggerEnter(Collider col)
    {
        if(canAttack && col.gameObject.tag == "Enemy")
        {
            isCharged = false;
            Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
            Enemy enemy = col.gameObject.GetComponent<Enemy>();
            controller.SendHapticImpulse(1,0.5f);
            enemy.TakeDamage(100, 5, false, HitDirection.normalized * HitForce * Speed * playerPowerUps.HitforceBoostAmount);

            PunchImpactVFX.transform.LookAt(col.gameObject.transform);
            StartCoroutine(PlayPunchImpactFX());
        }
    }

    IEnumerator PlayPunchImpactFX()
    {
        PunchImpactVFX.enabled = true;
        PunchImpactVFX.SendEvent("Play");
        punchSFX.Play();
        yield return new WaitForSeconds(2);
        PunchImpactVFX.enabled = false;
    }
}
