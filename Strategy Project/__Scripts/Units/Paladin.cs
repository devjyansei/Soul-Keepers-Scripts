using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Paladin : Unit
{
    [SerializeField] private PaladinSO paladinSO;
    HealthBar healthBar;
    [SerializeField] GameObject healthbarObject;

    protected const string ATTACK_0 = "attack 0";
    protected const string ATTACK_1 = "attack 1";

    protected const string ATTACK_TYPE = "attackType";
    private int attackType;




    Coroutine normalPathCoroutine;
    Coroutine enemyDetectCoroutine;
    Coroutine enemyFollowCoroutine;






    public PaladinStates currentState;

    public enum PaladinStates
    {
        idle,
        move,
        followEnemy,
        attack
    }
    public void SwitchPaladinState(PaladinStates state)
    {
        currentState = state;

        switch (currentState)
        {
            case PaladinStates.idle:
                agent.isStopped = true;
                agent.radius = 0.3f;
                currentEnemy = null;
                enemiesInRadiusList.Clear();
                

                PlayerInputManager.Instance.DestroyMoveMarker();

                //detect menzili kontrol et
                if (normalPathCoroutine != null)
                {
                    StopCoroutine(normalPathCoroutine);
                }
                if (enemyFollowCoroutine != null)
                {
                    StopCoroutine(enemyFollowCoroutine);
                }
                if (enemyDetectCoroutine != null)
                {
                    StopCoroutine(enemyDetectCoroutine);
                }
                
                enemyDetectCoroutine = StartCoroutine(EnemyDetect());

                isWalking = false;
                animator.SetBool(IS_WALKING, isWalking);

                isAttacking = false;
                animator.SetBool(IS_ATTACKING, isAttacking);
                break;

            case PaladinStates.move:
                agent.isStopped = false;
                currentEnemy = null;
                enemiesInRadiusList.Clear();
                //agent.radius = 0.01f;


                if (normalPathCoroutine != null)
                {
                    StopCoroutine(normalPathCoroutine);
                }
                if (enemyFollowCoroutine != null)
                {
                    StopCoroutine(enemyFollowCoroutine);
                }
                if (enemyDetectCoroutine != null)
                {
                    StopCoroutine(enemyDetectCoroutine);
                }
                normalPathCoroutine = StartCoroutine(CheckRemainingPath());


                isWalking = true;
                animator.SetBool(IS_WALKING, isWalking);

                isAttacking = false;
                animator.SetBool(IS_ATTACKING, isAttacking);              
                break;

            case PaladinStates.followEnemy:
                agent.isStopped = false;
                enemiesInRadiusList.Clear();
                //agent.radius = 0.01f;



                if (normalPathCoroutine != null)
                {
                    StopCoroutine(normalPathCoroutine);
                }
                if (enemyFollowCoroutine != null)
                {
                    StopCoroutine(enemyFollowCoroutine);
                }
                if (enemyDetectCoroutine != null)
                {
                    StopCoroutine(enemyDetectCoroutine);
                }
                enemyFollowCoroutine = StartCoroutine(FollowEnemy());


                isWalking = true;
                animator.SetBool(IS_WALKING, isWalking);

                isAttacking = false;
                animator.SetBool(IS_ATTACKING, isAttacking);
                break;


            case PaladinStates.attack:
                agent.isStopped = true;

                


                if (normalPathCoroutine != null)
                {
                    StopCoroutine(normalPathCoroutine);
                }
                if (enemyFollowCoroutine != null)
                {
                    StopCoroutine(enemyFollowCoroutine);
                }
                if (enemyDetectCoroutine != null)
                {
                    StopCoroutine(enemyDetectCoroutine);
                }
                ChangeAttackAnimationRandom();
                
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
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        healthBar = GetComponentInChildren<HealthBar>();
    }
    private void Start()
    {
        base.AddToUnitList(this.gameObject);
        SetPaladinStats();
        SetPaladinButtonAttributes();
        SwitchPaladinState(PaladinStates.idle);
        StartCoroutine(HealthRegeneration());

        
        
    }



    
    private void Update()
    {
        
        
        //rotation
        if (currentEnemy != null && currentState == PaladinStates.attack)
        {
            transform.LookAt(currentEnemy.transform.position);          
        }

        //movemarker
        if (currentEnemy != null && currentState == PaladinStates.followEnemy)
        {
            if (PlayerInputManager.Instance.markersOwner == this)
            {
                PlayerInputManager.Instance.DestroyMoveMarker();
            }
        }
        
        //sað click
        if (Input.GetMouseButtonDown(1) && isSelected )
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //attack move
            if (Physics.Raycast(ray, out hit, rayDistance, enemy))
            {
                

                // logic
                SetCurrentEnemy(hit.transform.GetComponent<Enemy>());
                SetTargetPosition(currentEnemy.transform.position);

                SwitchPaladinState(PaladinStates.followEnemy);

                PlayerInputManager.Instance.DestroyMoveMarker();
                return; // sorun olursa kaldýr marker çýkmasýn dýye koydum
            }

            //normal move
            else if(Physics.Raycast(ray, out hit, rayDistance, ground))
            {
                SetTargetPosition(hit.point);

                SwitchPaladinState(PaladinStates.move);
              //  UnitSelections.Instance.InstantiateMoveMarker(hit.point);

            }         
        }
    }

    private IEnumerator EnemyDetect()
    {
        while (enemiesInRadiusList.Count == 0)
        {
            SetEnemiesInRadius();

            if (enemiesInRadiusList.Count > 0)
            {
                currentEnemy = GetLowestHealthEnemy();
                SwitchPaladinState(PaladinStates.followEnemy);
            }

            yield return new WaitForSeconds(.1f);
        }
    }
    
   

    private IEnumerator FollowEnemy()
    {
        while (true)
        {
            //enemy yaþýyorsa
            if (currentEnemy != null)
            {
                agent.SetDestination(currentEnemy.transform.position);

                // attack range e girince saldýr
                if (Vector3.Distance(currentEnemy.transform.position,this.transform.position) <= attackRange)
                {
                   
                    SwitchPaladinState(PaladinStates.attack);
                }         

            }

            //enemy ölmüþse koþturmayý býrak
            else
            {
                SwitchPaladinState(PaladinStates.idle);
                break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

   
    
    
    protected IEnumerator CheckRemainingPath()
    {

        agent.SetDestination(targetPosition);

        yield return new WaitForSeconds(0.1f); // frame hatasý olmamasý için

        while (true)
        {
            if (IsPathEnded())
            {
                //yürüme bittiðinde idle geçmek için
                SwitchPaladinState(PaladinStates.idle);
                break;
            }
            yield return new WaitForSeconds(remainingPathCheckInterval);

        }
    }




    








    
    // ---------------------- CALLED FROM ANIMATION --------------------------//
    public void DealDamageToCurrentEnemy()
    {
        if (currentEnemy != null)
        {
            //
            PlayDealDamageSound();
            currentEnemy.TakeDamage(damage);

        }
        else
        {
            currentEnemy = null;
            SwitchPaladinState(PaladinStates.idle);
        }
    }
    
    public void CheckIsEnemyDead()
    {
        if (currentEnemy == null)
        {
            currentEnemy = null; // for avoiding missing error
            SwitchPaladinState(PaladinStates.idle);

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

    protected Enemy GetLowestHealthEnemy()
    {
        // if any enemy in detectList
        Enemy lowestHealthEnemy = null;
        if (enemiesInRadiusList.Count > 0)
        {
            for (int i = 0; i < enemiesInRadiusList.Count; i++)
            {
                lowestHealthEnemy = enemiesInRadiusList[0];

                if (enemiesInRadiusList[i].GetCurrentHealth() < lowestHealthEnemy.GetCurrentHealth())
                {
                    enemiesInRadiusList[i] = lowestHealthEnemy;
                }

            }
            return lowestHealthEnemy;
        }

        //if there is no unit in detectList
        else
        {
            return null;
        }


    }
    // called from attack animations end
    public void ActAccordingToDistanceOfTarget()
    {
        if (IsCurrentTargetInDetectionRange() == false)
        {
            currentEnemy = null;
            SetEnemiesInRadius();
            SwitchPaladinState(PaladinStates.idle);
        }
        else if (IsCurrentTargetInDetectionRange() && IsCurrentTargetInAttackRange() == false)
        {
            SetEnemiesInRadius();
            SwitchPaladinState(PaladinStates.idle);
        }

    }
    



    //------- called in every single attack animations end -------//
    public void ChangeAttackAnimationRandom()
    {
        attackType = Random.Range(0, 2);
        animator.SetInteger(ATTACK_TYPE, attackType);
    }


    // -----------------  FUNCTIONAL ---------------------------------//


    private void SetPaladinButtonAttributes()
    {
        for (int i = 0; i < paladinSO.attributesClickEventList.Count; i++)
        {
            paladinSO.attributesClickEventList[i] = ClickEventsManager.Instance.paladinAttributesList[i];
        }
    }
    private void SetPaladinStats()
    {
        SetSpeed(unitDataSO.speed);
        SetMaxHealth(unitDataSO.maxHealth);
        SetDamage(unitDataSO.damage);
        SetAttackSpeed();
        SetArmor(unitDataSO.armor);
        SetAttackRange(unitDataSO.attackRange);
        SetDetectionRange(unitDataSO.detectionRange);
        SetHealthRegenPerFiveSecond(unitDataSO.healthRegenPerFiveSecond);

        SetCurrentHealth(GetMaxHealth());

    }
    public void SetAttackSpeed()
    {
        animator.SetFloat("attackSpeed", paladinSO.attackSpeed);
    }

    
    public override void TakeDamage(int amount)
    {
        if (isSpecialAttackActive)
        {
            specialAttackCounter++;
        }

        if (specialAttackCounter >= specialAttackDelay)
        {
            amount = 0;
            SpecialAttack();
        }

        int currentHealth = GetCurrentHealth();
        int reducedDamage = amount - GetArmor();
        if (reducedDamage < 0)
        {
            reducedDamage = 0;
        }

        SetCurrentHealth(currentHealth -= reducedDamage);
        healthBar.UpdateHealthbar(maxHealth, currentHealth);

        if (UnitSelections.Instance.selectedUnit != null)
        {
            UiManager.Instance.UpdateDisplayedAttributes(UnitSelections.Instance.selectedUnit.GetComponent<Unit>());
        }

        if (GetCurrentHealth() <= 0)
        {
            Die();
        }
    }
    protected override void Die()
    {
        
        isAlive = false;
        animator.SetTrigger("death");
        GetComponent<CapsuleCollider>().enabled = false;
        agent.isStopped = true;
        PlayDeathSound();

        if (UnitSelections.Instance.selectedUnit == this.gameObject)
        {
            UnitSelections.Instance.lastSelectedUnit = null;
            UnitSelections.Instance.selectedUnit = null;
            UiManager.Instance.CloseControlPanel();
            SetIsSelected(false);
            DeactivateSelectedBorders(this);

        }
        healthbarObject.transform.localScale = Vector3.zero;
        this.enabled = false;
        Destroy(gameObject, 6f);
    }

    // called from attack animations end
    public void ActAccordingAlive()
    {
        if (currentEnemy != null)
        {
            if (currentEnemy.GetCurrentHealth() <= 0)
            {
                currentEnemy = null;
                SetEnemiesInRadius();
                SwitchPaladinState(PaladinStates.idle);
            }
        }
        
    }
    protected IEnumerator HealthRegeneration()
    {
        while (true)
        {
            if (GetCurrentHealth() < GetMaxHealth())
            {

                SetCurrentHealth(GetCurrentHealth() + GetHealthRegenPerFiveSecond());
                healParticle.Play();

                if (GetCurrentHealth() > GetMaxHealth())
                {

                    SetCurrentHealth(GetMaxHealth());
                }
            }
            yield return new WaitForSeconds(5f);
            if (UnitSelections.Instance.selectedUnit == this.gameObject)
            {
                UiManager.Instance.UpdateDisplayedAttributes(UnitSelections.Instance.selectedUnit.GetComponent<Unit>());
            }
            
            healthBar.UpdateHealthbar(GetMaxHealth(),GetCurrentHealth());
        }
    }

    public void IncreaseAttackSpeed(float newAttackSpeed)
    {
        animator.SetFloat("attackSpeed", newAttackSpeed);
    }


    public void SpecialAttack()
    {

        if (specialAttackCounter >= specialAttackDelay)
        {
            specialAttackSO.protectionShieldSO.UseSpecialAttack(this.transform.position + Vector3.up*2f);
            specialAttackCounter = 0;

        }
    }

    public override void LevelUp()
    {
        SetCurrentLevel(GetCurrentLevel() + 1);
        EnlargeScale();
        SetLevelParticle();
        PlayLevelUpSound();
        healthBar.UpdateHealthbar(GetMaxHealth(),GetCurrentHealth());
    }

    public void PlayAcademyUpgradeParticle()
    {
        academyUpgradeParticle.Play();
    }
    public void PlayDeathSound()
    {
        int randomAudio = Random.Range(0, dieAudios.Count);
        audioSource.clip = dieAudios[randomAudio];
        audioSource.Play();
    }
    public void PlayDealDamageSound()
    {
        int playChance = Random.Range(0, 100);
        if ( playChance <= 40)
        {
            int randomAudio = Random.Range(0, damageAudios.Count);
            audioSource.clip = damageAudios[randomAudio];
            audioSource.Play();
        }
        
    }
    
    public void PlayLevelUpSound()
    {
        audioSource.clip = levelUpSound;
        audioSource.Play();
    }
}
