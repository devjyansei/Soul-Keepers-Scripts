using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UiManager : MonoSingleton<UiManager>
{
    //public TextMeshProUGUI sampleText;
    public Color damageColor;
    public Color armorColor;
    public Color maxHealthColor;
    public Color healthRegenColor;
    public Color speedColor;


    public Color woodColor;
    public Color stoneColor;
    public Color diamondColor;
    public Color soulColor;

    public GameObject controlPanel;
    public GameObject resourcesPanel;
    public GameObject settingsPanel;
    public GameObject minimapPanel;
    public GameObject populationPanel;
    public GameObject populationErrorPanel;
    public GameObject cancelBuildingInfoPanel;
    public GameObject buildingDistanceErrorInfoPanel;
    public GameObject buildingUnitErrorInfoPanel;
    public GameObject waveCountdownPanel;
    public GameObject soulPanel;
    public GameObject upgradeErrorPanel;
    public GameObject tutorialPanel;
    public GameObject insufficientResourcePanel;
    public GameObject generationPanel;
    public GameObject corePanel;

    public GameObject generalCanvas;
    public GameObject infoCanvas;
    public GameObject soulShopCanvas;
    public GameObject minimapCanvas;
    public GameObject generalInfoCanvas;
    public GameObject gameOverCanvas;
    public GameObject victoryCanvas;


    public CanvasGroup gameOverCanvasGroup;

    public TextMeshProUGUI[] generalInfoTexts;
    public GameObject[] generalInfoTextPanels;


    public GameObject imageHolder;
    public GameObject generalInfoHolder;
    public GameObject attributesHolder;

    public TextMeshProUGUI countdownText;
    public GameObject waveClearedPanel;
    public GameObject waveIsComingPanel;


    public GameObject rewardPanel;
    public TextMeshProUGUI woodRewardText;
    public TextMeshProUGUI stoneRewardText;
    public TextMeshProUGUI diamondRewardText;

    public TextMeshProUGUI woodText; 
    public TextMeshProUGUI stoneText; 
    public TextMeshProUGUI diamondText;
    public TextMeshProUGUI soulText;

    public TextMeshProUGUI populationText;

    public GameObject hearthPanel;


    
    private void Start()
    {
        CloseControlPanel();
        OpenSoulPanel();
        ToggleSoulPanel();
        OpenTutorialPanel();

    }
    private void OnEnable()
    {
        EventManager.OnAnyBuildingSelected += OpenBuildingControlPanel;
        EventManager.OnAnyBuildingSelected += SetBuildingsImage;
        EventManager.OnAnyBuildingSelected += SetBuildingButtonEventSettings;

        EventManager.OnAnyUnitSelected += OpenUnitControlPanel;
        EventManager.OnAnyUnitSelected += SetUnitsImage;
        EventManager.OnAnyUnitSelected += SetUnitButtonEventSettings;

        EventManager.OnAnyUnitSelected += SetUnitsDisplayedAttributes;


        EventManager.OnResourcesUpdated += UpdateResourcesTexts;


    }
    private void OnDisable()
    {
        EventManager.OnAnyBuildingSelected -= OpenBuildingControlPanel;
        EventManager.OnAnyBuildingSelected -= SetBuildingsImage;
        EventManager.OnAnyBuildingSelected -= SetBuildingButtonEventSettings;

        EventManager.OnAnyUnitSelected -= OpenUnitControlPanel;
        EventManager.OnAnyUnitSelected -= SetUnitsImage;
        EventManager.OnAnyUnitSelected -= SetUnitButtonEventSettings;

        EventManager.OnAnyUnitSelected -= SetUnitsDisplayedAttributes;


        EventManager.OnResourcesUpdated -= UpdateResourcesTexts;

    }

    //control panel
    public void OpenBuildingControlPanel(Building building)
    {
        controlPanel.SetActive(true);
    }
    public void OpenUnitControlPanel(Unit unit)
    {
        controlPanel.SetActive(true);
    }


    public void OpenTutorialPanel()
    {
        tutorialPanel.SetActive(true);
        CloseControlPanel();
        CloseMinimapCAnvas();
        generalCanvas.SetActive(false);
        minimapCanvas.SetActive(false);
        soulShopCanvas.SetActive(false);
        CloseInfoCanvas();
        GameManager.Instance.PauseGame();
    }
    public void CloseTutorialPanel()
    {
        tutorialPanel.SetActive(false);
        generalCanvas.gameObject.SetActive(true);
        minimapCanvas.SetActive(true);
        soulShopCanvas.SetActive(true);
        OpenInfoCanvas();
        GameManager.Instance.ResumeGame();
    }
    public void PlayHearthAnimation()
    {
        if (!hearthPanel.activeSelf)
        {
            hearthPanel.SetActive(true);
            hearthPanel.transform.DOScale(Vector3.one * 10f, 2).OnComplete(() =>
            {
                ResetHearthScale();
            });
        }
        
    }
    private void ResetHearthScale()
    {
        hearthPanel.transform.DOScale(Vector3.zero, 1).OnComplete(() =>
        {
            
            hearthPanel.SetActive(false);

        });
    }
    public void OpenVictoryCanvas()
    {
        victoryCanvas.SetActive(true);
    }
    public void CloseVictoryCanvas()
    {
        victoryCanvas.SetActive(false);
    }
    public void OpenInfoCanvas()
    {
        generalInfoCanvas.SetActive(true);
    }
    public void CloseInfoCanvas()
    {
        generalInfoCanvas.SetActive(false);
    }
    public void OpenGameOverCanvas()
    {
        CloseGeneralCanvas();
        CloseControlPanel();
        CloseMinimapCAnvas();
        ClosePopulationErrorPanel();
        CloseSoulPanel();
        CloseWaveIsComingText();
        CloseCancelBuildingInfoPanel();
        CloseBuildingDistanceErrorPanel();
        
        gameOverCanvasGroup.alpha = 0f;
        gameOverCanvas.SetActive(true);
        gameOverCanvasGroup.DOFade(1f, 2f);
    }
    public void CloseGameOverCanvas()
    {
        
        gameOverCanvas.SetActive(false);
        OpenMinimapCanvas();
    }
    public void OpenCorePanel()
    {
        corePanel.SetActive(true);
    }
    public void CloseCorePanel()
    {
        corePanel.SetActive(false);
    }


    public void CloseGeneralCanvas()
    {
        generalCanvas.SetActive(false);
    }
    public void OpenGeneralCanvas()
    {
        generalCanvas.SetActive(true);
    }
    public void CloseControlPanel()
    {
        if (GameManager.Instance.GetIsGameStarted())
        {
            controlPanel.SetActive(false);
        }
        
    }
    public void OpenSoulShop()
    {
        soulShopCanvas.SetActive(true);
    }
    public void CloseSoulShop()
    {
        soulShopCanvas.SetActive(false);
    }
    public void OpenUpgradeErrorPanel()
    {
        AudioManager.Instance.PlayErrorSound();

        upgradeErrorPanel.SetActive(true);
        Invoke("CloseUpgradeErrorPanel", 2f);
        
        
    }
    public bool IsUpgradeErrorPanelActive()
    {
        return upgradeErrorPanel.activeInHierarchy;
    }
    public void CloseUpgradeErrorPanel()
    {
        upgradeErrorPanel.SetActive(false);
    }
    public void OpenInsufficientResourcePanel()
    {
        insufficientResourcePanel.SetActive(true);
        AudioManager.Instance.PlayErrorSound();
        if (insufficientResourcePanel.activeInHierarchy)
        {
            Invoke("CloseInsufficientResourcePanel", 2f);
        }
        
        CloseBuildingInUnitErrorInfoPanel();
        CloseCancelBuildingInfoPanel();
        CloseBuildingDistanceErrorPanel();
    }
    public void CloseInsufficientResourcePanel()
    {
        insufficientResourcePanel.SetActive(false);
    }
    public bool IsInsufficientResourcePanelOpened()
    {
        return insufficientResourcePanel.activeSelf;
    }
    public void UpdateCountdownText(float remainingTime)
    {
        countdownText.text = "Next Wave : " + Mathf.FloorToInt(remainingTime).ToString();

        if (remainingTime <= 10)
        {
            DOTween.To(() => countdownText.fontSize, x => countdownText.fontSize = x, 65, .25f).OnComplete(()=> { DOTween.To(() => countdownText.fontSize, x => countdownText.fontSize = x, 55, .25f); });
            
        }
    }



    public void ToggleSettingsPanel()
    {
        if (soulPanel.activeInHierarchy == false)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
            if (settingsPanel.activeSelf)
            {
                CloseCorePanel();
                CloseMinimapCAnvas();
                CloseSoulPanel();
                CloseResourcesPanel();
                CloseInfoCanvas();
                CloseControlPanel();
                soulShopCanvas.SetActive(false);
                GameManager.Instance.SwitchGameState(GameManager.InGameStates.paused);

            }
            else
            {
                OpenCorePanel();
                OpenMinimapCanvas();
                OpenResourcesPanel();
                OpenInfoCanvas();

                if (UnitSelections.Instance.selectedUnit != null)
                {
                    OpenUnitControlPanel(UnitSelections.Instance.lastSelectedUnit);
                }
                if (BuildingManager.Instance.selectedStructurePrefab != null)
                {
                    OpenBuildingControlPanel(BuildingManager.Instance.lastSelectedStructurePrefab.GetComponent<Building>());
                }
                soulShopCanvas.SetActive(true);
                GameManager.Instance.SwitchGameState(GameManager.InGameStates.flow);
            }
        }
        
    }

    public void ToggleSoulPanel()
    {
        soulPanel.SetActive(!soulPanel.activeSelf);

        if (soulPanel.activeSelf)
        {
            CameraController.Instance.DisablePan();
            CloseControlPanel();
            generationPanel.transform.localScale = Vector3.zero;

            CloseCorePanel();
            CloseResourcesPanel();
            CloseInfoCanvas();
            CloseMinimapCAnvas();

            UpdateResourcesTexts();
            UnitSelections.Instance.ClearSelectedUnit();
            AudioManager.Instance.PlaySoulShopSound();
            GameManager.Instance.SwitchGameState(GameManager.InGameStates.paused);

        }
        else
        {
            CameraController.Instance.EnablePan();
            generationPanel.transform.localScale = Vector3.one;

            
            OpenCorePanel();
            OpenResourcesPanel();
            OpenInfoCanvas();
            OpenMinimapCanvas();
            AudioManager.Instance.PlaySoulShopSound();

            AudioManager.Instance.PlayInGameBackgroundSound();
            GameManager.Instance.SwitchGameState(GameManager.InGameStates.flow);
        }
    }
    public void OpenResourcesPanel()
    {
        resourcesPanel.SetActive(true);
    }
    public void CloseResourcesPanel()
    {
        resourcesPanel.SetActive(false);
    }
    public void CloseSoulPanel()
    {
        if (GameManager.Instance.GetIsGameStarted())
        {
            soulPanel.SetActive(false);
        }
        

    }
    public void OpenSoulPanel()
    {
        soulPanel.SetActive(true);

    }

    // -------------------------- INFO TEXTS ------------------------------- //

    public void OpenWaveIsComingText()
    {
        waveIsComingPanel.transform.localScale = Vector3.zero;
        waveIsComingPanel.SetActive(true);
        waveIsComingPanel.transform.DOScale(Vector3.one,.5f);
        Invoke("CloseWaveIsComingText", 2.5f);
    }
    public void CloseWaveIsComingText()
    {
        waveIsComingPanel.transform.DOScale(Vector3.zero, .4f).OnComplete(()=> waveIsComingPanel.SetActive(false));    
    }
    private void Update()
    {
        
        
        /*
        if (Input.GetKeyDown(KeyCode.D))
        {
            OpenWaveIsComingText();
        }
        */
    }
    public void OpenWaveClearedText()
    {
        
        waveClearedPanel.transform.localScale = Vector3.zero;
        waveClearedPanel.gameObject.SetActive(true);
        waveClearedPanel.transform.DOScale(Vector3.one, .5f);
        Invoke("CloseWaveClearedText", 3f);

    }
    public void CloseWaveClearedText()
    {
        waveClearedPanel.transform.DOScale(Vector3.zero, .2f).OnComplete(() => waveClearedPanel.gameObject.SetActive(false));
    }

    
    public void OpenRewardPanel(float woodReward,float stoneReward,float diamondReward) // popup
    {
        

        //reset
        rewardPanel.SetActive(false);
        rewardPanel.transform.localScale = Vector3.zero;

        //open
        rewardPanel.transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), .3f).OnComplete(() =>
          { rewardPanel.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), .15f); });

        rewardPanel.SetActive(true);

        woodRewardText.text = "+" + woodReward;
        stoneRewardText.text = "+" + stoneReward;
        diamondRewardText.text = "+" + diamondReward;
        Invoke("CloseRewardPanel", 3f);

        
    }
    public void CloseRewardPanel()
    {
        rewardPanel.transform.DOScale(Vector3.zero, .5f).OnComplete(() => { rewardPanel.SetActive(false); });
    }

    public void OpenMinimapCanvas()
    {
        minimapCanvas.SetActive(true);
    }
    public void CloseMinimapCAnvas()
    {
        minimapCanvas.SetActive(false);
    }
    
    public void OpenBuildingDistanceErrorPanel()
    {
        buildingDistanceErrorInfoPanel.SetActive(true);
        

        if (buildingUnitErrorInfoPanel.activeSelf)
        {
            CloseBuildingInUnitErrorInfoPanel();
            Debug.Log("1");
        }        

        if (IsInsufficientResourcePanelOpened())
        {
            Debug.Log("2");

            CloseInsufficientResourcePanel();
        }
        
    }
    public void CloseBuildingDistanceErrorPanel()
    {
        buildingDistanceErrorInfoPanel.SetActive(false);
    }
    public void OpenCancelBuildingInfoPanel()
    {
        cancelBuildingInfoPanel.SetActive(true);

    }

    public void CloseCancelBuildingInfoPanel()
    {
        cancelBuildingInfoPanel.SetActive(false);
    }
    public void OpenBuildingInUnitErrorInfoPanel()
    {
        buildingUnitErrorInfoPanel.SetActive(true);
        AudioManager.Instance.PlayErrorSound();

        if (buildingUnitErrorInfoPanel.activeInHierarchy)
        {
            Invoke("CloseUnitErrorInfoPanel", 2f);
        }
        
        CloseInsufficientResourcePanel();
        Debug.Log("2");
        
    }

    public void CloseBuildingInUnitErrorInfoPanel()
    {
        buildingUnitErrorInfoPanel.SetActive(false);
    }
    public void OpenPopulationErrorPanel()
    {
        AudioManager.Instance.PlayErrorSound();

        if (!populationErrorPanel.activeSelf)
        {

            populationErrorPanel.SetActive(true);
            Invoke("ClosePopulationErrorPanel", 3f);
        }
        
    }
    public void ClosePopulationErrorPanel()
    {
        populationErrorPanel.SetActive(false);
    }


    #region PERSONAL UPGRADE TEXT INFO 
    //------------------  upgrade info texts  ----------------------
    public void OpenInfoPanel(string statName, int point,int point2, Color color)
    {
        StartCoroutine(InfoPanel(statName,point,point2,color));
    }
    public void OpenInfoPanel(string statName, float point,float point2, Color color)
    {
        StartCoroutine(InfoPanel(statName, point,point2, color));
    }

    public void OpenRewardInfoPanel(string rewardName, float amount, Color color)
    {
        StartCoroutine(RewardInfoPanel(rewardName, amount, color));
    }
    public IEnumerator InfoPanel(string statName,int point, int point2,Color color)
    {
        for (int i = 0; i < generalInfoTexts.Length; i++)
        {
            if (generalInfoTexts[i].gameObject.activeInHierarchy == false)
            {
                
                string coloredStatName = ChangeStringsColor(statName, color);
                string coloredPointName = ChangeStringsColor(point.ToString(), color);
                string coloredPointName2 = ChangeStringsColor(point2.ToString(), color);

                generalInfoTexts[i].text = coloredStatName + " increased " + coloredPointName + " to " + coloredPointName2;

                generalInfoTextPanels[i].SetActive(true);
                generalInfoTexts[i].gameObject.SetActive(true);
                yield return new WaitForSeconds(6f);
                generalInfoTextPanels[i].SetActive(false);
                generalInfoTexts[i].gameObject.SetActive(false);                
                break;
            }
            
            
        }
    }

    public IEnumerator InfoPanel(string statName, float point, float point2, Color color)
    {
        for (int i = 0; i < generalInfoTexts.Length; i++)
        {
            if (generalInfoTexts[i].gameObject.activeSelf == false)
            {
                string coloredStatName = ChangeStringsColor(statName, color);
                string coloredPointName = ChangeStringsColor(point.ToString(), color);
                string coloredPointName2 = ChangeStringsColor(point2.ToString(), color);

                generalInfoTexts[i].text = coloredStatName + " increased " + coloredPointName + " to " + coloredPointName2;
                generalInfoTextPanels[i].SetActive(true);
                generalInfoTexts[i].gameObject.SetActive(true);
                yield return new WaitForSeconds(6f);
                generalInfoTextPanels[i].SetActive(false);
                generalInfoTexts[i].gameObject.SetActive(false);
                break;
            }

        }
    }

    public IEnumerator RewardInfoPanel(string rewardName, float amount, Color color)
    {
        for (int i = 0; i < generalInfoTexts.Length; i++)
        {
            if (generalInfoTexts[i].gameObject.activeSelf == false)
            {
                string coloredRewardName = ChangeStringsColor(rewardName, color);

                generalInfoTexts[i].text = "Gained " + amount.ToString("0.") + " " + coloredRewardName;
                generalInfoTextPanels[i].SetActive(true);
                generalInfoTexts[i].gameObject.SetActive(true);
                yield return new WaitForSeconds(6f);
                generalInfoTextPanels[i].SetActive(false);
                generalInfoTexts[i].gameObject.SetActive(false);
                break;
            }

        }
    }

    #endregion


    #region ACADEMY TEXT INFO METHODS
    // ---------- Academy UPGRADES --------------

    public void OpenAcademyUpgradeInfoPanel(string statName, int point, Color color)
    {
        StartCoroutine(AcademyUpgradeInfoPanel(statName, point,color));
    }
    public void OpenAcademyUpgradeInfoPanel(string statName, float point, Color color)
    {
        StartCoroutine(AcademyUpgradeInfoPanel(statName, point,color));
    }

    public IEnumerator AcademyUpgradeInfoPanel(string statName, float point, Color color)
    {
        for (int i = 0; i < generalInfoTexts.Length; i++)
        {
            if (generalInfoTexts[i].gameObject.activeSelf == false)
            {
                string coloredStatName = ChangeStringsColor(statName, color);
                string coloredPointName = ChangeStringsColor(point.ToString(), color);
                generalInfoTexts[i].text = "All units " + coloredStatName  + " is increased by " + coloredPointName;
                generalInfoTextPanels[i].SetActive(true);
                generalInfoTexts[i].gameObject.SetActive(true);
                yield return new WaitForSeconds(6f);
                generalInfoTextPanels[i].SetActive(false);
                generalInfoTexts[i].gameObject.SetActive(false);
                break;
            }

        }
    }
    public IEnumerator AcademyUpgradeInfoPanel(string statName, int point, Color color)
    {
        for (int i = 0; i < generalInfoTexts.Length; i++)
        {
            if (generalInfoTexts[i].gameObject.activeSelf == false)
            {
                string coloredStatName = ChangeStringsColor(statName, color);
                string coloredPointName = ChangeStringsColor(point.ToString(), color);
                generalInfoTexts[i].text = "All units " + coloredStatName + " is increased by " + coloredPointName;
                generalInfoTextPanels[i].SetActive(true);
                generalInfoTexts[i].gameObject.SetActive(true);
                yield return new WaitForSeconds(6f);
                generalInfoTextPanels[i].SetActive(false);
                generalInfoTexts[i].gameObject.SetActive(false);
                break;
            }

        }
    }

    #endregion





    #region UNIT SETTINGS
    public void SetUnitsImage(Unit unit) //general image
    {
        imageHolder.GetComponent<Image>().sprite = unit.unitDataSO.image;
    } 

    public void SetUnitsDisplayedAttributes(Unit unit) // general info
    {
       
        Unit selectedUnit = unit.GetComponent<Unit>();

        // WORKERS
        if (selectedUnit.GetComponent<Worker>() != null)
        {
            // ------ Unit Health -------
            generalInfoHolder.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Health : " + selectedUnit.GetCurrentHealth().ToString() + " / " + selectedUnit.GetMaxHealth().ToString();

            // ------- Unit Productivity ------- 
            generalInfoHolder.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "Productivity : " + selectedUnit.GetComponent<Worker>().productivity.ToString();

            // ------- Unit Armor --------
            generalInfoHolder.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = "Armor : " + selectedUnit.GetArmor().ToString();
        }

        //WARRIORS
        if (selectedUnit.GetComponent<Worker>() == null)
        {
            // ------ Unit Health -------
            generalInfoHolder.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Health : " + selectedUnit.GetCurrentHealth().ToString() + " / " + selectedUnit.GetMaxHealth().ToString();

            // ------- Unit Damage ------- 
            generalInfoHolder.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "Damage : " + selectedUnit.GetDamage().ToString();

            // ------- Unit Armor --------
            generalInfoHolder.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = "Armor : " + selectedUnit.GetArmor().ToString();
        }

        
    }
    public void UpdateDisplayedAttributes(Unit unit)
    {
        Unit selectedUnit = unit.GetComponent<Unit>();

        //WORKER
        if (selectedUnit.GetComponent<Worker>() != null)
        {
            generalInfoHolder.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Health : " + selectedUnit.GetCurrentHealth().ToString() + " / " + selectedUnit.GetMaxHealth().ToString();
            generalInfoHolder.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "Productivity : " + selectedUnit.GetComponent<Worker>().productivity;
            generalInfoHolder.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = "Armor : " + selectedUnit.GetArmor().ToString();
        }

        //WARRIOR
        if (selectedUnit.GetComponent<Worker>() == null)
        {
            generalInfoHolder.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Health : " + selectedUnit.GetCurrentHealth().ToString() + " / " + selectedUnit.GetMaxHealth().ToString();
            generalInfoHolder.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "Damage : " + selectedUnit.GetDamage().ToString();
            generalInfoHolder.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = "Armor : " + selectedUnit.GetArmor().ToString();
        }
        
    }


    

   
    public void UpdateDisplayedAttributes(Building building)
    {
        if (building.GetComponent<House>() != null){return;}

        SetBuildingsDisplayedGeneralInfo(building);
        SetBuildingButtonEventSettings(building);
    }
    public void SetUnitButtonEventSettings(Unit unit) // click events image and functionality and tooltip
    {
        ClearAllButtonEventSetting();

        
        // Color Settings
        string coloredWoodCost = ChangeStringsColor("WOOD COST :", woodColor);
        string coloredStoneCost = ChangeStringsColor("STONE COST :", stoneColor);
        string coloredDiamondCost = ChangeStringsColor("DIAMOND COST :", diamondColor);



        int listOrder = 0;

        for (int i = 0; i < attributesHolder.transform.childCount; i++)
        {
            for (int j = 0; j < attributesHolder.transform.GetChild(i).childCount; j++)
            {

                attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = true;
                attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().sprite = unit.unitDataSO.attributesImageList[listOrder];
                attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Button>().onClick = unit.unitDataSO.attributesClickEventList[listOrder];
               // unit.GetComponentInChildren<HealthBar>().UpdateHealthbar(unit.GetMaxHealth(),unit.GetCurrentHealth());
                
                //----------------TOOLTIP MESSAGES -----------------//



                // WARRIOR

                #region Warriors tooltip settings
                if (unit.GetComponent<Worker>() == null)
                {
                    // First Line
                    if (i == 0)
                    {
                        if (j == 0) //attribute 1
                        {
                            string coloredDamageName = ChangeStringsColor("Damage", damageColor);
                            string coloredDamageAmount = ChangeStringsColor(unit.unitDataSO.damageIncreasementAmount.ToString(), damageColor);

                            string tooltipMessage = attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Tooltip>().message = "Increases " + coloredDamageName + " by " + coloredDamageAmount +
                            "\n" +
                            "\n" + coloredWoodCost + " " + unit.unitDataSO.damageIncreaseWoodCost +
                            "\n" + coloredStoneCost + " " + unit.unitDataSO.damageIncreaseStoneCost +
                            "\n" + coloredDiamondCost + " " + unit.unitDataSO.damageIncreaseDiamondCost;

                            //Debug.Log(sampleText.textInfo.lineCount);
                        }
                        if (j == 1)//attribute 2
                        {
                            //TooltipManager._instance.tooltipText.textInfo.lineCount++;

                            string coloredArmorName = ChangeStringsColor("Armor", armorColor);
                            string coloredArmorAmount = ChangeStringsColor(unit.unitDataSO.armorIncreasementAmount.ToString(), armorColor);

                            string coloredMaxHealthName = ChangeStringsColor("Max Health", maxHealthColor);
                            string coloredMaxHealthAmount = ChangeStringsColor(unit.unitDataSO.maxHealthIncreasementAmount.ToString(), maxHealthColor);

                            string tooltipMessage = attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Tooltip>().message = "Increases " + coloredArmorName + " by " + coloredArmorAmount +
                            "\n" + "Increases " + coloredMaxHealthName + " by " + coloredMaxHealthAmount +
                            "\n" +
                            "\n" + coloredWoodCost + " " + unit.unitDataSO.defensiveIncreaseWoodCost +
                            "\n" + coloredStoneCost + " " + unit.unitDataSO.defensiveIncreaseStoneCost +
                            "\n" + coloredDiamondCost + " " + unit.unitDataSO.defensiveIncreaseDiamondCost;

                            //Debug.Log(sampleText.textInfo.lineCount);
                        }
                        if (j == 2) //attribute 3
                        {
                            //TooltipManager._instance.tooltipText.textInfo.lineCount++;

                            string coloredHealthRegenName = ChangeStringsColor("Health Regen", healthRegenColor);
                            string coloredHealthRegenAmount = ChangeStringsColor(unit.unitDataSO.healthRegenIncreasementAmount.ToString(), healthRegenColor);

                            string coloredSpeedName = ChangeStringsColor("Speed", speedColor);
                            string coloredSpeedAmount = ChangeStringsColor(unit.unitDataSO.speedIncreasementAmount.ToString(), speedColor);

                            string tooltipMessage = attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Tooltip>().message = "Increases " + coloredHealthRegenName + " by " + coloredHealthRegenAmount +
                            "\n" + "Increases " + coloredSpeedName + " by " + coloredSpeedAmount +
                            "\n" +
                            "\n" + coloredWoodCost + " " + unit.unitDataSO.utilityIncreaseWoodCost +
                            "\n" + coloredStoneCost + " " + unit.unitDataSO.utilityIncreaseStoneCost +
                            "\n" + coloredDiamondCost + " " + unit.unitDataSO.utilityIncreaseDiamondCost; ;


                        }
                    }
                    if (i == 1)
                    {
                        
                        attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                    }
                    if (i == 2)
                    {

                        attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                    }
                    //second line buraya gelecek

                }
                #endregion


                // WORKER
                #region Workers tooltip settings
                if (unit.GetComponent<Worker>() != null)
                {

                    // First Line
                    if (i == 0)
                    {
                        if (j == 0) //house
                        {   
                            string tooltipMessage = attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Tooltip>().message = unit.unitDataSO.attributesNameList[j].ToString() +
                            "\n" +
                            unit.unitDataSO.attributesDescriptionList[j].ToString() +
                            "\n" +
                            "\n" + coloredWoodCost + " " + BuildingManager.Instance.houseDataSO.woodCost.ToString() +
                            "\n" + coloredStoneCost + " " + BuildingManager.Instance.houseDataSO.stoneCost.ToString() +
                            "\n" + coloredDiamondCost + " " + BuildingManager.Instance.houseDataSO.diamondCost.ToString();

                            
                        }

                        if (j == 1) //archer tower
                        {

                            string tooltipMessage = attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Tooltip>().message = unit.unitDataSO.attributesNameList[j].ToString() +
                             "\n" +
                             unit.unitDataSO.attributesDescriptionList[j].ToString() +
                            "\n" +
                             "\n" + coloredWoodCost + " " + BuildingManager.Instance.archerTowerSO.woodCost.ToString() +
                             "\n" + coloredStoneCost + " " + BuildingManager.Instance.archerTowerSO.stoneCost.ToString() +
                             "\n" + coloredDiamondCost + " " + BuildingManager.Instance.archerTowerSO.diamondCost.ToString();
                        }
                        if (j == 2) //academy
                        {

                            string tooltipMessage = attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Tooltip>().message = unit.unitDataSO.attributesNameList[j].ToString() +
                             "\n" +
                             unit.unitDataSO.attributesDescriptionList[j].ToString() +
                            "\n" +
                             "\n" + coloredWoodCost + " " + BuildingManager.Instance.academySO.woodCost.ToString() +
                             "\n" + coloredStoneCost + " " + BuildingManager.Instance.academySO.stoneCost.ToString() +
                             "\n" + coloredDiamondCost + " " + BuildingManager.Instance.academySO.diamondCost.ToString();
                        }
                    }
                    //Second Line
                    if (i == 1)
                    {
                        if (j == 0) //wood generator
                        {
                            string tooltipMessage = attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Tooltip>().message = unit.unitDataSO.attributesNameList[3 + j].ToString() +
                            "\n" +
                            unit.unitDataSO.attributesDescriptionList[3 + j].ToString() +
                            "\n" +
                            "\n" + coloredWoodCost + " " + BuildingManager.Instance.woodGeneratorSO.woodCost.ToString() +
                            "\n" + coloredStoneCost + " " + BuildingManager.Instance.woodGeneratorSO.stoneCost.ToString() +
                            "\n" + coloredDiamondCost + " " + BuildingManager.Instance.woodGeneratorSO.diamondCost.ToString();
                        }
                        if (j == 1)//stone gnerator
                        {
                            string tooltipMessage = attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Tooltip>().message = unit.unitDataSO.attributesNameList[3 + j].ToString() +
                             "\n" +
                             unit.unitDataSO.attributesDescriptionList[3 + j].ToString() +
                            "\n" +
                             "\n" + coloredWoodCost + " " + BuildingManager.Instance.stoneGeneratorSO.woodCost.ToString() +
                             "\n" + coloredStoneCost + " " + BuildingManager.Instance.stoneGeneratorSO.stoneCost.ToString() +
                             "\n" + coloredDiamondCost + " " + BuildingManager.Instance.stoneGeneratorSO.diamondCost.ToString();
                        }
                        if (j == 2) // empty
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                    }
                    // Third Line
                    if (i == 2)
                    {
                        if (j == 0) // empty
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                        if (j == 1) // empty
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                        if (j == 2) // empty
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                    }

                }
                #endregion
                
               


                listOrder++;
            }
        }
    }
    #endregion



   
    #region Buildings

    public void SetBuildingsImage(Building building)
    {
        if (building.GetComponent<House>() != null) { return; }
        imageHolder.GetComponent<Image>().sprite = building.structureDataSO.sprite;
    }
    public void SetBuildingsDisplayedGeneralInfo(Building building)
    {
        if (building.GetComponent<House>() != null) { return; }
        generalInfoHolder.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "";
        generalInfoHolder.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "";
        generalInfoHolder.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = "";
    }
    

    public void SetBuildingButtonEventSettings(Building building) // click events image and functionality
    {
        

        ClearAllButtonEventSetting();
        if (building.GetComponent<House>() != null) { return; }

        // Color Settings
        string coloredWood = ChangeStringsColor("WOOD COST :", woodColor);
        string coloredStone = ChangeStringsColor("STONE COST :", stoneColor);
        string coloredDiamond = ChangeStringsColor("DIAMOND COST :", diamondColor);

        int listOrder = 0;

        for (int i = 0; i < attributesHolder.transform.childCount; i++)
        {
            for (int j = 0; j < attributesHolder.transform.GetChild(i).childCount; j++)
            {
                
                attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = true;
                attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().sprite = building.structureDataSO.attributesImageList[listOrder];
                attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Button>().onClick = building.structureDataSO.attributesClickEventList[listOrder];

                // if first attributes line
                if (i == 0)
                {
                    if (j == 0)
                    {
                        if (building.structureDataSO.isAttributeUsed[j] == true)
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                        if (building.structureDataSO.isAnyAttributes[j] == false)
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                            
                        }

                        attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Tooltip>().message = building.structureDataSO.attributesNamesList[j] +
                        "\n" +
                        building.structureDataSO.attributesDescription[j]+
                        "\n" +
                        "\n" + coloredWood + " " + building.structureDataSO.attributesWoodCostsList[j] +
                        "\n" + coloredStone + " " + building.structureDataSO.attributesStoneCostsList[j] +
                        "\n" + coloredDiamond + " " + building.structureDataSO.attributesDiamondCostsList[j];
                    }
                    if (j == 1)
                    {
                        if (building.structureDataSO.isAttributeUsed[j] == true)
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                        if (building.structureDataSO.isAnyAttributes[j] == false)
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }

                        attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Tooltip>().message = building.structureDataSO.attributesNamesList[j] +
                        "\n" +
                        building.structureDataSO.attributesDescription[j] +
                        "\n" +
                        "\n" + coloredWood + " " + building.structureDataSO.attributesWoodCostsList[j] +
                        "\n" + coloredStone + " " + building.structureDataSO.attributesStoneCostsList[j] +
                        "\n" + coloredDiamond + " " + building.structureDataSO.attributesDiamondCostsList[j];
                    }
                    if (j == 2)
                    {
                        if (building.structureDataSO.isAttributeUsed[j] == true)
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                        if (building.structureDataSO.isAnyAttributes[j] == false)
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }

                        attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Tooltip>().message = building.structureDataSO.attributesNamesList[j] +
                        "\n" +
                        building.structureDataSO.attributesDescription[j] +
                        "\n" +
                        "\n" + coloredWood + " " + building.structureDataSO.attributesWoodCostsList[j] +
                        "\n" + coloredStone + " " + building.structureDataSO.attributesStoneCostsList[j] +
                        "\n" + coloredDiamond + " " + building.structureDataSO.attributesDiamondCostsList[j];
                    }
                }
                if (i==1)
                {
                    if (j == 0)
                    {
                        if (building.structureDataSO.isAttributeUsed[3+j] == true)
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                        if (building.structureDataSO.isAnyAttributes[3+j] == false)
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                        attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Tooltip>().message = building.structureDataSO.attributesNamesList[3 + j] +
                        "\n" +
                        building.structureDataSO.attributesDescription[3+j] +
                        "\n" +
                        "\n" + coloredWood + " " + building.structureDataSO.attributesWoodCostsList[3 + j] +
                        "\n" + coloredStone + " " + building.structureDataSO.attributesStoneCostsList[3 + j] +
                        "\n" + coloredDiamond + " " + building.structureDataSO.attributesDiamondCostsList[3 + j];
                    }
                    if (j == 1)
                    {
                        if (building.structureDataSO.isAttributeUsed[3+j] == true)
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                        if (building.structureDataSO.isAnyAttributes[3+j] == false)
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                        attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Tooltip>().message = building.structureDataSO.attributesNamesList[3 + j] +
                        "\n" +
                        building.structureDataSO.attributesDescription[3+j] +
                        "\n" +
                        "\n" + coloredWood + " " + building.structureDataSO.attributesWoodCostsList[3 + j] +
                        "\n" + coloredStone + " " + building.structureDataSO.attributesStoneCostsList[3 + j] +
                        "\n" + coloredDiamond + " " + building.structureDataSO.attributesDiamondCostsList[3 + j];
                    }
                    if (j == 2)
                    {
                        if (building.structureDataSO.isAttributeUsed[3+j] == true)
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                        if (building.structureDataSO.isAnyAttributes[3+j] == false)
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                        attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Tooltip>().message = building.structureDataSO.attributesNamesList[3 + j] +
                        "\n" +
                        building.structureDataSO.attributesDescription[3+j] +
                        "\n" +
                        "\n" + coloredWood + " " + building.structureDataSO.attributesWoodCostsList[3 + j] +
                        "\n" + coloredStone + " " + building.structureDataSO.attributesStoneCostsList[3 + j] +
                        "\n" + coloredDiamond + " " + building.structureDataSO.attributesDiamondCostsList[3 + j];
                    }
                }
                if (i == 2)
                {
                    if (j==0)
                    {
                        if (building.structureDataSO.isAttributeUsed[6+j] == true)
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                        if (building.structureDataSO.isAnyAttributes[6+j] == false)
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }


                        attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Tooltip>().message = building.structureDataSO.attributesNamesList[6+j] +
                        "\n" +
                        building.structureDataSO.attributesDescription[6+j] +
                        "\n" +
                        "\n" + coloredWood + " " + building.structureDataSO.attributesWoodCostsList[6+j] +
                        "\n" + coloredStone + " " + building.structureDataSO.attributesStoneCostsList[6+j] +
                        "\n" + coloredDiamond + " " + building.structureDataSO.attributesDiamondCostsList[6+j];
                    }
                    if (j == 1)
                    {
                        if (building.structureDataSO.isAttributeUsed[6+j] == true)
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                        if (building.structureDataSO.isAnyAttributes[6+j] == false)
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }


                        attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Tooltip>().message = building.structureDataSO.attributesNamesList[6 + j] +
                        "\n" +
                        building.structureDataSO.attributesDescription[6+j] +
                        "\n" +
                        "\n" + coloredWood + " " + building.structureDataSO.attributesWoodCostsList[6 + j] +
                        "\n" + coloredStone + " " + building.structureDataSO.attributesStoneCostsList[6 + j] +
                        "\n" + coloredDiamond + " " + building.structureDataSO.attributesDiamondCostsList[6 + j];
                    }
                    if (j == 2)
                    {
                        if (building.structureDataSO.isAttributeUsed[6+j] == true)
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                        if (building.structureDataSO.isAnyAttributes[6+j] == false)
                        {
                            attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        }


                        attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Tooltip>().message = building.structureDataSO.attributesNamesList[6 + j] +
                        "\n" +
                        building.structureDataSO.attributesDescription[6+j] +
                        "\n" +
                        "\n" + coloredWood + " " + building.structureDataSO.attributesWoodCostsList[6 + j] +
                        "\n" + coloredStone + " " + building.structureDataSO.attributesStoneCostsList[6 + j] +
                        "\n" + coloredDiamond + " " + building.structureDataSO.attributesDiamondCostsList[6 + j];
                    }
                }
                listOrder++;
            }
        }
    }
    #endregion


    #region PUBLIC METHODS
    public void UpdateLevelText(Unit unit, int level)
    {

        unit.levelText.text = "Lv " + level;
    }
    private string ChangeStringsColor(string text, Color color)
    {
        return "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + text + "</color>";
    }

    public void ClearAllButtonEventSetting()
    {
        for (int i = 0; i < attributesHolder.transform.childCount; i++)
        {
            for (int j = 0; j < attributesHolder.transform.GetChild(i).childCount; j++)
            {
                attributesHolder.transform.GetChild(i).GetChild(j).GetComponent<Button>().onClick = ClickEventsManager.Instance.defaultAttribute;
            }
        }

    }

    // ----------------- Population   ---------------------

    public void UpdatePopulationText()
    {
        populationText.text = PopulationManager.Instance.GetCurrentPopulation().ToString() + " / " + PopulationManager.Instance.GetPopulationLimit().ToString();
    }


    // ----------------- Resources   ---------------------
    public void UpdateResourcesTexts()
    {
        woodText.text = ResourceManager.Instance.GetCurrentWoodAmount().ToString("0.");
        stoneText.text = ResourceManager.Instance.GetCurrentStoneAmount().ToString("0.");
        diamondText.text = ResourceManager.Instance.GetCurrentDiamondAmount().ToString("0.");
        soulText.text = ResourceManager.Instance.GetCurrentSoulAmount().ToString("0.");
    }

    #endregion
}
