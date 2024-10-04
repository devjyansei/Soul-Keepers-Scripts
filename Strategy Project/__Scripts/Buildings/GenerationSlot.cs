using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
[System.Serializable]
public class GenerationSlot : MonoBehaviour
{
    private int storedWood;
    private int storedStone;
    private int storedDiamond;

    [SerializeField] private GameObject spriteHolder;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject border;

    public bool isFull;//make getter

    
    public void StartGenerate(Action eventArg,float generateSpeed)
    {
        fillImage.gameObject.SetActive(true);
        StartCoroutine(KeepGenerate(eventArg,generateSpeed));
    }

    IEnumerator KeepGenerate(Action eventArg,float generateSpeed)
    {
        spriteHolder.SetActive(true);
        border.SetActive(true);
        while (isFull)
        {
            fillImage.fillAmount += generateSpeed * Time.deltaTime;

            if (fillImage.fillAmount >= 1)
            {
                eventArg?.Invoke();
                SetSlotEmpty();
                
            }
            yield return new WaitForSeconds(1f);

        }
    }

    
    public void SetSlotEmpty()
    {
        isFull = false;
        spriteHolder.SetActive(false);
        border.SetActive(false);
        fillImage.gameObject.SetActive(false);

        transform.GetChild(0).GetComponent<Image>().sprite = null;
        fillImage.fillAmount = 0;
    }

    // called when generate button clicked and canceled
    public void CancelGenerate()
    {
        GainBackStoredResources();
        EventManager.OnResourcesUpdated?.Invoke();
        SetSlotEmpty();
        
    }
    public void SetStoredResources(int woodCost,int stoneCost,int diamondCost)
    {
        storedWood = woodCost;
        storedStone = stoneCost;
        storedDiamond = diamondCost;
    }
    public void GainBackStoredResources()
    {
        ResourceManager.Instance.IncreaseCurrentWoodAmount(storedWood);
        ResourceManager.Instance.IncreaseCurrentStoneAmount(storedStone);
        ResourceManager.Instance.IncreaseCurrentDiamondAmount(storedDiamond);

        storedWood = 0;
        storedStone = 0;
        storedDiamond = 0;
    }
}
