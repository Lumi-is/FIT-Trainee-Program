using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Unity.XR.CoreUtils;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyStatPreset StatPreset;
    public Transform player;
    [SerializeField] EnemyGroundCheck enemyGroundCheck;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private NavMeshAgent agent;

    public float HP;
    public bool canSlice;
    public bool isStunned;
    public bool isStaggered;

    [Header("Attacking")]
    [SerializeField] private GameObject enemySword;
     private Collider enemySwordCollider;
    [SerializeField] bool readyToAttack = true;
    [SerializeField] bool attacking = false;
    [SerializeField] private float approachRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float approachSpeed;
    [SerializeField] private bool isGrounded = true;


    [Header("State Machine")]
    [SerializeField] Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        HP = StatPreset.MaxHP;
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = enemyGroundCheck.isGrounded;
        player = FindAnyObjectByType<XROrigin>().transform;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if(distanceToPlayer > approachRange && !attacking)
        {
            anim.SetBool("IsPlayerInApproachRange", false);
            anim.SetBool("IsPlayerInAttackRange", false);
            //RunTowards();
        }
        else if(distanceToPlayer < approachRange && distanceToPlayer > attackRange && !attacking)
        {
            anim.SetBool("IsPlayerInApproachRange", true);
            anim.SetBool("IsPlayerInAttackRange", false);
            //Attack();
        }
        else
        {
            anim.SetBool("IsPlayerInAttackRange", true);
        }

        if(HP < 33 || isStaggered)
        {
            canSlice = true;
        } else {
            canSlice = false;
        }

        if(readyToAttack && !attacking && !isStunned && !isStaggered)
        {
            anim.SetBool("canAttack", true);
        }
        else
        {
            anim.SetBool("canAttack", false);
        }
    }

    void Idle()
    {

    }

    public void Chase()
    {
        agent.destination = player.position;
        agent.speed = chaseSpeed;
    }
    public void Approach()
    {
        agent.destination = player.position;
        agent.speed = approachSpeed;
    }

    public void Attack()
    {
        readyToAttack = false;
        attacking = true;

        //enemySwordCollider.enabled = true;

        Debug.Log("Enemy Attacked");

        Invoke(nameof(AttackEnd), 1f);
    }
    void AttackEnd()
    {
        //enemySwordCollider.enabled = false;
        attacking = false;
        Invoke(nameof(ResetAttack), 0.4f);
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


    public void TakeDamage(float TakenDamage, float StunDuration, bool swordKb, Vector3 hitDir)
    {
        Mathf.Clamp(TakenDamage,0,StatPreset.MaxDamageTaken);
        HP = HP - TakenDamage;
        KnockBack(swordKb, hitDir);
        StartCoroutine(Stun(StunDuration));
    }

    void KnockBack(bool swordKb, Vector3 kbDirection)
    {
        if(swordKb)
        {
            SwordKnockBack();
        } else {
            FistKnockBack(kbDirection);
        }
    }

    void SwordKnockBack()
    {
        if(StatPreset.canDodge)
        {
            CancelInvoke(nameof(EnableAgent));
            DisableAgent();
            Vector3 dodgeStep = (player.position - transform.position).normalized * StatPreset.dodgeDistance * -1;
            rb.velocity = Vector3.zero;
            rb.AddForce(new Vector3(dodgeStep.x, 0, dodgeStep.z));
            Invoke(nameof(EnableAgent), 1f);
        }
    }

    void FistKnockBack(Vector3 kbDirection)
    {
        CancelInvoke(nameof(EnableAgent));
        DisableAgent();
        rb.velocity = Vector3.zero;
        rb.AddForce(kbDirection);
        Invoke(nameof(EnableAgent), 1f);
    }

    IEnumerator Stun(float stunDuration)
    {
        
        anim.Play("Stun");
        isStunned = true;
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
        anim.SetBool("IsStunned",false);
    }

    IEnumerator AgentGroundCheck()
    {
        if(isGrounded)
        {
            agent.enabled = true;
            rb.isKinematic = true;
            yield break;
        } else {
            yield return null;
            StartCoroutine(AgentGroundCheck());
        }     
    }

    void EnableAgent()
    {
        StartCoroutine(AgentGroundCheck());
    }

    void DisableAgent()
    {
        agent.enabled = false;
        rb.isKinematic = false;
    }
}
