using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoSingleton<BuildingManager>
{
    public StructureDataSO selectedStructureData; //will hide in inspector
    public GameObject selectedStructurePrefab; //will hide in inspector
    public GameObject lastSelectedStructurePrefab; //will hide in inspector

    public List<Building> buildingsList = new List<Building>();

    public StructureDataSO coreDataSO;  // workerýn(yada unitlerin) startýnda,  dataSo larýndaki attribute kýsýmlarýna,  buildmanagerdaki datalarý tek tek aktarýcaz.

    public StructureDataSO houseDataSO;
    public StructureDataSO barrackDataSO; 
    public StructureDataSO archerTowerSO; 
    public StructureDataSO academySO; 
    public StructureDataSO templeSO;
    public StructureDataSO woodGeneratorSO;
    public StructureDataSO stoneGeneratorSO;

    public Vector3 buildingPosition;
    public float buildingDistance;

    [SerializeField] LineRenderer connectionLineRenderer;
    [SerializeField] Material farConnectionMaterial;
    [SerializeField] Material closeConnectionMaterial;

    [SerializeField] GameObject temporaryPrefab;

    
    public BuildingStates currentState;
    public enum BuildingStates
    {
        notBuilding,
        placingCopy

    }
    public void SwitchState(BuildingStates state)
    {
        currentState = state;

        switch (currentState)
        {
            case BuildingStates.notBuilding:
                selectedStructureData = null;
                UiManager.Instance.CloseCancelBuildingInfoPanel();    
                UiManager.Instance.OpenMinimapCanvas();
                UiManager.Instance.OpenUnitControlPanel(UnitSelections.Instance.lastSelectedUnit);

                break;

            case BuildingStates.placingCopy:
                UiManager.Instance.OpenCancelBuildingInfoPanel();
                UiManager.Instance.CloseControlPanel();
                UiManager.Instance.OpenMinimapCanvas();
                TooltipManager._instance.HideTooltip();

                break;

            default:
                break;
        }
    }

    private void Start()
    {
        connectionLineRenderer.startWidth = 0.1f;
    }
    private void OnEnable()
    {
        EventManager.OnBuildingCopySelectPhaseStarted += InstantiateBuildingCopy;
        EventManager.OnBuildingCopySelectPhaseStarted += SetSelectedStructure;


        
    }
    private void OnDisable()
    {
        EventManager.OnBuildingCopySelectPhaseStarted -= InstantiateBuildingCopy;
        EventManager.OnBuildingCopySelectPhaseStarted -= SetSelectedStructure;


    }
    public void OpenConnectionRenderer()
    {
        connectionLineRenderer.gameObject.SetActive(true);
    }
    public void CloseConnectionRenderer()
    {
        connectionLineRenderer.gameObject.SetActive(false);
    }
    public bool IsConnectionRendererActive()
    {
        return connectionLineRenderer.gameObject.activeInHierarchy;       
        
    }
    public void DrawConnectionRenderer(Vector3 startPos, Vector3 endPos)
    {
        
        connectionLineRenderer.SetPosition(0, startPos + Vector3.up);
        connectionLineRenderer.SetPosition(1, endPos + Vector3.up);

        
        if (CanPlacingStart(temporaryPrefab.transform.position,UnitSelections.Instance.selectedUnit.GetComponent<Worker>()))
        {
            UiManager.Instance.CloseBuildingDistanceErrorPanel();
            connectionLineRenderer.material = closeConnectionMaterial;
            
        }

        else
        {
            if (!UiManager.Instance.IsInsufficientResourcePanelOpened())
            {
                UiManager.Instance.OpenBuildingDistanceErrorPanel();

                connectionLineRenderer.material = farConnectionMaterial;
            }
            

            
        }

        
    }
    
    private void Update()
    {
        if (IsConnectionRendererActive())
        {
            DrawConnectionRenderer(UnitSelections.Instance.selectedUnit.transform.position, selectedStructurePrefab.transform.position);
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButton(1))
        {
            if (currentState == BuildingStates.placingCopy)
            {
                CancelPlacingCopy();

            }

        }
        
    }
    
    // Called from Button
    public void StartBuildingCopyPlacePhase(StructureDataSO structureDataSO)
    {
        if (CanPlacingCopyStart())
        {
            SwitchState(BuildingStates.placingCopy);

            EventManager.OnBuildingCopySelectPhaseStarted?.Invoke(structureDataSO);
            
            OpenConnectionRenderer();
            

        }
        
    }
    
    public void CancelPlacingCopy()
    {
        SwitchState(BuildingStates.notBuilding);
        UiManager.Instance.CloseBuildingDistanceErrorPanel();
        CloseConnectionRenderer();
        Destroy(selectedStructurePrefab);
        selectedStructurePrefab = null;
    }
    
    public bool CanPlacingStart(Vector3 position,Worker worker)
    {

        buildingPosition = position;

        if (Vector3.Distance(buildingPosition,worker.transform.position) < buildingDistance)
        {
            return true;
        }
        else
        {
            
            //UiManager.Instance.OpenBuildingDistanceErrorPanel();
            return false;
        }

        
    }
    void InstantiateBuildingCopy(StructureDataSO structureDataSO)
    {
        temporaryPrefab = selectedStructurePrefab = Instantiate(structureDataSO.buildingCopyPrefab);
        
       
        
    }
    void SetSelectedStructure(StructureDataSO structureDataSO)
    {
        selectedStructureData = structureDataSO;
    }
    bool CanPlacingCopyStart()
    {
        
        
        if (currentState == BuildingStates.notBuilding)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public void SetSelectedBuilding(Building building)
    {
        selectedStructurePrefab = building.gameObject;
        selectedStructureData = building.structureDataSO;
        lastSelectedStructurePrefab = building.gameObject;
    }
    
    public void ClearSelectedStructure()
    {
        selectedStructureData = null;
        selectedStructurePrefab = null;
    }
}
