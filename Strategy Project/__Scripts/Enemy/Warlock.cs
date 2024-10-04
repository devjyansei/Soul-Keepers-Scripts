using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Warlock : Neutral
{
    [SerializeField] WarlockSO warlockDataSO;
    [SerializeField] private SpellSO spellSO;
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] Transform missileCastPoint;
    Vector3 spellSpawnPos;

    [SerializeField] public Unit currentEnemy;
    private HealthBar healthBar;


    Coroutine unitDetectCoroutine;
    Coroutine unitFollowCoroutine;

    [SerializeField] private float missileSpeed;


    protected bool isCasting = false;
    protected const string IS_CASTING = "isCasting";
    ParticleSystem tempSpell;
    private int spellCounter;

    [SerializeField] private List<AudioClip> spellSounds = new List<AudioClip>();

    //GETTER - SETTER
    public float GetMissileSpeed() { return missileSpeed; }
    public void SetMissileSpeed(float value) => missileSpeed = value;




    public WarlockStates currentState;
    public enum WarlockStates
    {
        idle,
        followUnit,
        casting,
        attack

    }
    public void SwitchWarlockState(WarlockStates state)
    {

        currentState = state;

        switch (currentState)
        {
            case WarlockStates.idle:
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

                    isCasting = false;
                    animator.SetBool(IS_CASTING, isCasting);

                    isWalking = false;
                    animator.SetBool(IS_WALKING, isWalking);

                    isAttacking = false;
                    animator.SetBool(IS_ATTACKING, isAttacking);
                }
                
                break;


            case WarlockStates.followUnit:
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

                    isCasting = false;
                    animator.SetBool(IS_CASTING, isCasting);

                    isWalking = true;
                    animator.SetBool(IS_WALKING, isWalking);

                    isAttacking = false;
                    animator.SetBool(IS_ATTACKING, isAttacking);
                }


                break;

            case WarlockStates.casting:
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



                    isCasting = true;
                    animator.SetBool(IS_CASTING, isCasting);

                    isWalking = false;
                    animator.SetBool(IS_WALKING, isWalking);

                    isAttacking = false;
                    animator.SetBool(IS_ATTACKING, isAttacking);
                }

                
                break;

            case WarlockStates.attack:
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

                    isCasting = false;
                    animator.SetBool(IS_CASTING, isCasting);

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
  
    public void SetSpellSpawnPos()
    {
        
        spellSpawnPos = currentEnemy.transform.position + Vector3.up;
    }
    public void SetSpellPositionOriginSelf()
    {
        spellSpawnPos = transform.position;
    }
    
    public void CastSpell(SpellSO spellSO)
    {


        if (currentEnemy != null)
        {
            //Vector3 direction = (currentEnemy.transform.position - this.transform.position).normalized;


            tempSpell = Instantiate(spellSO.spellVFX, spellSpawnPos, spellSO.spellVFX.transform.rotation);
            PlaySpellSound();
            StartCoroutine(SpellCastCheck());
        }

    }

    public void DealSpellDamageInRadiusDamage(SpellSO spellSO)
    {
        Collider[] unitColliders = Physics.OverlapSphere(tempSpell.transform.position, spellSO.spellRadius, unit);
        foreach (var unitCollider in unitColliders)
        {
            unitCollider.GetComponent<Unit>().TakeDamage(spellSO.spellDamage);
        }
    }
    IEnumerator SpellCastCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(spellSO.animationDuration);
            SwitchWarlockState(WarlockStates.idle);
            Destroy(tempSpell);
            tempSpell = null;
            break;
        }
    }


    // called from attack anim
    public void SendMissile()
    {
        if (currentEnemy != null)
        {
            Vector3 direction = (currentEnemy.transform.position - this.transform.position).normalized;
            missilePrefab.transform.forward = direction;

            GameObject missile = Instantiate(missilePrefab, missileCastPoint.position, missilePrefab.transform.rotation);
            missile.GetComponent<WarlockMissile>().owner = this;

            PlayDamageSound();

            CountSpell();
        }
       
    }
    
    private void CountSpell()
    {
        spellCounter++;

        if (spellCounter >= warlockDataSO.spellDelay)
        {
            spellCounter = 0;
            SwitchWarlockState(WarlockStates.casting);
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
                SwitchWarlockState(WarlockStates.followUnit);
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

                    SwitchWarlockState(WarlockStates.attack);
                }


            }

            //unit ölmüþse koþturmayý býrak
            else
            {
                SwitchWarlockState(WarlockStates.idle);
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
            SwitchWarlockState(WarlockStates.idle);
        }
        else if (Vector3.Distance(currentEnemy.transform.position, this.transform.position) > detectionRange)
        {
            SwitchWarlockState(WarlockStates.idle);
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
        
        SetMissileSpeed(warlockDataSO.missileSpeed);

        SetStats();
        SwitchWarlockState(WarlockStates.idle);

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
                SwitchWarlockState(WarlockStates.idle);
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
            SwitchWarlockState(WarlockStates.followUnit);
        }
        else if (IsCurrentTargetInAttackRange() == false && IsCurrentTargetInDetectionRange() == false)
        {
            SwitchWarlockState(WarlockStates.idle);
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
            SwitchWarlockState(WarlockStates.idle);
        }
        else if (IsCurrentTargetInDetectionRange() && IsCurrentTargetInAttackRange() == false)
        {
            SetUnitsInRadius();
            SwitchWarlockState(WarlockStates.idle);
        }

    }

    // called from run animations mid
    public void CheckDistanceBetweenTarget()
    {
        if (IsCurrentTargetInDetectionRange() == false)
        {
            currentEnemy = null;
            SwitchWarlockState(WarlockStates.idle);

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
    public void DealDamageToCurrentTarget()
    {
        if (currentEnemy != null)
        {
            Debug.Log("hit");
            
            currentEnemy.TakeDamage(damage);
        }
        else
        {
            currentEnemy = null;
            SwitchWarlockState(WarlockStates.idle);
        }
    }

    protected bool IsCurrentTargetInAttackRange()
    {
        if (currentEnemy != null)
        {
            if (Vector3.Distance(currentEnemy.transform.position, this.transform.position) +1 < attackRange)
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
        audioSource.volume = .18f;
        audioSource.clip = hitSounds[randomAudio];
        audioSource.Play();
    }
    public void PlayDamageSound()
    {
        int randomAudio = Random.Range(0, damageSounds.Count);
        audioSource.volume = .18f;
        audioSource.PlayOneShot(damageSounds[randomAudio]);
    }
    public void PlaySpellSound()
    {
        int randomAudio = Random.Range(0, spellSounds.Count);
        audioSource.volume = .5f;
        audioSource.PlayOneShot(spellSounds[randomAudio]);
    }

    public void PlayDieSound()
    {
        int randomAudio = Random.Range(0, dieSounds.Count);
        audioSource.volume = .5f;
        audioSource.PlayOneShot(dieSounds[randomAudio]);
    }
}