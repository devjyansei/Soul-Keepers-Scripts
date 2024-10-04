using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node 
{
    public Vector2 coordinates;
    public bool canWalkable;
    public bool canPlaceable;
    public bool isBorderNode;


    public void SetPlaceable(bool state)
    {
        canPlaceable = state;
    }
    public void SetWalkable(bool state)
    {
        canWalkable = state;
    }
}
