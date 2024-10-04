using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class PlayerInputManager : MonoSingleton<PlayerInputManager>
{


    Vector3 screenPosition;
    public GameObject moveMarker;
    GameObject livingMarker;
    [HideInInspector] public Unit markersOwner;

    [Header("KeyCodes")]
    public KeyCode snapCamToUnit = KeyCode.Space;
    public KeyCode toggleSettings = KeyCode.Escape;

    [Header("Layers")]
    public LayerMask ground;
    public LayerMask building;
    public LayerMask unit;
    public LayerMask chest;
    public LayerMask ui;

    [Header("Cursors")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D attackCursor;
    [SerializeField] private Texture2D collectCursor;
    [SerializeField] private Texture2D selectCursor;
    [SerializeField] private Texture2D lootCursor;

    private void Start()
    {
        Cursor.SetCursor(defaultCursor, new Vector2(16,0), CursorMode.ForceSoftware);
        GameManager.Instance.SetIsGameStarted(true);
    }





    private void Update()
    {
        #region SHORTCUTS
        // Move Marker
        if (UnitSelections.Instance.selectedUnit == null)
        {
            DestroyMoveMarker();
        }

        // Settings
        if (Input.GetKeyDown(toggleSettings))
        {
            UiManager.Instance.ToggleSettingsPanel();
        }

        if (Input.GetKey(snapCamToUnit))
        {
            if (UnitSelections.Instance.selectedUnit != null)
            {
                Camera.main.transform.position = new Vector3(UnitSelections.Instance.selectedUnit.transform.position.x, Camera.main.transform.position.y, UnitSelections.Instance.selectedUnit.transform.position.z - 10);
            }
            else
            {
                Camera.main.transform.position = new Vector3(0, 25, -5);
            }
        }


        GetMousePositionInWorld();

        #endregion

        // ON NORMAL CLICK

        if (Input.GetMouseButtonDown(0) && BuildingManager.Instance.currentState != BuildingManager.BuildingStates.placingCopy)
        {
            RaycastHit hitData;
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            // UI CLICK
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            else if (!EventSystem.current.IsPointerOverGameObject())
            {
                UnitSelections.Instance.ClearSelectedUnit();

                if (Physics.Raycast(ray, out hitData, Mathf.Infinity, unit))
                {
                    GameObject clickedUnit = hitData.collider.gameObject;
                    UnitSelections.Instance.ClearSelectedGroup();
                    UnitSelections.Instance.SetSelectedUnit(clickedUnit);
                    BuildingManager.Instance.ClearSelectedStructure();
                    EventManager.OnAnyUnitSelected?.Invoke(clickedUnit.GetComponent<Unit>());
                    return;

                }
                else if (Physics.Raycast(ray, out hitData, Mathf.Infinity, building))
                {
                    EventManager.OnAnyBuildingSelected?.Invoke(hitData.transform.GetComponent<Building>());
                    UnitSelections.Instance.ClearSelectedUnit();
                    UnitSelections.Instance.ClearSelectedGroup();
                    return;
                }
                else if (Physics.Raycast(ray, out hitData, Mathf.Infinity, ground))
                {
                    UiManager.Instance.CloseControlPanel();
                    UiManager.Instance.ClearAllButtonEventSetting();
                    UnitSelections.Instance.ClearSelectedUnit();
                    UnitSelections.Instance.ClearSelectedGroup();
                    BuildingManager.Instance.ClearSelectedStructure();

                }



            }



        }
        if (Input.GetMouseButtonDown(1))
        {


            if (UnitSelections.Instance.selectedUnit != null && BuildingManager.Instance.currentState != BuildingManager.BuildingStates.placingCopy)
            {
                RaycastHit hitData;
                Ray ray = Camera.main.ScreenPointToRay(screenPosition);

                if (Physics.Raycast(ray, out hitData, Mathf.Infinity, ground))
                {
                    InstantiateMoveMarker(hitData.point);
                }

                if (Physics.Raycast(ray, out hitData, Mathf.Infinity, chest))
                {
                    StartCoroutine(MoveSelectedUnitToChest(hitData.transform.GetComponent<Chest>()));
                    DestroyMoveMarker();
                    //return;//hata olursa kaldýr(chestte yürüme iconu cýkmasýn dýye koydum)
                }

                
            }

            
        }

    }    
    IEnumerator MoveSelectedUnitToChest(Chest chest)
    {
        Unit unit = UnitSelections.Instance.selectedUnit.GetComponent<Unit>();
        while (true)
        {
            
            float distanceBetweenUnitAndChest = Vector3.Distance(chest.transform.position, unit.transform.position);
            if ( distanceBetweenUnitAndChest < 2)
            {
                chest.OpenChest();
                break;
            }
            yield return new WaitForSeconds(.1f);
        }




    }



    Vector3 GetMousePositionInWorld()
    {
        screenPosition = Input.mousePosition;


        RaycastHit hitData;
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out hitData, Mathf.Infinity))
        {
            //Debug.Log(hitData.transform.position);
            return hitData.transform.position;       
        }
        return Vector3.zero;
    }
    public void InstantiateMoveMarker(Vector3 position)
    {
        if (livingMarker == null)
        {
            markersOwner = UnitSelections.Instance.selectedUnit.GetComponent<Unit>();

            livingMarker = Instantiate(moveMarker, position, Quaternion.identity);
        }
        else
        {
            Destroy(livingMarker);
            livingMarker = null;
            markersOwner = UnitSelections.Instance.selectedUnit.GetComponent<Unit>();
            livingMarker = Instantiate(moveMarker, position, Quaternion.identity);
           


        }


    }
    public void DestroyMoveMarker()
    {
        Destroy(livingMarker);
        livingMarker = null;
        markersOwner = null;
    }





    // CURSOR METHODS
    public void SwitchToDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, new Vector2(16, 0), CursorMode.ForceSoftware);
    }
    public void SwitchToSelectCursor()
    {
        Cursor.SetCursor(selectCursor, new Vector2(16, 0), CursorMode.ForceSoftware);
    }
    public void SwitchToAttackCursor()
    {
        Cursor.SetCursor(attackCursor, new Vector2(16, 0), CursorMode.ForceSoftware);
    }
    public void SwitchToCollectCursor()
    {
        Cursor.SetCursor(collectCursor, new Vector2(16, 0), CursorMode.ForceSoftware);
    }
    public void SwitchToChestCursor()
    {
        Cursor.SetCursor(lootCursor, new Vector2(16, 0), CursorMode.ForceSoftware);
    }
}
