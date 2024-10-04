using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AxedOrc : Invader
{
    [SerializeField] AxedOrcSO axedOrcSO;
    HealthBar healthBar;

    
    private float attackRangeTolerance = 0.4f;

    public AxedOrcStates currentState;
    public enum AxedOrcStates
    {
        idle,
        move,
        attack
    }

    public void SwitchAxedOrcState(AxedOrcStates state)
    {

        currentState = state;

        switch (currentState)
        {
            case AxedOrcStates.idle:
                agent.isStopped = true;
                unitsInRadiusList.Clear();



                isWalking = false;
                animator.SetBool(IS_WALKING, isWalking);

                isAttacking = false;
                animator.SetBool(IS_ATTACKING, isAttacking);
                break;

            case AxedOrcStates.move:
                agent.isStopped = false;


                isWalking = true;
                animator.SetBool(IS_WALKING, isWalking);

                isAttacking = false;
                animator.SetBool(IS_ATTACKING, isAttacking);


                break;


            case AxedOrcStates.attack:
                agent.isStopped = true;

                isWalking = false;
                animator.SetBool(IS_WALKING, isWalking);

                isAttacking = true;
                animator.SetBool(IS_ATTACKING, isAttacking);
                break;




            default:
                break;

        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        core = FindObjectOfType<Core>();
        healthBar = GetComponentInChildren<HealthBar>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        //agent.stoppingDistance = enemyDataSO.attackRange - attackRangeTolerance;

        SetStats();
        SwitchAxedOrcState(AxedOrcStates.idle);

        GoToCore();
    }
    
    protected bool IsPathEnded()
    {

        float distance = agent.remainingDistance;
        if (distance <= 0.1f && agent.pathStatus == NavMeshPathStatus.PathComplete) //when Arrived
        {
            return true;
        }


        return false;
    }
    private void GoToCore()
    {
        StartCoroutine(CheckRemainingDistanceToCore());

    }
    IEnumerator CheckRemainingDistanceToCore()
    {
        Vector3 direction = (this.transform.position - core.transform.position).normalized;
        Vector3 targetPos = core.transform.position + direction * (agent.stoppingDistance + core.transform.localScale.x / 2);
        Vector3 randomPosAroundTargetPos = new Vector3(targetPos.x, 0, Random.Range(targetPos.z - 2.5f, targetPos.z + 2.5f));

        agent.SetDestination(randomPosAroundTargetPos);

        SwitchAxedOrcState(AxedOrcStates.move);

        while (true)
        {
            //Debug.Log(Vector3.Distance(targetPos, transform.position));

            if (Vector3.Distance(targetPos, transform.position) <= agent.stoppingDistance + attackRange + attackRangeTolerance)
            {
                SwitchAxedOrcState(AxedOrcStates.attack);
                break;
            }


            yield return new WaitForSeconds(.5f);
        }
    }


    public override void TakeDamage(int amount)
    {
        currentHealth -= amount;
        healthBar.UpdateHealthbar(maxHealth, currentHealth);
        hitParticle.Play();
        PlayRandomHitSound();

        if (currentHealth <= 0)
        {
            Die();
        }

    }
    public void PlayRandomHitSound()
    {
        int randomAudio = Random.Range(0, hitSounds.Count);
        audioSource.clip = hitSounds[randomAudio];
        audioSource.Play();
    }
}
