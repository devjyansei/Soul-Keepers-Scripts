using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="Structures/Academy")]
public class AcademySO : StructureDataSO
{

    [Header("Damage")]
    public int allDamageIncreasementAmount;

    [Header("Max Health")]
    public int allMaxHealthIncreasementAmount;

    [Header("Armor")]
    public int allArmorIncreasementAmount;

    [Header("Health Regen")]
    public int allHealthRegenIncreasementAmount;

    [Header("Speed")]
    public float allSpeedIncreasementAmount;


    [Header("All Damage Costs")]
    public int allDamageIncreaseWoodCost;
    public int allDamageIncreaseStoneCost;
    public int allDamageIncreaseDiamondCost;

    [Header("All Defence Costs")]
    public int allDefenceIncreaseWoodCost;
    public int allDefenceIncreaseStoneCost;
    public int allDefenceIncreaseDiamondCost;

    [Header("All Utility Costs")]
    public int allUtilityIncreaseWoodCost;
    public int allUtilityIncreaseStoneCost;
    public int allUtiltyIncreaseDiamondCost;

    public int currentAllDamageIncreasement=0;
    public int currentAllDefenceIncreasement=0;
    public int currentAllUtilityIncreasement=0;
}
