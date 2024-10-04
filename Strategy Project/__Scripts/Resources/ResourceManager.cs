using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoSingleton<ResourceManager>
{
    [SerializeField] private float woodStartAmount;
    [SerializeField] private float stoneStartAmount;
    [SerializeField] private float diamondStartAmount;
    [SerializeField] private int soulStartAmount;


    [Header("Resources")]
    [SerializeField] private float currentWoodAmount;
    [SerializeField] private float currentStoneAmount;
    [SerializeField] private float currentDiamondAmount;
    [SerializeField] private int currentSoulAmount;

    [SerializeField] private Animator soulTextAnimator;
    public float GetCurrentWoodAmount()
    {
        return currentWoodAmount;
    }
    public float GetCurrentStoneAmount()
    {
        return currentStoneAmount;
    }
    public float GetCurrentDiamondAmount()
    {
        return currentDiamondAmount;
    }
    public int GetCurrentSoulAmount()
    {
        return currentSoulAmount;
    }

    public void IncreaseCurrentWoodAmount(float amount)
    {
        currentWoodAmount += amount;
        EventManager.OnResourcesUpdated?.Invoke();
    }
    public void IncreaseCurrentStoneAmount(float amount)
    {
        currentStoneAmount += amount;
        EventManager.OnResourcesUpdated?.Invoke();
    }
    public void IncreaseCurrentDiamondAmount(float amount)
    {
        currentDiamondAmount += amount;
        EventManager.OnResourcesUpdated?.Invoke();
    }
    public void IncreaseCurrentSoulAmount(int amount)
    {
        currentSoulAmount += amount;
        EnablePopupSoulText();
        EventManager.OnResourcesUpdated?.Invoke();
    }
    public void DecreaseCurrentWoodAmount(float amount)
    {
        currentWoodAmount -= amount;
        EventManager.OnResourcesUpdated?.Invoke();
    }
    public void DecreaseCurrentStoneAmount(float amount)
    {
        currentStoneAmount -= amount;
        EventManager.OnResourcesUpdated?.Invoke();
    }
    public void DecreaseCurrentDiamondAmount(float amount)
    {
        currentDiamondAmount -= amount;
        EventManager.OnResourcesUpdated?.Invoke();
    }
    public void DecreaseCurrentSoulAmount(int amount)
    {
        currentSoulAmount -= amount;
        EventManager.OnResourcesUpdated?.Invoke();
    }
    private void Start()
    {
        currentWoodAmount = woodStartAmount;
        currentStoneAmount = stoneStartAmount;
        currentDiamondAmount = diamondStartAmount;
        currentSoulAmount = soulStartAmount;

        EventManager.OnResourcesUpdated?.Invoke();

    }

    public void EnablePopupSoulText()
    {
        soulTextAnimator.SetBool("popup",true);
        Invoke("DisablePopupSoulText", 0.2f);
    }
    public void DisablePopupSoulText()
    {
        soulTextAnimator.SetBool("popup", false);
    }
}
