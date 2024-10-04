using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoSingleton<UpgradeManager>
{
    [SerializeField] GameObject attributesHolder;



    [Header("Unit Prefabs")]

    [SerializeField] GameObject paladinPrefab;
    [SerializeField] GameObject archerPrefab;


    [Header("Unit DATAS")]
    [SerializeField] private UnitDataSO paladinSO;
    [SerializeField] private UnitDataSO archerSO;
    [SerializeField] private WorkerSO workerSO;
    [SerializeField] List<UnitDataSO> unitSOsList = new List<UnitDataSO>();

    [Header("Building DATAS")]
    [SerializeField] private AcademySO academySO;
    [SerializeField] private ArcherTowerSO archerTowerSO;
    [SerializeField] private WoodGeneratorSO woodGeneratorSO;
    [SerializeField] private StoneGeneratorSO stoneGeneratorSO;

    [Header("Building Prefabs")]

    [SerializeField] GameObject corePrefab;

    [Header("Personal Increasement Limits")]
    [SerializeField] private int damageIncreasementLimit;
    [SerializeField] private int deffensiveIncreasementLimit;
    [SerializeField] private int utilityIncreasementLimit;



    [Header("Academy Increasement Limits")]
    [SerializeField] private int allDamageIncreasementLimit;
    [SerializeField] private int allDefensiveIncreasementLimit;
    [SerializeField] private int allUtilityIncreasementLimit;



    [Header("Temple")]
    [SerializeField] private List<EnemyDataSO> allInvadersList = new List<EnemyDataSO>(); // ONLY INVADERS
    [SerializeField] public List<Enemy> allNeutralsList = new List<Enemy>(); // ONLY NEUTRALS
    [SerializeField] private List<Chest> allChests = new List<Chest>();

    [SerializeField] [Tooltip("In Seconds")] private float waveProlongTimeAmount;
    [SerializeField] private float invaderSpeedDecreaseAmount;
    [SerializeField] private int invaderDamageDecreaseAmount;
    [SerializeField] private float chestRewardMultiplierAmount;
    [SerializeField] private float workerProductivityIncreaseAmount;
    [SerializeField] private float neutralEnemyRewardMultiplierAmount;
    [SerializeField] private float attackSpeedIncreasedValue;


    [Header("Archer Tower")]
    [SerializeField] private int archerTowerDamageIncreaseAmount;
    [SerializeField] private float archerTowerRangeIncreaseAmount;
    [SerializeField] private float archerTowerAttackIntervalDecreaseAmount;

    [SerializeField] private int archerTowerDamageIncreaseLimit;
    [SerializeField] private int archerTowerRangeIncreaseLimit;
    [SerializeField] private float archerTowerAttackIntervalDecreaseLimit;

    [Header("Wood Generator")]
    [SerializeField] private float woodGeneratorProductivityIncreaseAmount;
    [SerializeField] private float woodGeneratorProductivityIncreaseLimit;


    [Header("Stone Generator")]
    [SerializeField] private float stoneGeneratorProductivityIncreaseAmount;
    [SerializeField] private float stoneGeneratorProductivityIncreaseLimit;






    

    #region STONE GENERATOR

    public void IncreaseStoneGeneratorsProductivity()
    {
        if (ResourceManager.Instance.GetCurrentWoodAmount() >= stoneGeneratorSO.attributesWoodCostsList[0] &&
            ResourceManager.Instance.GetCurrentStoneAmount() >= stoneGeneratorSO.attributesStoneCostsList[0] &&
            ResourceManager.Instance.GetCurrentDiamondAmount() >= stoneGeneratorSO.attributesDiamondCostsList[0])
        {
            if (stoneGeneratorSO.stoneGeneratorProductivityCurrentIncrease < stoneGeneratorProductivityIncreaseLimit)
            {
                //PAY
                ResourceManager.Instance.DecreaseCurrentWoodAmount(stoneGeneratorSO.attributesWoodCostsList[0]);
                ResourceManager.Instance.DecreaseCurrentStoneAmount(stoneGeneratorSO.attributesStoneCostsList[0]);
                ResourceManager.Instance.DecreaseCurrentDiamondAmount(stoneGeneratorSO.attributesDiamondCostsList[0]);

                //LOGIC
                StoneGenerator generator = BuildingManager.Instance.lastSelectedStructurePrefab.GetComponent<StoneGenerator>();
                generator.SetProductivity(generator.GetProductivity() + stoneGeneratorProductivityIncreaseAmount);

                generator.PlayUpgradeParticle();
                generator.PlayUpgradeSound();

                stoneGeneratorSO.stoneGeneratorProductivityCurrentIncrease++;

                if (stoneGeneratorSO.stoneGeneratorProductivityCurrentIncrease == stoneGeneratorProductivityIncreaseLimit)
                {
                    if (stoneGeneratorSO.isAttributeReusable[0] == false)
                    {
                        stoneGeneratorSO.isAttributeUsed[0] = true;
                    }

                    UiManager.Instance.UpdateDisplayedAttributes(stoneGeneratorSO.prefab.GetComponent<Building>());
                }

            }



        }

        else
        {
            UiManager.Instance.OpenInsufficientResourcePanel();
        }


    }


    #endregion

    #region WOOD GENERATOR

    public void IncreaseWoodGeneratorsProductivity()
    {
        if (ResourceManager.Instance.GetCurrentWoodAmount() >= woodGeneratorSO.attributesWoodCostsList[0] &&
            ResourceManager.Instance.GetCurrentStoneAmount() >= woodGeneratorSO.attributesStoneCostsList[0] &&
            ResourceManager.Instance.GetCurrentDiamondAmount() >= woodGeneratorSO.attributesDiamondCostsList[0])
        {
            if (woodGeneratorSO.woodGeneratorProductivityCurrentIncrease < woodGeneratorProductivityIncreaseLimit)
            {
                //PAY
                ResourceManager.Instance.DecreaseCurrentWoodAmount(woodGeneratorSO.attributesWoodCostsList[0]);
                ResourceManager.Instance.DecreaseCurrentStoneAmount(woodGeneratorSO.attributesStoneCostsList[0]);
                ResourceManager.Instance.DecreaseCurrentDiamondAmount(woodGeneratorSO.attributesDiamondCostsList[0]);

                //LOGIC
                WoodGenerator generator = BuildingManager.Instance.lastSelectedStructurePrefab.GetComponent<WoodGenerator>();
                generator.SetProductivity(generator.GetProductivity() + woodGeneratorProductivityIncreaseAmount);

                generator.PlayUpgradeParticle();
                generator.PlayUpgradeSound();

                woodGeneratorSO.woodGeneratorProductivityCurrentIncrease++;
                Debug.Log("increased");
                if (woodGeneratorSO.woodGeneratorProductivityCurrentIncrease == woodGeneratorProductivityIncreaseLimit)
                {
                    Debug.Log("sýnýra ulastý");

                    if (woodGeneratorSO.isAttributeReusable[0] == false)
                    {
                        woodGeneratorSO.isAttributeUsed[0] = true;
                    }

                    UiManager.Instance.UpdateDisplayedAttributes(woodGeneratorSO.prefab.GetComponent<Building>());
                }
            }
            
        }
        else
        {
            UiManager.Instance.OpenInsufficientResourcePanel();
        }
    }

               
     
    #endregion

    #region  SOUL
    //-----------SOUL----------//

    public void DecreaseAllInvadersSpeed(SoulCard soulCard)
    {
        if (ResourceManager.Instance.GetCurrentSoulAmount() >= soulCard.card.cost)
        {
            
            soulCard.BuySoulCard();

            foreach (EnemyDataSO invaderSO in allInvadersList)
            {
                invaderSO.speed -= invaderSpeedDecreaseAmount;              
            }           
        }
        
    }
    public void DecreaseAllInvadersDamage(SoulCard soulCard)
    {
        if (ResourceManager.Instance.GetCurrentSoulAmount() >= soulCard.card.cost)
        {
            soulCard.BuySoulCard();

            foreach (EnemyDataSO invaderSO in allInvadersList)
            {
                invaderSO.damage -= invaderDamageDecreaseAmount;
            }  
        }
       
    }
    public void ProlongAllWavesSpawnTime(SoulCard soulCard)
    {
        if (ResourceManager.Instance.GetCurrentSoulAmount() >= soulCard.card.cost)
        {
            soulCard.BuySoulCard();
            for (int i = 0; i < WaveManager.Instance.waveList.Count; i++)
            {
                WaveManager.Instance.waveList[i].prepareTime += waveProlongTimeAmount;
            }

        }
        

    }

    public void IncreaseAllChestsRewardAmount(SoulCard soulCard)
    {
        if (ResourceManager.Instance.GetCurrentSoulAmount() >= soulCard.card.cost)
        {
            soulCard.BuySoulCard();

            for (int i = 0; i < allChests.Count; i++)
            {
                allChests[i].rewardMultiplier += chestRewardMultiplierAmount;
            }
            
        }
        
    }

    public void IncreaseAllWorkersProductivityAmount(SoulCard soulCard)
    {
        if (ResourceManager.Instance.GetCurrentSoulAmount() >= soulCard.card.cost)
        {
            soulCard.BuySoulCard();

            workerSO.productivity += workerProductivityIncreaseAmount;

        }
    }

    public void IncreaseAllNeutralsRewardMultiplier(SoulCard soulCard)
    {
        if (ResourceManager.Instance.GetCurrentSoulAmount() >= soulCard.card.cost)
        {
            soulCard.BuySoulCard();

            for (int i = 0; i < allNeutralsList.Count; i++)
            {
                allNeutralsList[i].SetRewardMultiplier(allNeutralsList[i].GetRewardMultiplier() * neutralEnemyRewardMultiplierAmount);
            }
        }
        
    }

    public void IncreaseAllUnitsAttackSpeed(SoulCard soulCard) //33%
    {

        if (ResourceManager.Instance.GetCurrentSoulAmount() >= soulCard.card.cost)
        {
            soulCard.BuySoulCard();

            for (int i = 0; i < unitSOsList.Count; i++)
            {
                paladinSO.attackSpeed = attackSpeedIncreasedValue;
                archerSO.attackSpeed = attackSpeedIncreasedValue;
            }

            Paladin[] paladins = FindObjectsOfType<Paladin>();

            for (int i = 0; i < paladins.Length; i++)
            {
                paladins[i].IncreaseAttackSpeed(attackSpeedIncreasedValue);
            }

            Archer[] archers = FindObjectsOfType<Archer>();

            for (int i = 0; i < archers.Length; i++)
            {
                archers[i].IncreaseAttackSpeed(attackSpeedIncreasedValue);
            }
        }
        
    }

    public void EnableSpecialAttack(SoulCard soulCard)
    {
        if (ResourceManager.Instance.GetCurrentSoulAmount() >= soulCard.card.cost)
        {
            soulCard.BuySoulCard();

            Archer[] archers = FindObjectsOfType<Archer>();

            for (int i = 0; i < archers.Length; i++)
            {
                archers[i].isSpecialAttackActive = true;
            }

            Paladin[] paladins = FindObjectsOfType<Paladin>();

            for (int i = 0; i < paladins.Length; i++)
            {
                paladins[i].isSpecialAttackActive = true;
            }
        }
            
    }





    #endregion
    
    #region ARCHER TOWER
    //------------ARCHER TOWER----------//

    public void IncreaseTowerDamage()
    {

        if (ResourceManager.Instance.GetCurrentWoodAmount() >= archerTowerSO.attributesWoodCostsList[0] &&
            ResourceManager.Instance.GetCurrentStoneAmount() >= archerTowerSO.attributesStoneCostsList[0] &&
            ResourceManager.Instance.GetCurrentDiamondAmount() >= archerTowerSO.attributesDiamondCostsList[0])
        {
            if (archerTowerSO.currentDamageIncreasement < archerTowerDamageIncreaseLimit)
            {
                // PAY COST
                ResourceManager.Instance.DecreaseCurrentWoodAmount(archerTowerSO.attributesWoodCostsList[0]);
                ResourceManager.Instance.DecreaseCurrentStoneAmount(archerTowerSO.attributesStoneCostsList[0]);
                ResourceManager.Instance.DecreaseCurrentDiamondAmount(archerTowerSO.attributesDiamondCostsList[0]);

                ArcherTower archerTower = BuildingManager.Instance.selectedStructurePrefab.GetComponent<ArcherTower>();

                archerTower.SetDamage(archerTower.GetDamage() + archerTowerDamageIncreaseAmount);

                archerTower.PlayUpgradeParticle();
                archerTower.PlayUpgradeSound();

                archerTowerSO.currentDamageIncreasement++;

                //artýk kullanýlamaz hale getirir.
                if (archerTowerSO.currentDamageIncreasement == archerTowerDamageIncreaseLimit)
                {
                    if (archerTowerSO.isAttributeReusable[0] == false)
                    {
                        archerTowerSO.isAttributeUsed[0] = true;
                    }

                    UiManager.Instance.UpdateDisplayedAttributes(archerTowerSO.prefab.GetComponent<Building>());
                }
            }

        }
        else
        {
            UiManager.Instance.OpenInsufficientResourcePanel();
        }

    }

    public void IncreaseTowerRange()
    {
        if (ResourceManager.Instance.GetCurrentWoodAmount() >= archerTowerSO.attributesWoodCostsList[1] &&
            ResourceManager.Instance.GetCurrentStoneAmount() >= archerTowerSO.attributesStoneCostsList[1] &&
            ResourceManager.Instance.GetCurrentDiamondAmount() >= archerTowerSO.attributesDiamondCostsList[1])
        {
            if (archerTowerSO.currentRangeIncreasement < archerTowerRangeIncreaseLimit)
            {
                // PAY COST
                ResourceManager.Instance.DecreaseCurrentWoodAmount(archerTowerSO.attributesWoodCostsList[1]);
                ResourceManager.Instance.DecreaseCurrentStoneAmount(archerTowerSO.attributesStoneCostsList[1]);
                ResourceManager.Instance.DecreaseCurrentDiamondAmount(archerTowerSO.attributesDiamondCostsList[1]);

                ArcherTower archerTower = BuildingManager.Instance.selectedStructurePrefab.GetComponent<ArcherTower>();

                archerTower.SetRange(archerTower.GetRange() + archerTowerRangeIncreaseAmount);

                archerTower.PlayUpgradeParticle();
                archerTower.PlayUpgradeSound();

                archerTowerSO.currentRangeIncreasement++;

                //artýk kullanýlamaz hale getirir.
                if (archerTowerSO.currentRangeIncreasement == archerTowerRangeIncreaseLimit)
                {
                    if (archerTowerSO.isAttributeReusable[1] == false)
                    {
                        archerTowerSO.isAttributeUsed[1] = true;
                    }

                    UiManager.Instance.UpdateDisplayedAttributes(archerTowerSO.prefab.GetComponent<Building>());
                }

            }
        }
        else
        {
            UiManager.Instance.OpenInsufficientResourcePanel();
        }
    }
    public void IncreaseTowerAttackSpeed()
    {
        if (ResourceManager.Instance.GetCurrentWoodAmount() >= archerTowerSO.attributesWoodCostsList[2] &&
            ResourceManager.Instance.GetCurrentStoneAmount() >= archerTowerSO.attributesStoneCostsList[2] &&
            ResourceManager.Instance.GetCurrentDiamondAmount() >= archerTowerSO.attributesDiamondCostsList[2])
        {
            if (archerTowerSO.currentSpeedIncreasement < archerTowerAttackIntervalDecreaseLimit)
            {
                // PAY COST
                ResourceManager.Instance.DecreaseCurrentWoodAmount(archerTowerSO.attributesWoodCostsList[2]);
                ResourceManager.Instance.DecreaseCurrentStoneAmount(archerTowerSO.attributesStoneCostsList[2]);
                ResourceManager.Instance.DecreaseCurrentDiamondAmount(archerTowerSO.attributesDiamondCostsList[2]);

                ArcherTower archerTower = BuildingManager.Instance.selectedStructurePrefab.GetComponent<ArcherTower>();

                archerTower.SetAttackInterval(archerTower.GetAttackInterval() - archerTowerAttackIntervalDecreaseAmount);

                archerTower.PlayUpgradeParticle();
                archerTower.PlayUpgradeSound();
                archerTowerSO.currentSpeedIncreasement++;

                //artýk kullanýlamaz hale getirir.
                if (archerTowerSO.currentSpeedIncreasement == archerTowerAttackIntervalDecreaseLimit)
                {
                    if (archerTowerSO.isAttributeReusable[2] == false)
                    {
                        archerTowerSO.isAttributeUsed[2] = true;
                    }

                    UiManager.Instance.UpdateDisplayedAttributes(archerTowerSO.prefab.GetComponent<Building>());
                }
            }
        }
        else
        {
            UiManager.Instance.OpenInsufficientResourcePanel();
        }

    }
    #endregion


    #region ACADEMY
    //----------------- ACADEMY ----------------------- //

    public void IncreaseAllUnitsDamages()
    {

        // COST CONDITION
        if (ResourceManager.Instance.GetCurrentWoodAmount() >= academySO.allDamageIncreaseWoodCost &&
            ResourceManager.Instance.GetCurrentStoneAmount() >= academySO.allDamageIncreaseStoneCost &&
            ResourceManager.Instance.GetCurrentDiamondAmount() >= academySO.allDamageIncreaseDiamondCost)
        {

            if (academySO.currentAllDamageIncreasement < allDamageIncreasementLimit)
            {
                Debug.Log("work");
                // PAY COST
                ResourceManager.Instance.DecreaseCurrentWoodAmount(academySO.allDamageIncreaseWoodCost);
                ResourceManager.Instance.DecreaseCurrentStoneAmount(academySO.allDamageIncreaseStoneCost);
                ResourceManager.Instance.DecreaseCurrentDiamondAmount(academySO.allDamageIncreaseDiamondCost);



                // LOGIC
                Academy academy = BuildingManager.Instance.lastSelectedStructurePrefab.GetComponent<Academy>();

                Paladin[] paladins = FindObjectsOfType<Paladin>();
                Archer[] archers = FindObjectsOfType<Archer>();

                academy.PlayUpgradeParticle();
                academy.PlayUpgradeSound();

                paladinSO.damage += academySO.allDamageIncreasementAmount;
                archerSO.damage += academySO.allDamageIncreasementAmount;

                for (int i = 0; i < paladins.Length; i++)
                {
                    paladins[i].SetDamage(paladins[i].GetDamage() + academySO.allDamageIncreasementAmount);
                    paladins[i].PlayAcademyUpgradeParticle();
                }
                for (int i = 0; i < archers.Length; i++)
                {
                    archers[i].SetDamage(archers[i].GetDamage() + academySO.allDamageIncreasementAmount);
                    archers[i].PlayAcademyUpgradeParticle();

                }
                UiManager.Instance.OpenAcademyUpgradeInfoPanel("Damage", academySO.allDamageIncreasementAmount, UiManager.Instance.damageColor);
                UiManager.Instance.UpdateResourcesTexts();

                academySO.currentAllDamageIncreasement++;

                //artýk kullanýlamaz hale getirir.
                if (academySO.currentAllDamageIncreasement == allDamageIncreasementLimit)
                {
                    if (academySO.isAttributeReusable[0] == false)
                    {
                        academySO.isAttributeUsed[0] = true;
                    }

                    UiManager.Instance.UpdateDisplayedAttributes(academySO.prefab.GetComponent<Building>());
                }
            }


        }
        else
        {
            UiManager.Instance.OpenInsufficientResourcePanel();
        }

    }


    public void IncreaseAllUnitsDefences()
    {

        // COST CONDITION
        if (ResourceManager.Instance.GetCurrentWoodAmount() >= academySO.allDefenceIncreaseWoodCost &&
            ResourceManager.Instance.GetCurrentStoneAmount() >= academySO.allDefenceIncreaseStoneCost &&
            ResourceManager.Instance.GetCurrentDiamondAmount() >= academySO.allDefenceIncreaseDiamondCost)
        {

            if (academySO.currentAllDefenceIncreasement < allDefensiveIncreasementLimit)
            {

                // PAY COST
                ResourceManager.Instance.DecreaseCurrentWoodAmount(academySO.allDefenceIncreaseWoodCost);
                ResourceManager.Instance.DecreaseCurrentStoneAmount(academySO.allDefenceIncreaseStoneCost);
                ResourceManager.Instance.DecreaseCurrentDiamondAmount(academySO.allDefenceIncreaseDiamondCost);



                // LOGIC
                Academy academy = BuildingManager.Instance.lastSelectedStructurePrefab.GetComponent<Academy>();

                Paladin[] paladins = FindObjectsOfType<Paladin>();
                Archer[] archers = FindObjectsOfType<Archer>();

                academy.PlayUpgradeParticle();
                academy.PlayUpgradeSound();

                paladinSO.armor += academySO.allArmorIncreasementAmount;
                paladinSO.armor += academySO.allMaxHealthIncreasementAmount;

                archerSO.armor += academySO.allArmorIncreasementAmount;
                archerSO.armor += academySO.allMaxHealthIncreasementAmount;

                for (int i = 0; i < paladins.Length; i++)
                {//
                    paladins[i].SetArmor(paladins[i].GetArmor() + academySO.allArmorIncreasementAmount);
                    paladins[i].SetMaxHealth(paladins[i].GetMaxHealth() + academySO.allMaxHealthIncreasementAmount);
                    paladins[i].PlayAcademyUpgradeParticle();
                }
                for (int i = 0; i < archers.Length; i++)
                {
                    archers[i].SetArmor(archers[i].GetArmor() + academySO.allArmorIncreasementAmount);
                    archers[i].SetMaxHealth(archers[i].GetMaxHealth() + academySO.allMaxHealthIncreasementAmount);
                    archers[i].PlayAcademyUpgradeParticle();
                }
                UiManager.Instance.OpenAcademyUpgradeInfoPanel("Armor", academySO.allArmorIncreasementAmount, UiManager.Instance.armorColor);
                UiManager.Instance.OpenAcademyUpgradeInfoPanel("Max Health", academySO.allMaxHealthIncreasementAmount, UiManager.Instance.maxHealthColor);
                UiManager.Instance.UpdateResourcesTexts();
                academySO.currentAllDefenceIncreasement++;

                //artýk kullanýlamaz hale getirir.
                if (academySO.currentAllDefenceIncreasement == allDefensiveIncreasementLimit)
                {
                    if (academySO.isAttributeReusable[1] == false)
                    {
                        academySO.isAttributeUsed[1] = true;
                    }

                    UiManager.Instance.UpdateDisplayedAttributes(academySO.prefab.GetComponent<Building>());
                    
                }

            }


        }
        else
        {
            UiManager.Instance.OpenInsufficientResourcePanel();
        }

    }

    public void IncreaseAllUnitsUtilitys()
    {

        // COST CONDITION
        if (ResourceManager.Instance.GetCurrentWoodAmount() >= academySO.allUtilityIncreaseWoodCost &&
            ResourceManager.Instance.GetCurrentStoneAmount() >= academySO.allUtilityIncreaseStoneCost &&
            ResourceManager.Instance.GetCurrentDiamondAmount() >= academySO.allUtiltyIncreaseDiamondCost)
        {

            if (academySO.currentAllUtilityIncreasement < allUtilityIncreasementLimit)
            {

                // PAY COST
                ResourceManager.Instance.DecreaseCurrentWoodAmount(academySO.allUtilityIncreaseWoodCost);
                ResourceManager.Instance.DecreaseCurrentStoneAmount(academySO.allUtilityIncreaseStoneCost);
                ResourceManager.Instance.DecreaseCurrentDiamondAmount(academySO.allUtiltyIncreaseDiamondCost);



                // LOGIC

                Academy academy = BuildingManager.Instance.lastSelectedStructurePrefab.GetComponent<Academy>();

                

                Paladin[] paladins = FindObjectsOfType<Paladin>();
                Archer[] archers = FindObjectsOfType<Archer>();

                academy.PlayUpgradeParticle();
                academy.PlayUpgradeSound();

                paladinSO.armor += academySO.allArmorIncreasementAmount;
                paladinSO.armor += academySO.allMaxHealthIncreasementAmount;

                archerSO.armor += academySO.allArmorIncreasementAmount;
                archerSO.armor += academySO.allMaxHealthIncreasementAmount;

                for (int i = 0; i < paladins.Length; i++)
                {
                    paladins[i].SetSpeed(paladins[i].GetSpeed() + academySO.allSpeedIncreasementAmount);
                    paladins[i].SetHealthRegenPerFiveSecond(paladins[i].GetHealthRegenPerFiveSecond() + academySO.allHealthRegenIncreasementAmount);
                    paladins[i].PlayAcademyUpgradeParticle();
                }
                for (int i = 0; i < archers.Length; i++)
                {
                    archers[i].SetSpeed(archers[i].GetSpeed() + academySO.allSpeedIncreasementAmount);
                    archers[i].SetHealthRegenPerFiveSecond(archers[i].GetHealthRegenPerFiveSecond() + academySO.allHealthRegenIncreasementAmount);
                    archers[i].PlayAcademyUpgradeParticle();
                }
                UiManager.Instance.OpenAcademyUpgradeInfoPanel("Speed", academySO.allSpeedIncreasementAmount, UiManager.Instance.speedColor);
                UiManager.Instance.OpenAcademyUpgradeInfoPanel("Health Regen", academySO.allHealthRegenIncreasementAmount, UiManager.Instance.healthRegenColor);
                UiManager.Instance.UpdateResourcesTexts();
                academySO.currentAllUtilityIncreasement++;

                //artýk kullanýlamaz hale getirir.
                if (academySO.currentAllUtilityIncreasement == allUtilityIncreasementLimit)
                {
                    if (academySO.isAttributeReusable[2] == false)
                    {
                        academySO.isAttributeUsed[2] = true;
                    }

                    UiManager.Instance.UpdateDisplayedAttributes(academySO.prefab.GetComponent<Building>());
                }

            }


        }
        else
        {
            UiManager.Instance.OpenInsufficientResourcePanel();
        }

    }


    #endregion




    #region PALADIN




    //----------------- PALADIN ----------------------- //

    public void IncreasePaladinDamage()
    {
        Paladin paladin = UnitSelections.Instance.lastSelectedUnit.GetComponent<Paladin>();

        // COST CONDITION
        if (ResourceManager.Instance.GetCurrentWoodAmount() >= paladinSO.damageIncreaseWoodCost  && 
            ResourceManager.Instance.GetCurrentStoneAmount() >= paladinSO.damageIncreaseStoneCost &&
            ResourceManager.Instance.GetCurrentDiamondAmount() >= paladinSO.damageIncreaseDiamondCost)
        {
            
            if ( paladin.GetCurrentDamageIncreasement() < damageIncreasementLimit)
            {
                //PAY COST 
                ResourceManager.Instance.DecreaseCurrentWoodAmount(paladinSO.damageIncreaseWoodCost);
                ResourceManager.Instance.DecreaseCurrentStoneAmount(paladinSO.damageIncreaseStoneCost);
                ResourceManager.Instance.DecreaseCurrentDiamondAmount(paladinSO.damageIncreaseDiamondCost);


                //DAMAGE
                UiManager.Instance.OpenInfoPanel("Damage", paladin.GetDamage(),paladin.GetDamage()+ paladinSO.damageIncreasementAmount, UiManager.Instance.damageColor);
                paladin.SetDamage(paladin.GetDamage() + paladinSO.damageIncreasementAmount);
                paladin.SetCurrentDamageIncreasement(paladin.GetCurrentDamageIncreasement() + 1);
                paladin.upgradeParticle.Play();
                paladin.LevelUp();
                UiManager.Instance.UpdateLevelText(paladin,paladin.GetCurrentLevel());
                UiManager.Instance.UpdateDisplayedAttributes(paladin);
                UiManager.Instance.UpdateResourcesTexts();

                
            }
            else
            {
                if (!UiManager.Instance.IsUpgradeErrorPanelActive())
                {
                    UiManager.Instance.OpenUpgradeErrorPanel();
                }
            }
            
        }
        else
        {
            UiManager.Instance.OpenInsufficientResourcePanel();
        }
        
    }
    public void IncreasePaladinDeffensiveStats()
    {
        Paladin paladin = UnitSelections.Instance.lastSelectedUnit.GetComponent<Paladin>();
        
        // COST CONDITION
        if (ResourceManager.Instance.GetCurrentWoodAmount() >= paladinSO.defensiveIncreaseWoodCost &&
            ResourceManager.Instance.GetCurrentStoneAmount() >= paladinSO.defensiveIncreaseStoneCost &&
            ResourceManager.Instance.GetCurrentDiamondAmount() >= paladinSO.defensiveIncreaseDiamondCost)
        {

            if (paladin.GetCurrentDefensiveIncreasement() < deffensiveIncreasementLimit)
            {

                //PAY COST 
                ResourceManager.Instance.DecreaseCurrentWoodAmount(paladinSO.defensiveIncreaseWoodCost);
                ResourceManager.Instance.DecreaseCurrentStoneAmount(paladinSO.defensiveIncreaseStoneCost);
                ResourceManager.Instance.DecreaseCurrentDiamondAmount(paladinSO.defensiveIncreaseDiamondCost);


                //HEALTH
                paladin.SetMaxHealth(paladin.GetMaxHealth() + paladinSO.maxHealthIncreasementAmount);

                //ARMOR
                UiManager.Instance.OpenInfoPanel("MaxHealth", paladin.GetMaxHealth(), paladin.GetMaxHealth() + paladinSO.maxHealthIncreasementAmount, UiManager.Instance.maxHealthColor);
                UiManager.Instance.OpenInfoPanel("Armor", paladin.GetArmor(), paladin.GetArmor() + paladinSO.armorIncreasementAmount, UiManager.Instance.armorColor);
                paladin.SetArmor(paladin.GetArmor() + paladinSO.armorIncreasementAmount);
                paladin.SetCurrentDefensiveIncreasement(paladin.GetCurrentDefensiveIncreasement() + 1);
                paladin.upgradeParticle.Play();
                paladin.LevelUp();
                UiManager.Instance.UpdateLevelText(paladin, paladin.GetCurrentLevel());
                UiManager.Instance.UpdateDisplayedAttributes(paladin);
                UiManager.Instance.UpdateResourcesTexts();

            }
            else
            {
                if (!UiManager.Instance.IsUpgradeErrorPanelActive())
                {
                    UiManager.Instance.OpenUpgradeErrorPanel();
                }
            }
        }
        else
        {
            UiManager.Instance.OpenInsufficientResourcePanel();
        }
    }

    public void IncreasePaladinUtilityStats() 
    {

        Paladin paladin = UnitSelections.Instance.lastSelectedUnit.GetComponent<Paladin>();

        // COST CONDITION
        if (ResourceManager.Instance.GetCurrentWoodAmount() >= paladinSO.utilityIncreaseWoodCost &&
            ResourceManager.Instance.GetCurrentStoneAmount() >= paladinSO.utilityIncreaseStoneCost &&
            ResourceManager.Instance.GetCurrentDiamondAmount() >= paladinSO.utilityIncreaseDiamondCost)
        {

            if (paladin.GetCurrentUtilityIncreasement() < utilityIncreasementLimit)
            {
                //PAY COST 
                ResourceManager.Instance.DecreaseCurrentWoodAmount(paladinSO.utilityIncreaseWoodCost);
                ResourceManager.Instance.DecreaseCurrentStoneAmount(paladinSO.utilityIncreaseStoneCost);
                ResourceManager.Instance.DecreaseCurrentDiamondAmount(paladinSO.utilityIncreaseDiamondCost);



                //SPEED
                UiManager.Instance.OpenInfoPanel("Speed", paladin.GetSpeed(), paladin.GetSpeed() + paladinSO.speedIncreasementAmount,UiManager.Instance.speedColor);
                UiManager.Instance.OpenInfoPanel("Health Regen", paladin.GetHealthRegenPerFiveSecond(), paladin.GetHealthRegenPerFiveSecond() + paladinSO.healthRegenIncreasementAmount, UiManager.Instance.healthRegenColor);
                paladin.SetSpeed(paladin.GetSpeed() + paladinSO.speedIncreasementAmount);
                paladin.SetCurrentUtilityIncreasement(paladin.GetCurrentUtilityIncreasement() + 1);
                paladin.upgradeParticle.transform.position = paladin.transform.position;
                paladin.upgradeParticle.Play();
                paladin.LevelUp();

                //HEALTH REGEN
                paladin.SetHealthRegenPerFiveSecond(paladin.GetHealthRegenPerFiveSecond() + paladinSO.healthRegenIncreasementAmount);

                UiManager.Instance.UpdateLevelText(paladin, paladin.GetCurrentLevel());
                UiManager.Instance.UpdateDisplayedAttributes(paladin);
                UiManager.Instance.UpdateResourcesTexts();
 
            }
            else
            {
                if (!UiManager.Instance.IsUpgradeErrorPanelActive())
                {
                    UiManager.Instance.OpenUpgradeErrorPanel();
                }
            }
        }
        else
        {
            UiManager.Instance.OpenInsufficientResourcePanel();
        }
    }
    #endregion

    #region ARCHER
    //----------------- ARCHER ----------------------- //

    public void IncreaseArcherDamage()
    {
        Archer archer = UnitSelections.Instance.lastSelectedUnit.GetComponent<Archer>(); 

        // COST CONDITION
        if (ResourceManager.Instance.GetCurrentWoodAmount() >= archerSO.damageIncreaseWoodCost &&
            ResourceManager.Instance.GetCurrentStoneAmount() >= archerSO.damageIncreaseStoneCost &&
            ResourceManager.Instance.GetCurrentDiamondAmount() >= archerSO.damageIncreaseDiamondCost)
        {

            if (archer.GetCurrentDamageIncreasement() < damageIncreasementLimit)
            {
                //PAY COST 
                ResourceManager.Instance.DecreaseCurrentWoodAmount(archerSO.damageIncreaseWoodCost);
                ResourceManager.Instance.DecreaseCurrentStoneAmount(archerSO.damageIncreaseStoneCost);
                ResourceManager.Instance.DecreaseCurrentDiamondAmount(archerSO.damageIncreaseDiamondCost);


                //DAMAGE
                UiManager.Instance.OpenInfoPanel("Damage", archer.GetDamage(), archer.GetDamage() + archerSO.damageIncreasementAmount, UiManager.Instance.damageColor);
                archer.SetDamage(archer.GetDamage() + archerSO.damageIncreasementAmount);
                archer.SetCurrentDamageIncreasement(archer.GetCurrentDamageIncreasement() + 1);
                archer.upgradeParticle.Play();
                archer.LevelUp();
                UiManager.Instance.UpdateLevelText(archer, archer.GetCurrentLevel());
                UiManager.Instance.UpdateDisplayedAttributes(archer);
                UiManager.Instance.UpdateResourcesTexts();

            }
            else
            {
                if (!UiManager.Instance.IsUpgradeErrorPanelActive())
                {
                    UiManager.Instance.OpenUpgradeErrorPanel();
                }
            }
        }
        else
        {
            UiManager.Instance.OpenInsufficientResourcePanel();
        }
    }
    public void IncreaseArcherDeffensiveStats()
    {
        Archer archer = UnitSelections.Instance.lastSelectedUnit.GetComponent<Archer>();

        // COST CONDITION
        if (ResourceManager.Instance.GetCurrentWoodAmount() >= archerSO.defensiveIncreaseWoodCost &&
            ResourceManager.Instance.GetCurrentStoneAmount() >= archerSO.defensiveIncreaseStoneCost &&
            ResourceManager.Instance.GetCurrentDiamondAmount() >= archerSO.defensiveIncreaseDiamondCost)
        {

            if (archer.GetCurrentDefensiveIncreasement() < deffensiveIncreasementLimit)
            {

                //PAY COST 
                ResourceManager.Instance.DecreaseCurrentWoodAmount(archerSO.defensiveIncreaseWoodCost);
                ResourceManager.Instance.DecreaseCurrentStoneAmount(archerSO.defensiveIncreaseStoneCost);
                ResourceManager.Instance.DecreaseCurrentDiamondAmount(archerSO.defensiveIncreaseDiamondCost);


                //HEALTH
                archer.SetMaxHealth(archer.GetMaxHealth() + archerSO.maxHealthIncreasementAmount);

                //ARMOR
                UiManager.Instance.OpenInfoPanel("MaxHealth", archer.GetMaxHealth(), archer.GetMaxHealth() + archerSO.maxHealthIncreasementAmount, UiManager.Instance.maxHealthColor);
                UiManager.Instance.OpenInfoPanel("Armor", archer.GetArmor(), archer.GetArmor() + archerSO.armorIncreasementAmount, UiManager.Instance.armorColor);
                archer.SetArmor(archer.GetArmor() + archerSO.armorIncreasementAmount);
                archer.SetCurrentDefensiveIncreasement(archer.GetCurrentDefensiveIncreasement() + 1);
                archer.upgradeParticle.Play();
                archer.LevelUp();
                UiManager.Instance.UpdateLevelText(archer, archer.GetCurrentLevel());
                UiManager.Instance.UpdateDisplayedAttributes(archer);
                UiManager.Instance.UpdateResourcesTexts();

            }
            else
            {
                if (!UiManager.Instance.IsUpgradeErrorPanelActive())
                {
                    UiManager.Instance.OpenUpgradeErrorPanel();
                }
            }
        }
        else
        {
            UiManager.Instance.OpenInsufficientResourcePanel();
        }
    }

    public void IncreaseArcherUtilityStats()
    {

        Archer archer = UnitSelections.Instance.lastSelectedUnit.GetComponent<Archer>();

        // COST CONDITION
        if (ResourceManager.Instance.GetCurrentWoodAmount() >= archerSO.utilityIncreaseWoodCost &&
            ResourceManager.Instance.GetCurrentStoneAmount() >= archerSO.utilityIncreaseStoneCost &&
            ResourceManager.Instance.GetCurrentDiamondAmount() >= archerSO.utilityIncreaseDiamondCost)
        {

            if (archer.GetCurrentUtilityIncreasement() < utilityIncreasementLimit)
            {
                //PAY COST 
                ResourceManager.Instance.DecreaseCurrentWoodAmount(archerSO.utilityIncreaseWoodCost);
                ResourceManager.Instance.DecreaseCurrentStoneAmount(archerSO.utilityIncreaseStoneCost);
                ResourceManager.Instance.DecreaseCurrentDiamondAmount(archerSO.utilityIncreaseDiamondCost);



                //SPEED
                UiManager.Instance.OpenInfoPanel("Speed", archer.GetSpeed(), archer.GetSpeed() + archerSO.speedIncreasementAmount, UiManager.Instance.speedColor);
                UiManager.Instance.OpenInfoPanel("Health Regen", archer.GetHealthRegenPerFiveSecond(), archer.GetHealthRegenPerFiveSecond() + archerSO.healthRegenIncreasementAmount, UiManager.Instance.healthRegenColor);
                archer.SetSpeed(archer.GetSpeed() + archerSO.speedIncreasementAmount);
                archer.SetCurrentUtilityIncreasement(archer.GetCurrentUtilityIncreasement() + 1);
                archer.upgradeParticle.Play();
                archer.LevelUp();

                //HEALTH REGEN
                archer.SetHealthRegenPerFiveSecond(archer.GetHealthRegenPerFiveSecond() + archerSO.healthRegenIncreasementAmount);

                UiManager.Instance.UpdateLevelText(archer, archer.GetCurrentLevel());
                UiManager.Instance.UpdateDisplayedAttributes(archer);
                UiManager.Instance.UpdateResourcesTexts();
            }
            else
            {
                if (!UiManager.Instance.IsUpgradeErrorPanelActive())
                {
                    UiManager.Instance.OpenUpgradeErrorPanel();
                }
            }
        }
        else
        {
            UiManager.Instance.OpenInsufficientResourcePanel();
        }
    }

    #endregion


    // bloodthirst



    /*
    public void IncreasePaladinsSpeed() // örnek
    {
        if (GenerationManager.Instance.IsAnyGenerationSlotEmpty())
        {
            paladinSO.speed += paladinSpeedIncreasement;

            Paladin[] paladins = FindObjectsOfType<Paladin>();
            for (int i = 0; i < paladins.Length; i++)
            {
                paladins[i].SetSpeed(paladinSO.speed);
            }
            // bunu kullandýktan sonra deaktif etmem lazým
        }
    }

    */
}
