using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Node node;
    public int listOrder;
    private void Start()
    {
        SetCoordinates();
        SetTileListOrder();
        //TileManager.Instance.tilesList.Add(this);
    }
    void SetCoordinates()
    {
        node.coordinates.x = transform.position.x;
        node.coordinates.y = transform.position.z;
    }
    private void SetTileListOrder()
    {
        List<Tile> tileList = TileManager.Instance.tilesList;

        for (int i = 0; i < tileList.Count; i++)
        {
            if (tileList[i] == this)
            {
                listOrder = i;
                return;
            }
        }
        
    }
    
}
