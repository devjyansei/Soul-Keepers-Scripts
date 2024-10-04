using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class House : Building
{
    [SerializeField] HouseSO houseSO;
    private BoxCollider bCollider;
    private void Awake()
    {
        bCollider = GetComponent<BoxCollider>();
    }
    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (BuildingManager.Instance.currentState != BuildingManager.BuildingStates.placingCopy)
            {
                UiManager.Instance.ClearAllButtonEventSetting();
                BuildingManager.Instance.SetSelectedBuilding(this);

            }

            UiManager.Instance.UpdateDisplayedAttributes(this); // image ve butonlarý ayarlar

            EventManager.OnAnyBuildingSelected?.Invoke(this);
        }

        UiManager.Instance.CloseControlPanel();
    }
    private void OnEnable()
    {
        bCollider.enabled = false;
    }
    private void Start()
    {
        SetHouseButtonAttributes();

        PopulationManager.Instance.IncreasePopulationLimit(houseSO.populationLimitIncreaseAmount);
        UiManager.Instance.UpdatePopulationText();
        PlayBuildSound();
    }
    private void SetHouseButtonAttributes()
    {
        for (int i = 0; i < structureDataSO.attributesClickEventList.Count; i++)
        {
            structureDataSO.attributesClickEventList[i] = ClickEventsManager.Instance.houseAttributesList[i];
        }
    }
    
}
