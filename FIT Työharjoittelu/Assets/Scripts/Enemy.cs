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
        obstacle = transform.GetComponent<NavMeshObstacle>();
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
            anim.SetBool("IsPlayerInAttackRange", true);


        if(HP <= 0)
        {
            anim.SetBool("IsStaggered", true);
        }
        else
        {
            anim.SetBool("IsStaggered", false);
        }

        if(readyToAttack && !attacking && !isStunned && !isStaggered)
            anim.SetBool("canAttack", true);
        else
            anim.SetBool("canAttack", false);

    }

    void Idle()
    {

    }

    public void Stun()
    {

    }

    public void Stagger()
    {
        agent.speed = 0;
        canSlice = true;
        Invoke(nameof(Regen), 5f);
    }
    void Regen()
    {
        HP = StatPreset.HP /2;
        canSlice = false;
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
        StartCoroutine(Stun(StunDuration));
        if(canSlice) return;
        //Mathf.Clamp(TakenDamage,0,StatPreset.MaxDamageTaken);
        HP = HP - TakenDamage;
        KnockBack(swordKb, hitDir);
    }

    void KnockBack(bool swordKb, Vector3 kbDirection)
    {
        if(swordKb)
        {
            StartCoroutine(SwordKnockBack());
        } else {
            FistKnockBack(kbDirection);
        }
    }

    private IEnumerator SwordKnockBack()
    {
        yield return null;
        DisableAgent();
        Vector3 dodgeStep = (player.position - transform.position).normalized * StatPreset.dodgeDistance * -1;
        rb.velocity = Vector3.zero;
        rb.AddForce(new Vector3(dodgeStep.x, 0, dodgeStep.z));

        yield return new WaitForFixedUpdate();
        yield return new WaitUntil(() => rb.velocity.magnitude < 0.05f);
        yield return new WaitForSeconds(0.10f);
        EnableAgent();
    }

    private IEnumerator FistKnockBack(Vector3 kbDirection)
    {
        yield return null;
        DisableAgent();
        rb.velocity = Vector3.zero;
        rb.AddForce(kbDirection);

        yield return new WaitForFixedUpdate();
        yield return new WaitUntil(() => rb.velocity.magnitude < 0.05f);
        yield return new WaitForSeconds(0.10f);
        EnableAgent();
    }

    IEnumerator Stun(float stunDuration)
    {
        anim.SetBool("IsStunned",true);
        anim.Play("Stunned");
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
            obstacle.enabled = true;
            yield break;
        } else {
            yield return null;
            StartCoroutine(AgentGroundCheck());
        }     
    }

    void EnableAgent()
    {
        rb.velocity = Vector3.zero;
        StartCoroutine(AgentGroundCheck());
    }

    void DisableAgent()
    {
        agent.enabled = false;
        obstacle.enabled = true;
        rb.isKinematic = false;
    }
}
