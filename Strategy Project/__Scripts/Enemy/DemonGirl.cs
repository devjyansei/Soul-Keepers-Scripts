using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonGirl : Neutral
{
    [SerializeField] DemonGirlSO demonGirlSO;
    [SerializeField] private Unit currentEnemy;

    private HealthBar healthBar;


    Coroutine unitDetectCoroutine;
    Coroutine unitFollowCoroutine;



    public DemonGirlStates currentState;
    public enum DemonGirlStates
    {
        idle,
        followUnit,
        rageing,
        attack

    }
    public void SwitchDemonGirlState(DemonGirlStates state)
    {

        currentState = state;

        switch (currentState)
        {
            case DemonGirlStates.idle:
                if (isAlive)
                {
                    agent.isStopped = true;
                    currentEnemy = null;
                    unitsInRadiusList.Clear();

                    if (unitFollowCoroutine != null)
                    {
                        StopCoroutine(unitFollowCoroutine);
                    }
                    if (unitDetectCoroutine != null)
                    {
                        StopCoroutine(unitDetectCoroutine);
                    }
                    unitDetectCoroutine = StartCoroutine(UnitDetect());


                    isWalking = false;
                    animator.SetBool(IS_WALKING, isWalking);

                    isAttacking = false;
                    animator.SetBool(IS_ATTACKING, isAttacking);
                }
                
                break;


            case DemonGirlStates.followUnit:
                if (isAlive)
                {
                    agent.isStopped = false;
                    unitsInRadiusList.Clear();

                    if (unitFollowCoroutine != null)
                    {
                        StopCoroutine(unitFollowCoroutine);
                    }
                    if (unitDetectCoroutine != null)
                    {
                        StopCoroutine(unitDetectCoroutine);
                    }


                    unitFollowCoroutine = StartCoroutine(FollowUnit());

                    isWalking = true;
                    animator.SetBool(IS_WALKING, isWalking);

                    isAttacking = false;
                    animator.SetBool(IS_ATTACKING, isAttacking);
                }
                


                break;

            case DemonGirlStates.attack:
                if (isAlive)
                {
                    agent.isStopped = true;

                    if (unitFollowCoroutine != null)
                    {
                        StopCoroutine(unitFollowCoroutine);
                    }
                    if (unitDetectCoroutine != null)
                    {
                        StopCoroutine(unitDetectCoroutine);
                    }

                    isWalking = false;
                    animator.SetBool(IS_WALKING, isWalking);

                    isAttacking = true;
                    animator.SetBool(IS_ATTACKING, isAttacking);
                }
                
                break;




            default:
                break;

        }
    }


    private IEnumerator UnitDetect()
    {

        while (unitsInRadiusList.Count == 0)
        {



            //yeni ekledim
            SetUnitsInRadius();

            if (unitsInRadiusList.Count > 0)
            {
                currentEnemy = GetLowestHealthUnit();
                SwitchDemonGirlState(DemonGirlStates.followUnit);
            }

            yield return new WaitForSeconds(.1f);
        }
    }


    private IEnumerator FollowUnit()
    {
        while (true)
        {

            //unit yaþýyorsa
            if (currentEnemy != null)
            {
                agent.SetDestination(currentEnemy.transform.position);

                // attack range e girince saldýr
                if (Vector3.Distance(currentEnemy.transform.position, this.transform.position) <= attackRange)
                {

                    SwitchDemonGirlState(DemonGirlStates.attack);
                }


            }

            //unit ölmüþse koþturmayý býrak
            else
            {
                SwitchDemonGirlState(DemonGirlStates.idle);
                break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }


    public void CheckCurrentEnemyState()
    {
        if (currentEnemy == null)
        {
            currentEnemy = null; // for avoiding missing error
            SwitchDemonGirlState(DemonGirlStates.idle);
        }
        else if (Vector3.Distance(currentEnemy.transform.position, this.transform.position) > detectionRange)
        {
            SwitchDemonGirlState(DemonGirlStates.idle);
        }

    }








    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        healthBar = GetComponentInChildren<HealthBar>();
        audioSource = GetComponent<AudioSource>();

    }
    private void Start()
    {
        SetStats();
        SwitchDemonGirlState(DemonGirlStates.idle);

    }

    private void Update()
    {
        RotateTowardsTarget();



    }



    protected void RotateTowardsTarget()
    {
        if (currentEnemy != null)
        {
            Vector3 lookPos = new Vector3(currentEnemy.transform.position.x, 0, currentEnemy.transform.position.z);
            transform.LookAt(lookPos);
        }

    }

    // called from attack animations end
    public void ActAccordingAlive()
    {
        if (currentEnemy != null)
        {
            if (currentEnemy.GetCurrentHealth() <= 0)
            {
                currentEnemy = null;
                SetUnitsInRadius();
                SwitchDemonGirlState(DemonGirlStates.idle);
            }
        }
        
    }
    //end of attack animation
    public void CanAttackAgain()
    {
        if (IsCurrentTargetInAttackRange())
        {
            return;
        }
        else if (IsCurrentTargetInAttackRange() == false && IsCurrentTargetInDetectionRange() == true)
        {
            SwitchDemonGirlState(DemonGirlStates.followUnit);
        }
        else if (IsCurrentTargetInAttackRange() == false && IsCurrentTargetInDetectionRange() == false)
        {
            SwitchDemonGirlState(DemonGirlStates.idle);
        }

    }
    // called from attack animations end


    public void ActAccordingToDistanceOfTarget()
    {

        if (IsCurrentTargetInDetectionRange())
        {
            SetUnitsInRadius();
            return;
        }
        else if (IsCurrentTargetInDetectionRange() == false)
        {
            currentEnemy = null;
            SetUnitsInRadius();
            SwitchDemonGirlState(DemonGirlStates.idle);
        }
        else if (IsCurrentTargetInDetectionRange() && IsCurrentTargetInAttackRange() == false)
        {
            SetUnitsInRadius();
            SwitchDemonGirlState(DemonGirlStates.idle);
        }

    }

    // called from run animations mid
    public void CheckDistanceBetweenTarget()
    {
        if (IsCurrentTargetInDetectionRange() == false)
        {
            currentEnemy = null;
            SwitchDemonGirlState(DemonGirlStates.idle);

        }
    }


    private void OnDestroy()
    {
        //UpgradeManager.Instance.allNeutralsList.Remove(this);
    }

    public override void TakeDamage(int amount)
    {
        currentHealth -= amount;
        healthBar.UpdateHealthbar(maxHealth, currentHealth);
        hitParticle.Play();
        PlayRandomHitSound();
        if (currentHealth <= 0)
        {
            PlayDieSound();
            Die();
        }
       
    }
    public void DealDamageToCurrentTarget()
    {
        if (currentEnemy != null)
        {
            
            PlayDamageSound();
            currentEnemy.TakeDamage(damage);
        }
        else
        {
            currentEnemy = null;
            SwitchDemonGirlState(DemonGirlStates.idle);
        }
    }

    protected bool IsCurrentTargetInAttackRange()
    {
        if (currentEnemy != null)
        {
            if (Vector3.Distance(currentEnemy.transform.position, this.transform.position) < attackRange)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;


    }
    protected bool IsCurrentTargetInDetectionRange()
    {
        if (currentEnemy != null)
        {
            if (Vector3.Distance(currentEnemy.transform.position, this.transform.position) < detectionRange)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;

    }


    public void PlayRandomHitSound()
    {
        int randomAudio = Random.Range(0, hitSounds.Count);
        audioSource.clip = hitSounds[randomAudio];
        audioSource.Play();
    }

    public void PlayDamageSound()
    {
        int randomAudio = Random.Range(0, damageSounds.Count);
        audioSource.volume = .18f;
        audioSource.PlayOneShot(damageSounds[randomAudio]);
    }
    public void PlayDieSound()
    {
        int randomAudio = Random.Range(0, dieSounds.Count);
        audioSource.volume = .18f;
        audioSource.PlayOneShot(dieSounds[randomAudio]);
    }
}
