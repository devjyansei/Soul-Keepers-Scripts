using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyDataSO enemyDataSO;
    [SerializeField] GameObject soulPrefab;
    protected NavMeshAgent agent;
    protected Animator animator;
    [SerializeField] GameObject healthbarObject;
    [SerializeField] GameObject[] onSelectedHoverBorder;
    [SerializeField] GameObject difficultyIcon;

    [SerializeField] ParticleSystem deathParticle;
    [SerializeField] protected ParticleSystem hitParticle;

    [SerializeField] protected float detectionCheckDelay;
    public bool isAlive = true;

    [SerializeField] protected float speed;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int damage;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float detectionRange;
    [SerializeField] protected float diamondRewardAmount;
    [SerializeField] protected float rewardMultiplier;


    protected bool isWalking = false;
    protected bool isAttacking = false;

    [SerializeField] private int maxSoulDropNumber;


    protected const string IS_WALKING = "isWalking";
    protected const string IS_ATTACKING = "isAttacking";


    [SerializeField] protected LayerMask unit;

    public List<Unit> unitsInRadiusList = new List<Unit>();

    [SerializeField] protected List<AudioClip> hitSounds = new List<AudioClip>();
    [SerializeField] protected List<AudioClip> damageSounds = new List<AudioClip>();
    [SerializeField] protected List<AudioClip> dieSounds = new List<AudioClip>();

    protected AudioSource audioSource;

    // GETTER - SETTER
    public float GetSpeed() { return speed; }
    public void SetSpeed(float value) => agent.speed = value;
    public int GetMaxHealth() { return maxHealth; }
    public void SetMaxHealth(int value) => maxHealth = value;
    public int GetCurrentHealth() { return currentHealth; }
    public void SetCurrentHealth(int value) => currentHealth = value;
    public float GetAttackSpeed() { return attackSpeed; }

    public void SetAttackSpeed(float value) => attackSpeed = value;

    public int GetDamage() { return damage; }
    public void SetDamage(int value) => damage = value;
    public float GetAttackRange() { return attackRange; }
    public void SetAttackRange(float value) => attackRange = value;
    public float GetDetectionRange() { return detectionRange; }
    public void SetDetectionRange(float value) => detectionRange = value;

    public float GetDiamondRewardAmount() { return diamondRewardAmount; }
    public void SetDiamondRewardAmount(float value) => diamondRewardAmount = value;

    public float GetRewardMultiplier() { return rewardMultiplier; }
    public void SetRewardMultiplier(float value) => rewardMultiplier = value;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    
    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        
        if (currentHealth <= 0)
        {
           //die
        }
        
    }
    protected void SetStats()
    {
        SetSpeed(enemyDataSO.speed);
        SetMaxHealth(enemyDataSO.maxHealth);
        SetDamage(enemyDataSO.damage);
        SetAttackSpeed(enemyDataSO.attackSpeedMultiplier);
        SetAttackRange(enemyDataSO.attackRange);
        SetDetectionRange(enemyDataSO.detectionRange);
        SetDiamondRewardAmount(enemyDataSO.diamondReward);
        SetRewardMultiplier(enemyDataSO.rewardMultiplier);
        SetCurrentHealth(GetMaxHealth());
    }
    /*
    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            healthBar.UpdateHealthbar(maxHealth, currentHealth);

            //die
        }
        if (GetComponent<Mutant>() != null)
        {
            Mutant enemy = GetComponent<Mutant>();

            if (currentHealth <= 50 && enemy.GetIsRaged() == false)
            {
                enemy.SwitchMutantState(Mutant.MutantStates.rageing);

            }
        }


    }
    */
    protected void SetUnitsInRadius()
    {
        unitsInRadiusList.Clear();

        Collider[] unitColliders = Physics.OverlapSphere(this.transform.position, detectionRange, unit);

        foreach (var unitCollider in unitColliders)
        {
            if (unitCollider.GetComponent<Unit>().GetCurrentHealth() > 0)
            {
                unitsInRadiusList.Add(unitCollider.GetComponent<Unit>());
            }

        }


    }
    private void OnMouseEnter()
    {
        if (UnitSelections.Instance.selectedUnit == null)
        {
            //
        }

        if (UnitSelections.Instance.selectedUnit != null )
        {
            for (int i = 0; i < onSelectedHoverBorder.Length; i++)
            {
                onSelectedHoverBorder[i].SetActive(true);

                if (UnitSelections.Instance.selectedUnit.GetComponent<Worker>() == null)
                {
                    PlayerInputManager.Instance.SwitchToAttackCursor();
                }
                
            }
        }
    }
    private void OnMouseExit()
    {
        for (int i = 0; i < onSelectedHoverBorder.Length; i++)
        {
            onSelectedHoverBorder[i].SetActive(false);
            PlayerInputManager.Instance.SwitchToDefaultCursor();
        }
    }













    // called from mutant swipe animation event










    protected Unit GetClosestUnit()
    {
        Unit closestUnit = null;
        float closestDistance=Mathf.Infinity;

        if (unitsInRadiusList.Count > 0)
        {
            for (int i = 0; i < unitsInRadiusList.Count; i++)
            {
                if (Vector3.Distance(unitsInRadiusList[i].transform.position,this.transform.position) < closestDistance)
                {
                    closestUnit = unitsInRadiusList[i];
                    closestDistance = Vector3.Distance(unitsInRadiusList[i].transform.position, this.transform.position);
                }
                
            }
            return closestUnit;
        }


        return null;
    }




    protected Unit GetLowestHealthUnit()
    {
        // if any unit in detectList
        Unit lowestHealthUnit = null;
        if (unitsInRadiusList.Count > 0)
        {
            for (int i = 0; i < unitsInRadiusList.Count; i++)
            {
                lowestHealthUnit = unitsInRadiusList[0];

                if (unitsInRadiusList[i].GetCurrentHealth() < lowestHealthUnit.GetCurrentHealth())
                {
                    unitsInRadiusList[i] = lowestHealthUnit;
                }

            }
            return lowestHealthUnit;
        }

        //if there is no unit in detectList
        else
        {
            return null;
        }


    }
    Vector3 GetTargetDirection(Vector3 target)
    {
        Vector3 direction = (target - this.transform.position).normalized;
        return direction;
    }



    protected virtual void Die()
    {
        if (isAlive)
        {
            isAlive = false;
            
            float totalReward = GetDiamondRewardAmount() * GetRewardMultiplier();
            ResourceManager.Instance.IncreaseCurrentDiamondAmount(totalReward);

            DropSoul();
            Debug.Log(totalReward);
            if (totalReward > 0)
            {
                UiManager.Instance.OpenRewardInfoPanel("Diamond ", totalReward, UiManager.Instance.diamondColor);
            }

            //para kazanma sesi

            animator.SetTrigger("death");
            GetComponent<CapsuleCollider>().enabled = false;
            deathParticle.Play();
            difficultyIcon.SetActive(false);
            healthbarObject.transform.localScale = Vector3.zero;
            agent.isStopped = true;
            agent.enabled = false;
            Destroy(gameObject, 6f);
            this.enabled = false;
        }
        
        
        
    }

    public void DropSoul()
    {
        int dropnumber = Random.Range(1, maxSoulDropNumber);

        for (int i = 0; i < dropnumber; i++)
        {
            GameObject soul = Instantiate(soulPrefab, transform.position, transform.rotation);
            Vector3 randomDirection = new Vector3(Random.Range(-20, 20), transform.position.y, Random.Range(-20, 20)).normalized;
            float randomDropForce = Random.Range(2, 4);
            soul.transform.DOJump(randomDirection * randomDropForce + transform.position, 2f, 1, 1f);
        }

        
    }
    
}
