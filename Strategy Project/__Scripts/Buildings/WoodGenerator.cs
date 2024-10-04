using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WoodGenerator : Building
{
    [SerializeField] ResourceBase resourceData;
    [SerializeField] WoodGeneratorSO woodGeneratorDataSO;

    private float productivity = 1;

    public float GetProductivity() { return productivity; }
    public void SetProductivity(float value) { productivity = value; }
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


        SetWoodGeneratorStats();
        SetWoodGeneratorButtonAttributes();
        StartCoroutine(StartGenerateWood());
        SetRotation();
        PlayBuildSound();

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
            structureDataSO.attributesClickEventList[i] = ClickEventsManager.Instance.woodGeneratorAttributesList[i];
        }
    }
    private IEnumerator StartGenerateWood()
    {

        while (true)
        {
            resourceData.Produce(productivity*10);
            yield return new WaitForSeconds(1f);
        }
    }
    private void SetWoodGeneratorStats()
    {
        SetProductivity(productivity);
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
