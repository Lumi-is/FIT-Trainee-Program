using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyStatPreset StatPreset;

    [SerializeField] private Transform Player;

    [SerializeField] private Vector3 SpawnPosition;

    [SerializeField] private float EnemyAttackThreshold;

    [SerializeField] private Rigidbody rb;

    [SerializeField] private EnemySword enemySword;
    public float HP;
    public bool canSlice;
    public bool isStunned;
    public bool isStaggered;

    private bool attackActive;

    // Start is called before the first frame update
    void Start()
    {
        HP = StatPreset.MaxHP;
        transform.position = SpawnPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(Player.position, transform.position) > EnemyAttackThreshold)
        {
            RunTowards();
        } else if(!attackActive){
            StartCoroutine(Attack());
        }

        if(HP < 33 || isStaggered)
        {
            canSlice = true;
        } else {
            canSlice = false;
        }
    }

    void RunTowards()
    {
        if(!isStunned)
        {
            rb.velocity = (Player.position - transform.position).normalized * StatPreset.Speed;
        }  
    }

    void StepBack()
    {
        if(StatPreset.canDodge)
        {
            Vector3 dodgeStep = (Player.position - transform.position).normalized * StatPreset.dodgeDistance * -1;
            rb.AddForce(dodgeStep);
        }    
    }

    IEnumerator Attack()
    {
        attackActive = true;
        //Do basic hit from randomized direction with the speed of preset attribute attackSpeed
        yield return null; 
        attackActive = false;
    }

    public void TakeDamage(float TakenDamage, float StunDuration)
    {
        Mathf.Clamp(TakenDamage,0,StatPreset.MaxDamageTaken);
        HP = HP - TakenDamage;
        StepBack();
        StartCoroutine(Stun(StunDuration));
    }

    public void AttackBlocked()
    {
        HP =- StatPreset.blockDamage;
    }
    public void StunPublic(float stunDuration)
    {
        StartCoroutine(Stun(stunDuration));
    }

    IEnumerator Stun(float stunDuration)
    {
        isStunned = true;
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
    }
}
