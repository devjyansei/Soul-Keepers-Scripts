using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour
{
    [SerializeField] public UnitDataSO unitDataSO;
    [SerializeField] protected SpecialAttackSO specialAttackSO;
    [SerializeField] protected GameObject[] onHoverBorder;
    [SerializeField] protected GameObject[] selectedBorder;
    public TextMeshProUGUI levelText;
    [SerializeField] float scaleUpgradeAmount;
    [SerializeField] protected int specialAttackCounter;
    [SerializeField] protected int specialAttackDelay;

    protected NavMeshAgent agent;
    protected Animator animator;

    [SerializeField] protected Enemy currentEnemy;
    [SerializeField] protected Vector3 targetPosition;
    Vector3 screenPosition;

    protected float remainingPathCheckInterval = 0.1f;
    protected float rayDistance = 1000;
    [SerializeField] protected bool isSelected;
    public bool isAlive = true;
    public bool isSpecialAttackActive = false;

    [SerializeField] protected bool isWalking = false;
    [SerializeField] protected bool isWorking = false;
    [SerializeField] protected bool isAttacking = false;

    protected const string IS_WALKING = "isWalking";
    protected const string IS_WORKING = "isWorking";
    protected const string IS_ATTACKING = "isAttacking";

    public LayerMask ground;
    public LayerMask enemy;

    [SerializeField] protected int currentLevel;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected float currentExperience;
    [SerializeField] protected int damage;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected int armor;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float detectionRange;
    [SerializeField] protected int healthRegenPerFiveSecond;

    


    //INCREASEMENT LIMITS
    [SerializeField] private int currentDamageIncreasement = 0;
    [SerializeField] private int currentDefensiveIncreasement = 0;
    [SerializeField] private int currentUtilityIncreasement = 0;

    public List<Enemy> enemiesInRadiusList = new List<Enemy>();

    public ParticleSystem upgradeParticle;
    public ParticleSystem academyUpgradeParticle;
    public ParticleSystem healParticle;
    [SerializeField] ParticleSystem level3Particle;
    [SerializeField] ParticleSystem level6Particle;
    [SerializeField] ParticleSystem level9Particle;

    [SerializeField] protected List<AudioClip> damageAudios = new List<AudioClip>();
    [SerializeField] protected List<AudioClip> dieAudios = new List<AudioClip>();

    [SerializeField] protected AudioClip levelUpSound;

    [SerializeField] protected AudioSource audioSource;


    // GETTER - SETTER
    public int GetLevel() { return currentLevel; }
    public void SetLevel(int value) => currentLevel = value;
    public float GetSpeed() { return agent.speed; }
    public void SetSpeed(float value) => agent.speed = value;
    public int GetMaxHealth() { return maxHealth; }
    public void SetMaxHealth(int value) => maxHealth = value;
    public int GetCurrentHealth() { return currentHealth; }
    public void SetCurrentHealth(int value) => currentHealth = value;
    public int GetDamage() { return damage; }
    public void SetDamage(int value) => damage = value;
    public float GetAttackSpeed() { return attackSpeed; }

    public int GetArmor() { return armor; }
    public void SetArmor(int value) => armor = value;
    public float GetAttackRange() { return attackRange; }
    public void SetAttackRange(float value) => attackRange = value;
    public float GetDetectionRange() { return detectionRange; }
    public void SetDetectionRange(float value) => detectionRange = value;
    public int GetHealthRegenPerFiveSecond() { return healthRegenPerFiveSecond; }
    public void SetHealthRegenPerFiveSecond(int value) => healthRegenPerFiveSecond = value;
    public float GetCurrentExperience() { return currentExperience; }
    public void SetCurrentExperience(float value) => currentExperience = value;


    public int GetCurrentDamageIncreasement() { return currentDamageIncreasement; }
    public void SetCurrentDamageIncreasement(int value) => currentDamageIncreasement = value;

    public int GetCurrentDefensiveIncreasement() { return currentDefensiveIncreasement; }
    public void SetCurrentDefensiveIncreasement(int value) => currentDefensiveIncreasement = value;

    public int GetCurrentUtilityIncreasement() { return currentUtilityIncreasement; }
    public void SetCurrentUtilityIncreasement(int value) => currentUtilityIncreasement = value;

    public int GetCurrentLevel() { return currentLevel; }
    public void SetCurrentLevel(int value) => currentLevel = value;


    private void OnMouseDown()
    {
       

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (BuildingManager.Instance.currentState != BuildingManager.BuildingStates.placingCopy && !GameManager.Instance.IsGameEnded())
            {
                UnitSelections.Instance.ClearSelectedUnit();
                UnitSelections.Instance.ClearSelectedGroup();
                UnitSelections.Instance.SetSelectedGroupText(this);
                
                EventManager.OnAnyUnitSelected?.Invoke(this);
            }
        }
        
        
    }
    private void OnMouseEnter()
    {
        ActivateHoverBorders(this);
    }
    private void OnMouseExit()
    {
        DeactivateHoverBorders(this);

    }
    
    public void ActivateHoverBorders(Unit unit)
    {

        if (UnitSelections.Instance.selectedUnit != this.gameObject)
        {
            for (int i = 0; i < onHoverBorder.Length; i++)
            {
                unit.onHoverBorder[i].SetActive(true);
            }
        }
        


    }
    public void DeactivateHoverBorders(Unit unit)
    {


        for (int i = 0; i < onHoverBorder.Length; i++)
        {
            unit.onHoverBorder[i].SetActive(false);
        }



    }
    public void ActivateSelectedBorders(Unit unit)
    {

        for (int i = 0; i < selectedBorder.Length; i++)
        {
            unit.selectedBorder[i].SetActive(true);
        }
        DeactivateHoverBorders(unit);

    }

    public void DeactivateSelectedBorders(Unit unit)
    {

      
        for (int i = 0; i < selectedBorder.Length; i++)
        {
                unit.selectedBorder[i].SetActive(false);
        }
        


    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        
    }

   
    
    private void OnDestroy()
    {
        RemoveFromAllUnitLists();

    }
    

    protected void SetCurrentEnemy(Enemy enemy)
    {
        currentEnemy = enemy;
    }
    protected void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
    }

    public void SetIsSelected(bool state)
    {
        isSelected = state;
    }
    public bool GetIsSelected()
    {
        return isSelected;
    }

    protected void RemoveFromAllUnitLists()
    {
        if (UnitSelections.Instance.unitsSelectedList.Contains(this.gameObject))
        {
            UnitSelections.Instance.unitsSelectedList.Remove(this.gameObject);
        }
        UnitSelections.Instance.unitList.Remove(this.gameObject);
        PopulationManager.Instance.DecreaseCurrentPopulation(1);
        UiManager.Instance.UpdatePopulationText();
    }
    public void RemoveSelectedUnit()
    {
        //UnitSelections.Instance.unitsSelectedList.Remove(this.gameObject);
        SetIsSelected(false);
        DeactivateSelectedBorders(this);
    }
    protected void AddToUnitList(GameObject unit)
    {       
        UnitSelections.Instance.unitList.Add(unit);
        PopulationManager.Instance.IncreaseCurrentPopulation(1);
        UiManager.Instance.UpdatePopulationText();
        
    }

    protected void RotateUnit()
    {
        this.transform.LookAt(agent.destination);
    }

    protected void SetEnemiesInRadius()
    {
        enemiesInRadiusList.Clear();

        Collider[] enemyColliders = Physics.OverlapSphere(this.transform.position, detectionRange, enemy);

        foreach (var enemyCollider in enemyColliders)
        {
            if (enemyCollider.GetComponent<Enemy>().GetCurrentHealth() > 0)
            {
                enemiesInRadiusList.Add(enemyCollider.GetComponent<Enemy>());
            }

        }


    }


    public virtual void TakeDamage(int amount)
    {
        int currentHealth = GetCurrentHealth();
        SetCurrentHealth(currentHealth -= amount);
        if (GetCurrentHealth() <= 0)
        {
            Die();
            Debug.Log("died");
        }
    }
    protected virtual void Die()
    {
        isAlive = false;        
        animator.SetTrigger("death");
        

        this.enabled = false;
        Destroy(gameObject,6f);
    }
    protected bool IsPathEnded()
    {
        float distance = agent.remainingDistance;
        if (distance != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <= agent.radius ) //when Arrived
        {
            return true;
        }
        return false;
    }
    
    public virtual void LevelUp()
    {
        SetCurrentLevel(GetCurrentLevel() + 1);
        EnlargeScale();
        SetLevelParticle();
        
    }
    public void EnlargeScale()
    {
        Vector3 targetScale = transform.localScale + new Vector3(scaleUpgradeAmount, scaleUpgradeAmount, scaleUpgradeAmount);
        transform.DOScale(targetScale, 0.3f);
         
    }

    public void SetLevelParticle()
    {
        if (GetLevel() >= 9)
        {
            level3Particle.gameObject.SetActive(false);
            level6Particle.gameObject.SetActive(false);
            level9Particle.gameObject.SetActive(true);
        }
        else if (GetLevel() >= 6)
        {
            level3Particle.gameObject.SetActive(false);
            level6Particle.gameObject.SetActive(true);
            level3Particle.gameObject.SetActive(false);
        }
        else if (GetLevel() >= 3)
        {
            level3Particle.gameObject.SetActive(true);
            level6Particle.gameObject.SetActive(false);
            level9Particle.gameObject.SetActive(false);
        }
    }
    
   

    

}
