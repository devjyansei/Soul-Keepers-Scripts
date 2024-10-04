using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public StructureDataSO structureDataSO;
    [SerializeField] protected GameObject[] selectedBorder;
    [SerializeField] ParticleSystem constructParticle;
    public List<Tile> coveredTiles = new List<Tile>();
    [SerializeField] protected ParticleSystem upgradeParticle;

    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip buildSound;
    [SerializeField] protected AudioClip upgradeSound;

    private void Start()
    {
        BuildingManager.Instance.buildingsList.Add(this);
        constructParticle.Play();
    }

    


    private void OnMouseDown()
    {
        
        BuildingManager.Instance.ClearSelectedStructure();
        
        
    }






    public void ActivateSelectedBorders(Building building)
    {
        for (int i = 0; i < selectedBorder.Length; i++)
        {
            building.selectedBorder[i].SetActive(true);
        }
    }

    public void DeactivateSelectedBorders(Building building)
    {
        for (int i = 0; i < selectedBorder.Length; i++)
        {
            building.selectedBorder[i].SetActive(false);
        }
    }




    






    // ------------PUBLIC METHODS----------------//

    //parayý öder
    protected void PayAttributesCost(int attributeListOrder)
    {
        ResourceManager.Instance.DecreaseCurrentWoodAmount(structureDataSO.attributesWoodCostsList[attributeListOrder]);
        ResourceManager.Instance.DecreaseCurrentStoneAmount(structureDataSO.attributesStoneCostsList[attributeListOrder]);
        ResourceManager.Instance.DecreaseCurrentDiamondAmount(structureDataSO.attributesDiamondCostsList[attributeListOrder]);
    }
    //paranýn yetip yetmedigini kontrol eder
    protected bool IsAffordAttribute(int attributeListOrder)
    {
        if (ResourceManager.Instance.GetCurrentWoodAmount() >= structureDataSO.attributesWoodCostsList[0]
             && ResourceManager.Instance.GetCurrentStoneAmount() >= structureDataSO.attributesStoneCostsList[0]
             && ResourceManager.Instance.GetCurrentDiamondAmount() >= structureDataSO.attributesDiamondCostsList[0])
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    protected bool IsAttributeReusable(int attributeListOrder)
    {
        if (structureDataSO.isAttributeReusable[attributeListOrder])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void PlayBuildSound()
    {
        audioSource.clip = buildSound;
        audioSource.Play();
    }
    
}
