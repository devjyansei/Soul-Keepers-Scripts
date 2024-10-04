using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TileManager : MonoSingleton<TileManager>
{
    public List<Tile> tilesList = new List<Tile>();



    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject tilesParent;
    [SerializeField] private Vector3 tileStartPosition;

    [SerializeField] private int gameAreaWidth;//x
    [SerializeField] private int gameAreaHeight;//z


    void Start()
    {
       // InstantiateTiles();
    }

    private void InstantiateTiles()
    {
        for (int i = 0; i < gameAreaWidth; i++)
        {
            for (int j = 0; j < gameAreaHeight; j++)
            {
                GameObject newTile = Instantiate(tilePrefab);
                newTile.transform.position = tileStartPosition + new Vector3(i, 0, j)*tilePrefab.transform.localScale.x;
                newTile.transform.parent = tilesParent.transform;
            }
        }
        
        

    }
    public int GetGameAreaWidth()
    {
        return gameAreaWidth;
    }
    public int GetGameAreaHeight()
    {
        return gameAreaHeight;
    }
    private void SetTilePlaceable()
    {

    }
}
