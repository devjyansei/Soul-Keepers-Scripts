using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
public class ArcherTower : Building
{
    [SerializeField] ArcherTowerSO archerTowerDataSO;
    [SerializeField] GameObject crossbow;
    public List<Enemy> enemiesInRadiusList = new List<Enemy>();
    [SerializeField] private GameObject projectilePrefab;

    public Enemy currentTarget;
    public LayerMask enemyLayer;
    [SerializeField] private GameObject firePoint;

    [SerializeField] private int damage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private float projectileSpeed = .3f;

    // ------- GETTER / SETTER -------- //
    public int GetDamage() { return damage; }
    public void SetDamage(int value) => damage = value;
    public float GetRange() { return attackRange; }
    public void SetRange(float value) => attackRange = value;

    public float GetAttackInterval() { return attackInterval; }
    public void SetAttackInterval(float value) => attackInterval = value;

    private void OnMouseDown()
    {

        if (!EventSystem.current.IsPointerOverGameObject() && !GameManager.Instance.IsGameEnded())
        {
            if (BuildingManager.Instance.currentState != BuildingManager.BuildingStates.placingCopy)
            {
                UiManager.Instance.ClearAllButtonEventSetting();
                BuildingManager.Instance.SetSelectedBuilding(this);
                UnitSelections.Instance.SetSelectedGroupNull();
            }

            UiManager.Instance.UpdateDisplayedAttributes(this); // image ve butonlarý ayarlar

            EventManager.OnAnyBuildingSelected?.Invoke(this);
        }
    }
    private void OnEnable()
    {
        StartCoroutine(BorderController());
    }
    private void Start()
    {
        SetArcherTowerButtonAttributes();
        SetArcherTowerStats();
        StartCoroutine(CheckForEnemiesInRange());
        PlayBuildSound();
        SetRotation();
    }
    private void SetRotation()
    {
        if (transform.position.z > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 270f, 0f);
        }
        if (transform.position.z < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
    }
    private void SetArcherTowerButtonAttributes()
    {
        for (int i = 0; i < structureDataSO.attributesClickEventList.Count; i++)
        {
            structureDataSO.attributesClickEventList[i] = ClickEventsManager.Instance.archerTowerAttributesList[i];
        }
    }
    private IEnumerator CheckForEnemiesInRange()
    {
        while (true)
        {
            if (IsAnyEnemiesInDetectList())
            {
                
                currentTarget = GetClosestEnemy();
                Attack();
            }

            yield return new WaitForSeconds(attackInterval);
        }
    }

    private void Attack()
    {
        SpawnAndFireProjectile();      
    }

    private Enemy GetClosestEnemy()
    {

        Enemy closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        if (enemiesInRadiusList.Count > 0)
        {
            for (int i = 0; i < enemiesInRadiusList.Count; i++)
            {
                if (Vector3.Distance(enemiesInRadiusList[i].transform.position, this.transform.position) < closestDistance)
                {
                    closestEnemy = enemiesInRadiusList[i];
                }

            }
            return closestEnemy;
        }
        return null;
    }

    protected bool IsAnyEnemiesInDetectList()
    {
        enemiesInRadiusList.Clear();

        Collider[] enemyColliders = Physics.OverlapSphere(this.transform.position, attackRange, enemyLayer);

        foreach (var unitCollider in enemyColliders)
        {
            enemiesInRadiusList.Add(unitCollider.GetComponent<Enemy>());
        }

        if (enemiesInRadiusList.Count > 0)
        {
            return true;
        }
        return false;
    }
    
    private void SpawnAndFireProjectile()
    {

        GameObject newProjectile = Instantiate(projectilePrefab, firePoint.transform.position, Quaternion.identity);
        newProjectile.GetComponent<TowerProjectile>().damage = damage;
        Vector3 targetPosition = new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y + 1.5f, currentTarget.transform.position.z);
        newProjectile.transform.DOJump(targetPosition, 1, 1,projectileSpeed);

        
    }
    public void SetArcherTowerStats()
    {
        SetDamage(archerTowerDataSO.damage);
        SetRange(archerTowerDataSO.attackRange);
        SetAttackInterval(archerTowerDataSO.attackInterval);
    }
    IEnumerator BorderController()
    {
        while (true)
        {
            if (BuildingManager.Instance.selectedStructurePrefab == this.gameObject)
            {
                ActivateSelectedBorders(this);
            }
            else if (BuildingManager.Instance.selectedStructurePrefab != this)
            {
                DeactivateSelectedBorders(this);
            }
            yield return new WaitForSeconds(0.15f);
        }
    }
    public void PlayUpgradeParticle()
    {
        upgradeParticle.Play();
    }
    public void PlayUpgradeSound()
    {
        audioSource.clip = upgradeSound;
        audioSource.Play();
    }
}
