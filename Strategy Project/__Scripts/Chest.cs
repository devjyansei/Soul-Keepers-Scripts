using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
public class Chest : MonoBehaviour
{
    Animator animator;
    NavMeshObstacle obstacle;
    [SerializeField] GameObject soulPrefab;
    [SerializeField] GameObject[] onHoverBorder;
    [SerializeField] ParticleSystem chestOpenEffect;

    private int soulReward;

    public float rewardMultiplier = 1f;

    [Header("Wood")]
    [SerializeField] private int minSoulReward;
    [SerializeField] private int maxSoulReward;




    public ChestType currentChestType;
    public enum ChestType
    {
        silver,
        golden,
        diamond
    }
    public void SwitchChestType(ChestType chestType)
    {
        currentChestType = chestType;

        switch (currentChestType)
        {
            case ChestType.silver:
                animator.SetBool("isSilver", true);
                break;
            case ChestType.golden:
                animator.SetBool("isGolden", true);

                break;
            case ChestType.diamond:
                animator.SetBool("isDiamond", true);

                break;
            default:
                break;
        }
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        obstacle = GetComponent<NavMeshObstacle>();
    }
    private void OnMouseEnter()
    {
        for (int i = 0; i < onHoverBorder.Length; i++)
        {
            onHoverBorder[i].SetActive(true);
            
        }

        if (UnitSelections.Instance.selectedUnit != null)
        {
            PlayerInputManager.Instance.SwitchToChestCursor();
        }
        
    }
    private void OnMouseExit()
    {
        for (int i = 0; i < onHoverBorder.Length; i++)
        {
            onHoverBorder[i].SetActive(false);
            PlayerInputManager.Instance.SwitchToDefaultCursor();

        }
    }
    public void OpenChest()
    {
        DropSoul();
        animator.SetTrigger("open");
        transform.gameObject.layer = 0;
        obstacle.carving = true;
        chestOpenEffect.Play();
        //obstacle.enabled = false;
    }
    

   
    private void SetRewardAccordingToChestType()
    {
        soulReward = Random.Range(minSoulReward, maxSoulReward);
       
    }
    private void Start()
    {
        SetRewardAccordingToChestType();
        SwitchChestType(currentChestType);
    }

    public void DropSoul()
    {
        int dropnumber = Random.Range(minSoulReward, maxSoulReward);
        int dropNumberWithMultiplier = dropnumber + Mathf.FloorToInt(dropnumber * rewardMultiplier);

        for (int i = 0; i < dropNumberWithMultiplier; i++)
        {
            GameObject soul = Instantiate(soulPrefab, transform.position, transform.rotation);
            Vector3 randomDirection = new Vector3(Random.Range(-100, 100), transform.position.y, Random.Range(-100, 100)).normalized;
            float randomDropForce = Random.Range(2, 5);
            soul.transform.DOJump(randomDirection * randomDropForce + transform.position, 1f, 1, 1f);
        }


    }
}
