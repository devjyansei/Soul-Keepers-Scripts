using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.AI;
using System;
using DG.Tweening;
public class Core : Building
{
    [SerializeField] CoreSO coreSO;

    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private GameObject paladinPrefab;
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] HealthBar healthBar;

    public Vector3 unitSpawnPosition;

    public static Action OnInstantiateWorker;
    public static Action OnInstantiatePaladin;
    public static Action OnInstantiateArcher;

    [SerializeField] WorkerSO workerDataSO;
    [SerializeField] PaladinSO paladinSO;
    [SerializeField] ArcherSO archerSO;

    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    //particles
    [SerializeField] ParticleSystem energyEmissionParticle;
    [SerializeField] ParticleSystem explosionParticle;
    [SerializeField] ParticleSystem novaParticle;

    //private Button.ButtonClickedEvent clickEventHolder;

    //----------GETTER / SETTER---------------

    public int GetCurrentHealth() { return currentHealth; }
    public void SetCurrentHealth(int value) => currentHealth = value;

    private void OnEnable()
    {
        StartCoroutine(BorderController());
    }
    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && !GameManager.Instance.IsGameEnded())
        {
            if (BuildingManager.Instance.currentState != BuildingManager.BuildingStates.placingCopy)
            {
                UiManager.Instance.ClearAllButtonEventSetting();
                BuildingManager.Instance.SetSelectedBuilding(this);
                UnitSelections.Instance.SetSelectedGroupNull();
            }

            UiManager.Instance.UpdateDisplayedAttributes(this); // image ve butonlarý ayarlar

            EventManager.OnAnyBuildingSelected?.Invoke(this);
        }
        
    }
    private void Start()
    {

        SetCoreButtonAttributes();
        currentHealth = maxHealth;
        
    }

    private void SetCoreButtonAttributes()
    {
        for (int i = 0; i < structureDataSO.attributesClickEventList.Count; i++)
        {
            structureDataSO.attributesClickEventList[i] = ClickEventsManager.Instance.coreAttributesList[i];
        }
    }

    

   

    //doðru parametreleri göndermeliyim  OLD VERSIYON
    /*
    public void StartInstantiateWorker()
    {

        if (GenerationManager.Instance.IsAnyGenerationSlotEmpty())
        {
            //cost condition
            if (IsAffordAttribute(0))
            {
                PayAttributesCost(0);

                OnInstantiateWorker += InstantiateWorker;
                GenerationManager.Instance.UseGeneratareSlot(InstantiateWorker, structureDataSO.attributesWoodCostsList[0], structureDataSO.attributesStoneCostsList[0],
                    structureDataSO.attributesDiamondCostsList[0], structureDataSO.attributesImageList[0], structureDataSO.attributesGenerateSpeedList[0]);
            }

        }
    }*/
    public void StartInstantiateWorker()
    {
        if (PopulationManager.Instance.GetCurrentPopulation() < PopulationManager.Instance.GetPopulationLimit())
        {
            if (GenerationManager.Instance.IsAnyGenerationSlotEmpty())
            {
                //cost condition
                if (IsAffordAttribute(0))
                {
                    PayAttributesCost(0);

                    OnInstantiateWorker += InstantiateWorker;
                    GenerationManager.Instance.UseGeneratareSlot(InstantiateWorker, workerDataSO.woodCost, workerDataSO.stoneCost, workerDataSO.diamondCost, workerDataSO.image, workerDataSO.generateSpeed);

                }

            }
        }
        else
        {
            UiManager.Instance.OpenPopulationErrorPanel();
        }
        
    }
    public void StartInstantiatePaladin()
    {
        if (PopulationManager.Instance.GetCurrentPopulation() < PopulationManager.Instance.GetPopulationLimit())
        {
            if (GenerationManager.Instance.IsAnyGenerationSlotEmpty())
            {
                //cost condition
                if (IsAffordAttribute(1))
                {
                    PayAttributesCost(1);

                    OnInstantiatePaladin += InstantiatePaladin;
                    GenerationManager.Instance.UseGeneratareSlot(InstantiatePaladin, paladinSO.woodCost, paladinSO.stoneCost, paladinSO.diamondCost, paladinSO.image, paladinSO.generateSpeed);

                }

            }
        }
        else
        {
            UiManager.Instance.OpenPopulationErrorPanel();
        }

    }
    public void StartInstantiateArcher()
    {
        if (PopulationManager.Instance.GetCurrentPopulation() < PopulationManager.Instance.GetPopulationLimit())
        {
            if (GenerationManager.Instance.IsAnyGenerationSlotEmpty())
            {
                //cost condition
                if (IsAffordAttribute(2))
                {
                    PayAttributesCost(2);

                    OnInstantiateArcher += InstantiateArcher;
                    GenerationManager.Instance.UseGeneratareSlot(InstantiateArcher, archerSO.woodCost, archerSO.stoneCost, archerSO.diamondCost, archerSO.image, archerSO.generateSpeed);

                }

            }
        }
        else
        {
            UiManager.Instance.OpenPopulationErrorPanel();
        }
    }



    public void InstantiateWorker()
    {
        GameObject newWorker = Instantiate(workerPrefab);

        newWorker.GetComponent<NavMeshAgent>().Warp(unitSpawnPosition);
        OnInstantiateWorker -= InstantiateWorker;

    }
    
    public void InstantiatePaladin()
    {
        GameObject newPaladin = Instantiate(paladinPrefab);
        newPaladin.GetComponent<NavMeshAgent>().Warp(unitSpawnPosition);
        OnInstantiatePaladin -= InstantiatePaladin;
    }
    public void InstantiateArcher()
    {
        GameObject newArcher = Instantiate(archerPrefab);

        newArcher.GetComponent<NavMeshAgent>().Warp(unitSpawnPosition);
        OnInstantiateArcher -= InstantiateArcher;
    }

    
    public void IncreaseWorkersProductivity()
    {
        if (IsAffordAttribute(3))
        {
            PayAttributesCost(3);

            if (!IsAttributeReusable(3))
            {
                workerDataSO.productivity++;
                ClickEventsManager.Instance.coreAttributesList[3] = null; // make default
                structureDataSO.attributesClickEventList[3] = null;
                structureDataSO.attributesImageList[3] = structureDataSO.defaultSprite;
                EventManager.OnAnyBuildingSelected?.Invoke(this);
                UiManager.Instance.UpdateDisplayedAttributes(this);
                Debug.Log("increased");
            }
        }


    }
    

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        healthBar.UpdateHealthbar(maxHealth,currentHealth);
        //hit effect
        if (currentHealth <= 0 && !GameManager.Instance.IsGameEnded())
        {
            GameManager.Instance.GameOver();
        }
    }

    IEnumerator BorderController()
    {
        while (true)
        {
            if (BuildingManager.Instance.selectedStructurePrefab == this.gameObject)
            {
                ActivateSelectedBorders(this);
            }
            else if (BuildingManager.Instance.selectedStructurePrefab != this)
            {
                DeactivateSelectedBorders(this);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void PlayDestroySequence()
    {
        StartCoroutine(DestroySequence());
    }
    public IEnumerator DestroySequence()
    {

        energyEmissionParticle.Play();
        energyEmissionParticle.transform.DOScale(energyEmissionParticle.transform.localScale / 4, 3f);
        yield return new WaitForSeconds(energyEmissionParticle.main.duration / 2);
        PlayExplosionParticle();
        yield return new WaitForSeconds(explosionParticle.main.duration);
        UiManager.Instance.OpenGameOverCanvas();
    }
    private void PlayExplosionParticle()
    {
        explosionParticle.Play();

    }
    public void PlayVictorySequence()
    {
        StartCoroutine(VictorySequence());
    }
    public IEnumerator VictorySequence()
    {
        //novaParticle.Play();
        
        CameraController.Instance.TranslateCamera(Camera.main.transform.position, new Vector3(0, Camera.main.transform.position.y, -15f), .5f);
        yield return new WaitForSeconds(1);
        UiManager.Instance.OpenVictoryCanvas();

    }
}
