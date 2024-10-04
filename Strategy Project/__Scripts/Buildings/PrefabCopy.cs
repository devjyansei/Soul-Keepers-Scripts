using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabCopy : MonoBehaviour
{  
    Vector3 screenPosition;
    [SerializeField] StructureDataSO structureDataSO;
    Tile centerTile = null;
    Tile closestTile = null;
    public List<Unit> unitsInRadiusList = new List<Unit>();
    public List<Enemy> enemiesInRadiusList = new List<Enemy>();
    public LayerMask unit;
    public LayerMask enemy;
    public float detectionRange = 4.5f;
    private void Update()
    {
        FollowMouse();

        if (Input.GetMouseButtonDown(0))
        {
            EventManager.OnBuildingPlaced?.Invoke();
        }
    }
    private void OnEnable()
    {
        EventManager.OnBuildingPlaced += PlaceIt;
    }
    private void OnDisable()
    {
        EventManager.OnBuildingPlaced -= PlaceIt;
    }


    

    public void PlaceIt()
    {
        centerTile = GetClosestTile();

       
        if (CanBuild() && IsResourcesAfford() && BuildingManager.Instance.CanPlacingStart(centerTile.transform.position,UnitSelections.Instance.selectedUnit.GetComponent<Worker>()))      
        {
            //pay
            PayBuildingCost();

            //Spawn
            GameObject newBuilding = Instantiate(structureDataSO.prefab);
            newBuilding.transform.position = new Vector3(centerTile.transform.position.x, structureDataSO.prefab.transform.position.y, centerTile.transform.position.z);

            //Node settings

            List<Tile> tilesToBeSealed = new List<Tile>();

            int buildingWidth = structureDataSO.areaCovered.x;
            int gameAreaWidth = TileManager.Instance.GetGameAreaWidth();


            // Find Target Tiles
            int LeftMidTileNumber = centerTile.listOrder - (buildingWidth / 2);

            for (int i = LeftMidTileNumber; i < LeftMidTileNumber + buildingWidth; i++)
            {
                tilesToBeSealed.Add(TileManager.Instance.tilesList[i]);
            }

            if (structureDataSO.areaCovered.x >= 3)
            {
                int leftBottomTileNumber = centerTile.listOrder - gameAreaWidth - (buildingWidth / 2);//2

                for (int i = leftBottomTileNumber; i < leftBottomTileNumber + buildingWidth; i++)
                {
                    tilesToBeSealed.Add(TileManager.Instance.tilesList[i]);
                }


                int leftUpTileNumber = centerTile.listOrder + gameAreaWidth - (buildingWidth / 2);//2

                for (int i = leftUpTileNumber; i < leftUpTileNumber + buildingWidth; i++)
                {
                    tilesToBeSealed.Add(TileManager.Instance.tilesList[i]);
                }
            }
            


            

            
            // Set Tiles

            foreach (var tile in tilesToBeSealed)
            {
                tile.node.SetPlaceable(false);
                tile.node.SetWalkable(false);

                newBuilding.GetComponent<Building>().coveredTiles.Add(tile);
            }

            

            //state settings
            BuildingManager.Instance.CancelPlacingCopy();
            


        }
        else
        {
            UiManager.Instance.OpenInsufficientResourcePanel();
        }
    }
    private void PayBuildingCost()
    {
        ResourceManager.Instance.DecreaseCurrentWoodAmount(structureDataSO.woodCost);
        ResourceManager.Instance.DecreaseCurrentStoneAmount(structureDataSO.stoneCost);
        ResourceManager.Instance.DecreaseCurrentDiamondAmount(structureDataSO.diamondCost);
    }
    private bool IsResourcesAfford()
    {
        if (ResourceManager.Instance.GetCurrentWoodAmount() >= structureDataSO.woodCost
             && ResourceManager.Instance.GetCurrentStoneAmount() >= structureDataSO.stoneCost
             && ResourceManager.Instance.GetCurrentDiamondAmount() >= structureDataSO.diamondCost)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool CanBuild() //Eger binanýn x ve z de geniþligi kadar tile boþtaysa true, deðilse false döner.
    {
       
        //Eger köþedeki tilelardan degilse
        if (centerTile.node.isBorderNode)
        {
            //you cant build top of a unit
            UiManager.Instance.CloseBuildingInUnitErrorInfoPanel();
            return false;
        }
        // radius içinde unit yada enemy varsa
        if (AnyUnitsInRadius() || AnyEnemiesInRadius())
        {
            UiManager.Instance.OpenBuildingInUnitErrorInfoPanel();
            return false;
        }
        

        List<Tile> tilesToBeSealed = new List<Tile>();

        int buildingWidth = structureDataSO.areaCovered.x;
        int gameAreaWidth = TileManager.Instance.GetGameAreaWidth();

        int LeftMidTileNumber = centerTile.listOrder - (buildingWidth / 2);

        for (int i = LeftMidTileNumber; i < LeftMidTileNumber + buildingWidth; i++)
        {
            tilesToBeSealed.Add(TileManager.Instance.tilesList[i]);
        }

        if (buildingWidth >= 3)
        {
            int leftBottomTileNumber = centerTile.listOrder - gameAreaWidth - (buildingWidth / 2);//2

            for (int i = leftBottomTileNumber; i < leftBottomTileNumber + buildingWidth; i++)
            {
                tilesToBeSealed.Add(TileManager.Instance.tilesList[i]);
            }


            int leftUpTileNumber = centerTile.listOrder + gameAreaWidth - (buildingWidth / 2);//2

            for (int i = leftUpTileNumber; i < leftUpTileNumber + buildingWidth; i++)
            {
                tilesToBeSealed.Add(TileManager.Instance.tilesList[i]);
            }
        }
        


        
        

        foreach (var tile in tilesToBeSealed)
        {
            if (tile.node.canPlaceable == false)
            {
                return false;
            }            
        }
        return true;


    }

    private Tile GetClosestTile()
    {
        Tile[] tiles = FindObjectsOfType<Tile>();
        
        float closestDistance = Mathf.Infinity;
        closestTile = null;

        for (int i = 0; i < tiles.Length; i++)
        {
            float distance = Vector3.Distance(this.transform.position, tiles[i].transform.position);
            
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = tiles[i];
            }
                      
        }
        return closestTile;
    }

    private void FollowMouse()
    {
        screenPosition = Input.mousePosition;

        RaycastHit hitData;
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out hitData, Mathf.Infinity))
        {
            hitData.transform.TryGetComponent(out Tile tile);
            if (tile != null)
            {
                transform.position = new Vector3(tile.node.coordinates.x,0f, tile.node.coordinates.y);

                
            }
            
        }

    }

    protected bool AnyUnitsInRadius()
    {
        unitsInRadiusList.Clear();

        Collider[] unitColliders = Physics.OverlapSphere(this.transform.position, detectionRange, unit);

        if (unitColliders.Length > 0)
        {
            return true;
            
        }
        else
        {
            return false;
        }

    }
    protected bool AnyEnemiesInRadius()
    {
        unitsInRadiusList.Clear();

        Collider[] enemyColliders = Physics.OverlapSphere(this.transform.position, detectionRange, enemy);

        if (enemyColliders.Length > 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
