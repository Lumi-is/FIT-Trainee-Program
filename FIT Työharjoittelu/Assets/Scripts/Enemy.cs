using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyStatPreset StatPreset;

    [SerializeField] private Transform Player;

    [SerializeField] private Vector3 SpawnPosition;

    [SerializeField] private Rigidbody rb;

    public float HP;
    public bool canSlice;
    public bool isStunned;
    public bool isStaggered;

    [Header("Attacking")]
    [SerializeField] private GameObject enemySword;
     private Collider enemySwordCollider;
    [SerializeField] bool readyToAttack = true;
    [SerializeField] bool attacking = false;
    [SerializeField] private float attackRange;


    [Header("Animation")]
    [SerializeField] Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        HP = StatPreset.MaxHP;
        transform.position = SpawnPosition;

        enemySwordCollider = enemySword.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(Player.position, transform.position) > attackRange)
        {
            RunTowards();
        }
        else
        {
            Attack();
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

    void Attack()
    {
        if(!readyToAttack || attacking || isStunned) return;
        
        readyToAttack = false;
        attacking = true;

        enemySwordCollider.enabled = false;

        anim.SetTrigger("Trigger");

        Invoke(nameof(AttackEnd), 1f);
    }
    void AttackEnd()
    {
        enemySwordCollider.enabled = false;
        attacking = false;
    }
    void ResetAttack()
    {
        readyToAttack = true;
    }


    public void AttackBlocked()
    {
        HP =- StatPreset.blockDamage;
        //Play animation

        //PlaceHolder
        anim.Play("Idle");
    }


    public void TakeDamage(float TakenDamage, float StunDuration)
    {
        Mathf.Clamp(TakenDamage,0,StatPreset.MaxDamageTaken);
        HP = HP - TakenDamage;
        StepBack();
        StartCoroutine(Stun(StunDuration));
    }

    IEnumerator Stun(float stunDuration)
    {
        isStunned = true;
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
    }
}
