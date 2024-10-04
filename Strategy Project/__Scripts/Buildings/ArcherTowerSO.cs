using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Structures/ArcherTower")]
public class ArcherTowerSO : StructureDataSO
{
    public int damage;
    public int attackRange;
    public int attackInterval;

    public int currentDamageIncreasement = 0;
    public int currentRangeIncreasement = 0;
    public int currentSpeedIncreasement = 0;
}
