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
    [SerializeField] private NavMeshObstacle obstacle;

    public float HP;
    public bool canSlice;
    public bool isStunned;
    public bool isStaggered;

    [Header("Attacking")]
    [SerializeField] Collider enemySwordCollider;
    [SerializeField] bool attacking = false;
    [SerializeField] private float approachRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float approachSpeed;
    [SerializeField] private bool isGrounded = true;

    [SerializeField] private float attackRate = 1;


    [Header("State Machine & Animation")]
    [SerializeField] Animator states;
    [SerializeField] Animator animator;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject rigModel;

    // Start is called before the first frame update
    void Start()
    {
        HP = StatPreset.MaxHP;
        obstacle = transform.GetComponent<NavMeshObstacle>();
        player = FindAnyObjectByType<PlayerBody>().transform;
        meshRenderer = transform.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = enemyGroundCheck.isGrounded;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if(distanceToPlayer > approachRange)
        {
            states.SetBool("IsPlayerInApproachRange", false);
            states.SetBool("IsPlayerInAttackRange", false);
        }
        else if(distanceToPlayer < approachRange && distanceToPlayer > attackRange)
        {
            states.SetBool("IsPlayerInApproachRange", true);
            states.SetBool("IsPlayerInAttackRange", false);
        }
        else if(distanceToPlayer < attackRange)
        {
            states.SetBool("IsPlayerInAttackRange", true);
            states.SetBool("IsPlayerInApproachRange", false);
        }
    }

    public void Idle()
    {
        
    }

    public void Stun()
    {

    }

    public void Stagger()
    {
        states.SetBool("IsStaggered",true);
        states.Play("Staggered");

        canSlice = true;

        Invoke(nameof(Regen), 5f);
        meshRenderer.enabled = true;
        rigModel.SetActive(false);

        DisableAgent();
    }
    void Regen()
    {
        states.SetBool("IsStaggered",false);

        HP = StatPreset.HP /2;
        canSlice = false;

        rigModel.SetActive(true);
        meshRenderer.enabled = false;

        EnableAgent();
    }

    public void Chase()
    {
        if(agent.enabled)
        {
            agent.destination = player.position;
            agent.speed = chaseSpeed;
            animator.Play("Walk");
        }
    }
    public void Approach()
    {
        if(agent.enabled)
        {
            agent.destination = player.position;
            agent.speed = approachSpeed;
            animator.Play("Walk");
        }
    }

    public void Attacking()
    {
        if(agent.enabled)
        {
            agent.speed = 0;
        }

        if(!attacking)
        {
            StartCoroutine(Attack());
        }
    }
    IEnumerator Attack()
    {
        yield return new WaitForSeconds(.5f);
        if(isStunned) yield break;
        attacking = true;

        animator.Play("Attack");
        yield return new WaitForSeconds(0.32f);
        enemySwordCollider.enabled = true;

        yield return new WaitForSeconds(0.11f);
        enemySwordCollider.enabled = false;
        
        yield return new WaitForSeconds(attackRate - 0.43f);
        attacking = false;
    }
    public void StopAttack()
    {
        StopCoroutine(Attack());
        attacking = false;
        

        if(enemySwordCollider.enabled)
            enemySwordCollider.enabled = false;
    }


    public void AttackBlocked()
    {
        StopAttack();
        HP =- StatPreset.blockDamage;
        if(HP <= 0)
        {
            Stagger();
        }
        else
            StartCoroutine(AttackBlockedStun());
    }
    IEnumerator AttackBlockedStun()
    {
        isStunned = true;
        animator.Play("Parried");
        states.SetBool("IsStunned",true);
        states.Play("Stunned");
        yield return new WaitForSeconds(1f);
        states.SetBool("IsStunned",false);
        isStunned = false;
    }


    public void TakeDamage(float TakenDamage, float StunDuration, bool swordKb, Vector3 hitDir)
    {
        HP = HP - TakenDamage;
        StopAttack();
        KnockBack(swordKb, hitDir);
        if(HP <= 0)
        {
            Stagger();
        }
    }

    void KnockBack(bool swordKb, Vector3 kbDirection)
    {
        if(swordKb)
        {
            StartCoroutine(SwordKnockBack());
        } else {
            StartCoroutine(FistKnockBack(kbDirection));
        }
    }

    IEnumerator SwordKnockBack()
    {
        yield return null;

        DisableAgent();
        Vector3 dodgeStep = (player.position - transform.position).normalized * StatPreset.dodgeDistance * -1;
        rb.velocity = Vector3.zero;
        rb.AddForce(new Vector3(dodgeStep.x, 0, dodgeStep.z));
        isStunned = true;
        animator.Play("Stun");
        states.Play("Stunned");
        animator.SetBool("IsStunned", true);
        states.SetBool("IsStunned",true);

        yield return new WaitForFixedUpdate();
        yield return new WaitUntil(() => rb.velocity.magnitude < 0.01f);
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("IsStunned", false);
        states.SetBool("IsStunned",false);
        isStunned = false;
        EnableAgent();
    }

    IEnumerator FistKnockBack(Vector3 kbDirection)
    {
        yield return null;

        DisableAgent();
        rb.velocity = Vector3.zero;
        rb.AddForce(kbDirection);
        animator.Play("FistStun");
        states.Play("Stunned");
        animator.SetBool("IsStunned", true);
        states.SetBool("IsStunned",true);
        isStunned = true;

        yield return new WaitForSeconds(0.1f);
        yield return new WaitForFixedUpdate();
        yield return new WaitUntil(() => rb.velocity.magnitude < 0.01f);
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("IsStunned", false);
        states.SetBool("IsStunned",false);
        isStunned = false;
        EnableAgent();
    }

    IEnumerator AgentGroundCheck()
    {
        if(isGrounded)
        {
            agent.enabled = true;
            rb.isKinematic = true;
            obstacle.enabled = false;
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
        obstacle.enabled = true;
    }
}
