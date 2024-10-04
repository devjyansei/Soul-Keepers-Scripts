using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
public class Archer : Unit
{
    [SerializeField] ArcherSO archerSO;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] Transform arrowStartPoint;
    HealthBar healthBar;
    [SerializeField] GameObject healthbarObject;


    

    Coroutine normalPathCoroutine;
    Coroutine enemyDetectCoroutine;
    Coroutine enemyFollowCoroutine;




    public ArcherStates currentState;
    public enum ArcherStates
    {
        idle,
        move,
        followEnemy,
        attack,
        solidStance
    }
    public void SwitchArcherState(ArcherStates state)
    {
        currentState = state;

        switch (currentState)
        {
            case ArcherStates.idle:
                agent.isStopped = true;
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

            case ArcherStates.move:
                agent.isStopped = false;
                currentEnemy = null;
                enemiesInRadiusList.Clear();

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


            case ArcherStates.followEnemy:
                agent.isStopped = false;
                enemiesInRadiusList.Clear();


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


            case ArcherStates.attack:
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

                isWalking = false;
                animator.SetBool(IS_WALKING, isWalking);

                isAttacking = true;
                animator.SetBool(IS_ATTACKING, isAttacking);
                break;

            case ArcherStates.solidStance:
                break;

            default:
                break;
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
        base.AddToUnitList(this.gameObject);
        SwitchArcherState(ArcherStates.idle);      
        SetArcherStats();
        SetArcherButtonAttributes();
        StartCoroutine(HealthRegeneration());
    }
    private void Update()
    {
        


        //rotation
        if (currentEnemy != null && currentState == ArcherStates.attack)
        {
            transform.LookAt(currentEnemy.transform.position);
        }

        //movemarker
        if (currentEnemy != null && currentState == ArcherStates.followEnemy  ) // düzelt marker gitmiyor
        {
            if (PlayerInputManager.Instance.markersOwner == this)
            {
                PlayerInputManager.Instance.DestroyMoveMarker();
            }
        }
        //sað click
        if (Input.GetMouseButtonDown(1) && isSelected)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //attack move
            if (Physics.Raycast(ray, out hit, rayDistance, enemy))
            {
                SetCurrentEnemy(hit.transform.GetComponent<Enemy>());
                SetTargetPosition(currentEnemy.transform.position);

                SwitchArcherState(ArcherStates.followEnemy);
            }

            //normal move
            else if (Physics.Raycast(ray, out hit, rayDistance, ground))
            {
                SetTargetPosition(hit.point);

                SwitchArcherState(ArcherStates.move);


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
                SwitchArcherState(ArcherStates.followEnemy);
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
                if (Vector3.Distance(currentEnemy.transform.position, this.transform.position) <= attackRange)
                {

                    SwitchArcherState(ArcherStates.attack);
                }

            }

            //enemy ölmüþse koþturmayý býrak
            else
            {
                SwitchArcherState(ArcherStates.idle);
                break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ActAccordingToDistanceOfTarget()
    {
        if (IsCurrentTargetInDetectionRange() == false)
        {
            
            currentEnemy = null;
            SetEnemiesInRadius();
            SwitchArcherState(ArcherStates.idle);
           
        }
        else if (IsCurrentTargetInDetectionRange() && IsCurrentTargetInAttackRange() == false)
        {
            SetEnemiesInRadius();
            SwitchArcherState(ArcherStates.idle);
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
    // ---------------------- CALLED FROM ANIMATION --------------------------//
    public void DealDamageToCurrentEnemy()
    {

        if (currentEnemy != null && IsCurrentTargetInAttackRange() && currentEnemy.isAlive)
        {
            if (isSpecialAttackActive)
            {
                specialAttackCounter++;
            }
            

            GameObject arrow = Instantiate(arrowPrefab);
            arrow.transform.position = arrowStartPoint.position;        
            arrow.transform.DOMove(currentEnemy.transform.position + Vector3.up*1.5f, .1f);            
            Destroy(arrow, .11f);

            PlayDealDamageSound();
            currentEnemy.TakeDamage(damage);
        }
        else
        {
            currentEnemy = null;
            SwitchArcherState(ArcherStates.idle);
        }
    }

    public void CheckIsEnemyDead()
    {
        if (currentEnemy == null)
        {
            currentEnemy = null; // for avoiding missing error
            SwitchArcherState(ArcherStates.idle);

        }

    }


    public override void TakeDamage(int amount)
    {
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
        //UiManager.Instance.UpdateDisplayedAttributes(this);
        if (GetCurrentHealth() <= 0)
        {
            Die();
            Debug.Log("died");
        }
    }

 

    private void SetArcherButtonAttributes()
    {
        for (int i = 0; i < archerSO.attributesClickEventList.Count; i++)
        {
            archerSO.attributesClickEventList[i] = ClickEventsManager.Instance.archerAttributesList[i];
        }
    }
    private void SetArcherStats()
    {
        SetSpeed(unitDataSO.speed);
        SetMaxHealth(unitDataSO.maxHealth);
        SetDamage(unitDataSO.damage);
        SetAttackSpeed();
        SetArmor(unitDataSO.armor);
        SetHealthRegenPerFiveSecond(unitDataSO.healthRegenPerFiveSecond);
        SetAttackRange(unitDataSO.attackRange);
        SetDetectionRange(unitDataSO.detectionRange);

        SetCurrentHealth(GetMaxHealth());

    }
    public void SetAttackSpeed()
    {
        animator.SetFloat("attackSpeed", archerSO.attackSpeed);
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
                SwitchArcherState(ArcherStates.idle);
                break;
            }
            yield return new WaitForSeconds(remainingPathCheckInterval);

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

    public void ActAccordingAlive()
    {
        if (currentEnemy != null)
        {
            if (currentEnemy.GetCurrentHealth() <= 0)
            {
                currentEnemy = null;
                SetEnemiesInRadius();
                SwitchArcherState(ArcherStates.idle);
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
                if (GetCurrentHealth() > GetMaxHealth())
                {
                    healParticle.Play();
                    SetCurrentHealth(GetMaxHealth());
                }
            }
            yield return new WaitForSeconds(5f);
            if (UnitSelections.Instance.selectedUnit == this.gameObject)
            {
                UiManager.Instance.UpdateDisplayedAttributes(UnitSelections.Instance.selectedUnit.GetComponent<Unit>());
            }
            healthBar.UpdateHealthbar(GetMaxHealth(), GetCurrentHealth());
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
            Debug.Log("used");
            specialAttackSO.arrowRainSO.UseSpecialAttack(currentEnemy.transform.position);
            specialAttackCounter = 0;
            
        }
    }
    public override void LevelUp()
    {
        SetCurrentLevel(GetCurrentLevel() + 1);
        EnlargeScale();
        SetLevelParticle();
        PlayLevelUpSound();
        healthBar.UpdateHealthbar(GetMaxHealth(), GetCurrentHealth());
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
        int randomAudio = Random.Range(0, damageAudios.Count);
        audioSource.clip = damageAudios[randomAudio];
        audioSource.Play();
    }
    public void PlayLevelUpSound()
    {
        audioSource.clip = levelUpSound;
        audioSource.Play();
    }
}
