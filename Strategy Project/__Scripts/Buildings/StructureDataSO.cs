using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class StructureDataSO : ScriptableObject
{
    public Sprite sprite;

    public GameObject prefab;
    public GameObject buildingCopyPrefab;

    public Vector2Int areaCovered;

    public int woodCost;
    public int stoneCost;
    public int diamondCost;

    public Sprite defaultSprite;

    public List<bool> isAnyAttributes = new List<bool>();
    public List<string> attributesNamesList = new List<string>();
    public List<string> attributesDescription = new List<string>();
    public List<bool> isAttributeReusable = new List<bool>();
    public List<bool> isAttributeUsed = new List<bool>();
    public List<Sprite> attributesImageList = new List<Sprite>();
    public List<float> attributesGenerateSpeedList = new List<float>();
    


    public List<int> attributesWoodCostsList = new List<int>();
    public List<int> attributesStoneCostsList = new List<int>();
    public List<int> attributesDiamondCostsList = new List<int>();


    public List<Button.ButtonClickedEvent> attributesClickEventList = new List<Button.ButtonClickedEvent>();





}


