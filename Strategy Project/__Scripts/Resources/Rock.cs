using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Rock : ResourceBase
{
    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private GameObject visual;

    private void Start()
    {
        currentHitPoint = resourceDataSO.hitPoint;
        SetRandomScale();
    }
    
    public override void Collect(float productivityMultiplier)
    {
        ResourceManager.Instance.IncreaseCurrentStoneAmount(resourceDataSO.rewardPerSecond * productivityMultiplier);
        UiManager.Instance.UpdateResourcesTexts();
    }
    public override void Produce(float productValue)
    {
        ResourceManager.Instance.IncreaseCurrentStoneAmount(productValue);
        UiManager.Instance.UpdateResourcesTexts();
    }
    public override void DecreseHitPoint(Worker worker,float productivityMultiplier)
    {

        currentHitPoint -= 1f * productivityMultiplier;

        if (currentHitPoint <= 0)
        {
            ParticleSystem newParticle = Instantiate(explosionParticle, transform.position + Vector3.up, Quaternion.identity);
            Destroy(newParticle, 2f);
            Destroy(gameObject);
        }
    }
   
    

    //called by animation
    public void CloseVisual()
    {
        visual.SetActive(false);
    }


    private void OnMouseEnter()
    {
        if (UnitSelections.Instance.selectedUnit != null)
        {
            if (UnitSelections.Instance.selectedUnit.GetComponent<Worker>() != null)
            {
                PlayerInputManager.Instance.SwitchToCollectCursor();
            }
            
        }

    }
    private void OnMouseExit()
    {
        PlayerInputManager.Instance.SwitchToDefaultCursor();
    }

}
