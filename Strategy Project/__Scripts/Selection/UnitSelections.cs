using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class UnitSelections : MonoSingleton<UnitSelections>
{
    [SerializeField] private TextMeshProUGUI groupSelectText;
    [SerializeField] private Button groupSelectButton;

    public List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> unitsSelectedList = new List<GameObject>();

    public GameObject selectedUnit;
    public Unit lastSelectedUnit;

    public List<Unit> selectedGroup = new List<Unit>();

    
    public void SetSelectedUnit(GameObject clickedUnit)
    {
        lastSelectedUnit = clickedUnit.GetComponent<Unit>();
        selectedUnit = clickedUnit;
        Unit selectedUnitObj = selectedUnit.GetComponent<Unit>();
        
        selectedUnitObj.SetIsSelected(true);
        selectedUnitObj.ActivateSelectedBorders(selectedUnitObj);
    }
    
    
    public void ClearSelectedUnit()
    {
        if (selectedUnit != null )
        {
            selectedUnit.GetComponent<Unit>().RemoveSelectedUnit();
        }
        
        selectedUnit = null;
        
    }
    
    
    public void SelectAllSameTypeUnits()
    {
        ClearSelectedGroup();

        if (selectedUnit != null)
        {
            if (selectedUnit.GetComponent<Paladin>() != null)
            {
                Paladin[] allPaladins = FindObjectsOfType<Paladin>();

                foreach (Paladin paladin in allPaladins)
                {
                    selectedGroup.Add(paladin);
                    paladin.SetIsSelected(true);
                    paladin.ActivateHoverBorders(paladin);
                }
            }
            else if (selectedUnit.GetComponent<Archer>() != null)
            {
                Archer[] allArchers = FindObjectsOfType<Archer>();

                foreach (Archer archer in allArchers)
                {
                    selectedGroup.Add(archer);
                    archer.SetIsSelected(true);
                    archer.ActivateHoverBorders(archer);
                }
            }
            else if (selectedUnit.GetComponent<Worker>() != null)
            {
                Worker[] allWorkers = FindObjectsOfType<Worker>();

                foreach (Worker worker in allWorkers)
                {
                    selectedGroup.Add(worker);
                    worker.SetIsSelected(true);
                    worker.ActivateHoverBorders(worker);
                }
            }
        }

        
        
    }
   public void ClearSelectedGroup()
    {
       
        foreach (Unit unit in selectedGroup)
        {
            unit.SetIsSelected(false);
            unit.DeactivateHoverBorders(unit);
        }

        selectedGroup.Clear();
    }

    public void SetSelectedGroupText(Unit unit)
    {
        if (unit.GetComponent<Paladin>() != null)
        {
            groupSelectText.text = "Select All Paladins";
        }
        else if (unit.GetComponent<Archer>() != null)
        {
            groupSelectText.text = "Select All Archers";
        }
        else if (unit.GetComponent<Worker>() != null)
        {
            groupSelectText.text = "Select All Workers";
        }
    }
    public void SetSelectedGroupNull()
    {
        ClearSelectedGroup();
        groupSelectText.text = " ";
    }
}