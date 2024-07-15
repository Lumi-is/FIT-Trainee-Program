using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStatPreset StatPreset;

    [SerializeField] Transform Player;

    [SerializeField] private Vector3 SpawnPosition;

    [SerializeField] private float EnemyAttackThreshold;

    [SerializeField] private Rigidbody rb;
    public float HP;

    public bool canSlice;
    public bool isStunned;

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
        } else {
            Attack();
        }

        if(HP < 33)
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

    }
    void SecondaryAttack()
    {

    }

    public void TakeDamage(float TakenDamage)
    {
        Mathf.Clamp(TakenDamage,0,StatPreset.MaxDamageTaken);
        HP = HP - TakenDamage;
        StepBack();
        StartCoroutine(Stun(1));
    }

    IEnumerator Stun(float stunDuration)
    {
        isStunned = true;
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
    }
}
