using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mutant : Neutral
{
    [SerializeField] MutantSO mutantDataSO;
    [SerializeField] private Unit currentEnemy;

    private HealthBar healthBar;
    [SerializeField] ParticleSystem rageParticle;
    private bool isRaged;

    [SerializeField] protected bool isRageing = false;

    private const string IS_RAGEING = "isRageing";


    Coroutine unitDetectCoroutine;
    Coroutine unitFollowCoroutine;

    public bool isInvulnerable = false;
    [SerializeField] private AudioClip rageAudio;
    
    public MutantStates currentState;
    public enum MutantStates
    {
        idle,
        followUnit,
        rageing,
        attack

    }
    public void SwitchMutantState(MutantStates state)
    {

        currentState = state;

        switch (currentState)
        {
            case MutantStates.idle:
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

           
            case MutantStates.followUnit:
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

            case MutantStates.rageing:
                if (isAlive)
                {
                    if (unitFollowCoroutine != null)
                    {
                        StopCoroutine(unitFollowCoroutine);
                    }
                    if (unitDetectCoroutine != null)
                    {
                        StopCoroutine(unitDetectCoroutine);
                    }

                    isRageing = true;
                    animator.SetBool(IS_RAGEING, isRageing);

                    EnterRageMode();
                }


                break;

            case MutantStates.attack:
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
                currentEnemy = GetClosestUnit();
                SwitchMutantState(MutantStates.followUnit);
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

                    SwitchMutantState(MutantStates.attack);
                }

                
            }
            
            //unit ölmüþse koþturmayý býrak
            else
            {
                SwitchMutantState(MutantStates.idle);
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
            SwitchMutantState(MutantStates.idle);
        }
        else if (Vector3.Distance(currentEnemy.transform.position, this.transform.position) > detectionRange)
        {
            SwitchMutantState(MutantStates.idle);
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
        SwitchMutantState(MutantStates.idle);
        rageParticle.gameObject.SetActive(false);
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
                SwitchMutantState(MutantStates.idle);
            }
        }
        else
        {
            SwitchMutantState(MutantStates.idle);
        }
    }

    // called from attack animations end


    public void ActAccordingToDistanceOfTarget()
    {
        
        if (IsCurrentTargetInDetectionRange() && IsCurrentTargetInAttackRange()==false)
        {
            SetUnitsInRadius();
        }
        else if (IsCurrentTargetInDetectionRange() == false)
        {
            currentEnemy = null;
            SetUnitsInRadius();
            SwitchMutantState(MutantStates.idle);
        }
        else if (IsCurrentTargetInDetectionRange() && IsCurrentTargetInAttackRange() == false)
        {            
            SetUnitsInRadius();
            SwitchMutantState(MutantStates.idle);
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
            SwitchMutantState(MutantStates.followUnit);
        }
        else if (IsCurrentTargetInAttackRange() == false && IsCurrentTargetInDetectionRange() == false)
        {
            SwitchMutantState(MutantStates.idle);
        }
        
    }

    // called from run animations mid
    public void CheckDistanceBetweenTarget()
    {
        if (IsCurrentTargetInDetectionRange() == false)
        {
            currentEnemy = null;
            SwitchMutantState(MutantStates.idle);

        }
    }



    // -------------- RAGE MODE ---------------- //
    public void EnterRageMode()
    {
        agent.isStopped = true;
        rageParticle.gameObject.SetActive(true);
        audioSource.clip = rageAudio;
        audioSource.Play();

        isInvulnerable = true;
        isRaged = true;

        IncreaseDamage();
        IncreaseAttackSpeed();
    }
    public void ExitRageAnimation()
    {
        agent.isStopped = false;       

        isInvulnerable = false;

        isRageing = false;
        animator.SetBool(IS_RAGEING, isRageing);

        SwitchMutantState(MutantStates.idle);
    }
    
    public void IncreaseDamage()
    {
        SetDamage(GetDamage() + mutantDataSO.extraDamageInRage);
        rageParticle.Play();
    }
    public void IncreaseAttackSpeed()
    {
        animator.SetFloat("attackSpeed", 1.35f);
    }
    public override void TakeDamage(int amount)
    {
        if (isInvulnerable == false)
        {
            currentHealth -= amount;
            healthBar.UpdateHealthbar(maxHealth, currentHealth);
            PlayRandomHitSound();
            hitParticle.Play();
            if (currentHealth <= 0)
            {
                PlayDieSound();
                Die();
            }
            if (currentHealth <= GetMaxHealth() * (mutantDataSO.ragePercentage/100f) && isRaged == false)
            {
                SwitchMutantState(MutantStates.rageing);

                int newHp = Mathf.RoundToInt(GetMaxHealth() * mutantDataSO.ragePercentage / 100f);
                SetCurrentHealth(newHp);
                
            }
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
            SwitchMutantState(MutantStates.idle);
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
        audioSource.volume = .5f;
        audioSource.PlayOneShot(dieSounds[randomAudio]);
    }
}
