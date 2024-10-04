using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Worker : Unit
{
    Rigidbody rb;
    public WorkerSO workerDataSO;
    [SerializeField] Core core;
    //[SerializeField] private float productivity=1f;
    HealthBar healthBar;
    [SerializeField] GameObject healthbarObject;
    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] private bool isOnWorkWayPoint;
    [SerializeField] private bool canWork;
    public float productivity;
    public bool isInBase;

    [SerializeField] ICollectable targetCollectable;

    public LayerMask collectable;
    public WorkerStates currentState;


    //------------------GETTER SETTER-----------------//
    public float GetProductivity() { return productivity; }
    public void SetProductivity(float value) => productivity = value;


    public enum WorkerStates
    {
        idle,
        move,
        work
    }

    public void SwitchWorkerState(WorkerStates state)
    {
        currentState = state;

        switch (currentState)
        {
            case WorkerStates.idle:
                rb.velocity = Vector3.zero;
                agent.isStopped = true;
                isOnWorkWayPoint = false;
                canWork = true;
                PlayerInputManager.Instance.DestroyMoveMarker();


                isWalking = false;
                animator.SetBool(IS_WALKING, isWalking);

                isWorking = false;
                animator.SetBool(IS_WORKING, isWorking);


                break;

            case WorkerStates.move:
                agent.isStopped = false;
                canWork = true;
                

                isWalking = true;
                animator.SetBool(IS_WALKING, isWalking);

                isWorking = false;
                animator.SetBool(IS_WORKING, isWorking);

                RotateUnit();
                break;

            case WorkerStates.work:
                agent.isStopped = true;
                LookAtResource();
                PlayerInputManager.Instance.DestroyMoveMarker();

                isWalking = false;
                animator.SetBool(IS_WALKING, isWalking);

                isWorking = true;
                animator.SetBool(IS_WORKING, isWorking);


                RotateUnit();
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
        rb = GetComponent<Rigidbody>();
    }
    
    private void Start()
    {
        SetWorkerStats();

        AddToUnitList(this.gameObject);
        SwitchWorkerState(WorkerStates.idle);
        canWork = true;
        isInBase = true;
        SetProductivity(workerDataSO.productivity);             
        SetWorkerButtonAttributes();
    }
    private void SetWorkerButtonAttributes()
    {
        for (int i = 0; i < workerDataSO.attributesClickEventList.Count; i++)
        {
            workerDataSO.attributesClickEventList[i] = ClickEventsManager.Instance.workerAttributesList[i];
        }
    }
    private void SetWorkerStats()
    {
        SetSpeed(unitDataSO.speed);
        SetMaxHealth(workerDataSO.maxHealth);
        SetCurrentHealth(GetMaxHealth());
        SetArmor(workerDataSO.armor);
        SetProductivity(workerDataSO.productivity);
    }
    private void OnDestroy()
    {
       // base.RemoveFromAllUnitLists();
    }
    private void Update()
    {


        if (Input.GetMouseButtonDown(1) && isSelected)
        {
            
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            isOnWorkWayPoint = false;

            if (BuildingManager.Instance.currentState != BuildingManager.BuildingStates.placingCopy)
            {
                //normal move
                if (Physics.Raycast(ray, out hit, rayDistance, ground))
                {

                    agent.SetDestination(hit.point);
                    SwitchWorkerState(WorkerStates.move);
                    StartCoroutine(CheckRemainingNormalPath());
                    canWork = true;


                }
                if (Physics.Raycast(ray, out hit, rayDistance, collectable) && currentState != WorkerStates.work)
                {
                    //SwitchWorkerState(WorkerStates.move);
                    //agent.SetDestination(hit.point);
                    isOnWorkWayPoint = true;
                    canWork = true;
                    targetCollectable = null;
                    targetCollectable = hit.collider.GetComponent<ICollectable>();
                    PlayerInputManager.Instance.DestroyMoveMarker();

                }

                

               
            }

            
        }

    }
    public void CollectResource()
    {

        targetCollectable.Collect(workerDataSO.productivity);
        targetCollectable.DecreseHitPoint(this, workerDataSO.productivity);
    }
    public void CheckIsResourceAlive()
    {
        if (targetCollectable.GetCurrentHitPoint() <= 0)
        {
            SwitchWorkerState(WorkerStates.idle);
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {



        if (other.gameObject.CompareTag("Collectable") && isOnWorkWayPoint && targetCollectable == other.GetComponent<ICollectable>())
        {
            
            SwitchWorkerState(WorkerStates.work);
            //StartCoroutine(StartWorkOnCollectible(other.GetComponent<ICollectable>()));
            
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Collectable") && isOnWorkWayPoint && targetCollectable == other.GetComponent<ICollectable>() && isWorking == false)
        {
            SwitchWorkerState(WorkerStates.work);
        }
    }
  
  
    protected IEnumerator CheckRemainingNormalPath()
    {
        yield return new WaitForSeconds(0.1f); // frame hatasý olmamasý için

        while (true)
        {
            if (IsUnitsPathEnded())
            {
                //yürüme bittiðinde idle geçmek için
                SwitchWorkerState(WorkerStates.idle);
                break;
            }
            yield return new WaitForSeconds(remainingPathCheckInterval);

        }
    }
    protected bool IsUnitsPathEnded()
    {
        float distance = agent.remainingDistance;
        if (distance != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0) //when Arrived
        {
            
            return true;
        }
        return false;
    }
    public void LookAtResource()
    {
        Collider[] unitColliders = Physics.OverlapSphere(this.transform.position, 2f, collectable);

        if (unitColliders.Length != 0)
        {

            transform.LookAt(unitColliders[0].transform.position);
        }


    }

    public override void TakeDamage(int amount)
    {
        int currentHealth = GetCurrentHealth();
        SetCurrentHealth(currentHealth -= amount);
        healthBar.UpdateHealthbar(maxHealth, currentHealth);

        if (GetCurrentHealth() <= 0)
        {
            Die();
            Debug.Log("died");
        }
    }

    protected override void Die()
    {
        isAlive = false;
        animator.SetTrigger("death");
        GetComponent<CapsuleCollider>().enabled = false;
        agent.isStopped = true;

        if (UnitSelections.Instance.selectedUnit == this.gameObject)
        {

            UnitSelections.Instance.selectedUnit = null;
            UiManager.Instance.CloseControlPanel();
            SetIsSelected(false);
            healthbarObject.SetActive(false);
        }
        this.enabled = false;
        Destroy(gameObject, 6f);
    }
    public void PlayHitEffect()
    {
        hitEffect.Play();   
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
}
