using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDataSO : ScriptableObject
{

    public float speed;
    public int maxHealth;
    public int damage;
    public float attackRange;
    public float detectionRange;
    public float attackSpeedMultiplier;
    public float diamondReward;
    public float rewardMultiplier = 1f;
    
}
