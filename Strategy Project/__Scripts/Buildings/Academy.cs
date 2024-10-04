using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Academy : Building
{
    private void Start()
    {
        SetAcademyButtonAttributes();
        SetRotation();
        PlayBuildSound();
    }
    private void OnEnable()
    {
        StartCoroutine(BorderController());
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
    private void SetAcademyButtonAttributes()
    {
        for (int i = 0; i < structureDataSO.attributesClickEventList.Count; i++)
        {
            structureDataSO.attributesClickEventList[i] = ClickEventsManager.Instance.academyAttributesList[i];
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
