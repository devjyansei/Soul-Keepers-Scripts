using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitDataSO : ScriptableObject
{

    public Sprite image;
    public SpriteRenderer defaultSprite;
    public float speed;
    public int maxHealth;

    public float attackRange;
    public float detectionRange;
    public int healthRegenPerFiveSecond;
    public int damage;
    public float attackSpeed;
    public int armor;
    public float attackSpeedMultiplier;
    public float productivity;

    public int woodCost;
    public int stoneCost;
    public int diamondCost;

    public float generateSpeed;



    public List<Sprite> attributesImageList = new List<Sprite>();
    public List<string> attributesNameList = new List<string>();
    public List<string> attributesDescriptionList = new List<string>();
    public List<Button.ButtonClickedEvent> attributesClickEventList = new List<Button.ButtonClickedEvent>();
    public List<string> attributeMessages  = new List<string>();
    //test
    public void IncreaseMovementSpeed(int amount)
    {
        speed += amount;
    }
    [Header("Damage Costs")]
    public int damageIncreaseWoodCost;
    public int damageIncreaseStoneCost;
    public int damageIncreaseDiamondCost;

    [Header("Defensive Costs")]
    public int defensiveIncreaseWoodCost;
    public int defensiveIncreaseStoneCost;
    public int defensiveIncreaseDiamondCost;

    [Header("Utility Costs")]
    public int utilityIncreaseWoodCost;
    public int utilityIncreaseStoneCost;
    public int utilityIncreaseDiamondCost;

    [Header("Damage")]
    public int damageIncreasementAmount;

    [Header("Max Health")]
    public int maxHealthIncreasementAmount;

    [Header("Armor")]
    public int armorIncreasementAmount;

    [Header("Health Regen")]
    public int healthRegenIncreasementAmount;

    [Header("Speed")]
    public float speedIncreasementAmount;
}
