using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StoneGenerator : Building
{
    [SerializeField] ResourceBase resourceData;
    [SerializeField] StoneGeneratorSO stoneGeneratorDataSO;

    private float productivity;

    public float GetProductivity() { return productivity; }
    public void SetProductivity(float value) { productivity = value; }
    private void OnEnable()
    {
        StartCoroutine(BorderController());
    }
    private void Start()
    {
        SetStoneGeneratorStats();
        SetWoodGeneratorButtonAttributes();
        StartCoroutine(StartGenerateStone());
        SetRotation();
        PlayBuildSound();
        //for testing
        stoneGeneratorDataSO.stoneGeneratorProductivityCurrentIncrease = 0;

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
    private void SetRotation()
    {
        if (transform.position.z > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        if (transform.position.z < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
    private void SetWoodGeneratorButtonAttributes()
    {
        for (int i = 0; i < structureDataSO.attributesClickEventList.Count; i++)
        {
            structureDataSO.attributesClickEventList[i] = ClickEventsManager.Instance.stoneGeneratorAttributesList[i];
        }
    }
    private IEnumerator StartGenerateStone()
    {

        while (true)
        {
            resourceData.Produce(productivity*10);
            yield return new WaitForSeconds(1f);
        }
    }
    private void SetStoneGeneratorStats()
    {
        SetProductivity(stoneGeneratorDataSO.productivity);
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
            yield return new WaitForSeconds(0.15f);
        }
    }
    public void PlayUpgradeParticle()
    {
        upgradeParticle.Play();
    }
    public void PlayUpgradeSound()
    {
        audioSource.clip = upgradeSound;
        audioSource.Play();
    }
}
