using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour
{
    public List<Unit> unitsInRadiusList = new List<Unit>();
    [SerializeField] LayerMask unit;
    [SerializeField] float detectionRange;
    private bool canStartMove=false;
    Transform target;
    [SerializeField] private float speed;
    [SerializeField] GameObject visual;
    [SerializeField] ParticleSystem collectParticle;
    [SerializeField] float movementDelay;
    [SerializeField] float collectDelay;
    float duration;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collectSound;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        StartCoroutine(UnitDetect());
        Invoke("TurnCanMove", movementDelay);
       
    }
    public void Update()
    {

        duration += Time.deltaTime;

        if (target != null && canStartMove)
        {
            transform.position += speed * Time.deltaTime * GetDirection(target);
        }
        else if (target == null)
        {
            return;
        }
    }
    private void TurnCanMove()
    {   
        canStartMove = true;    
    }




    protected Unit GetClosestUnit()
    {
        Unit closestUnit = null;
        float closestDistance = Mathf.Infinity;
        if (unitsInRadiusList.Count > 0)
        {
            for (int i = 0; i < unitsInRadiusList.Count; i++)
            {
                if (Vector3.Distance(unitsInRadiusList[i].transform.position, this.transform.position) < closestDistance)
                {
                    closestUnit = unitsInRadiusList[i];
                }

            }
            return closestUnit;
        }
        return null;
    }

    protected void SetUnitsInRadius()
    {
        unitsInRadiusList.Clear();

        Collider[] unitColliders = Physics.OverlapSphere(this.transform.position, detectionRange, unit);

        foreach (var unitCollider in unitColliders)
        {
            if (unitCollider.GetComponent<Unit>().GetCurrentHealth() > 0)
            {
                unitsInRadiusList.Add(unitCollider.GetComponent<Unit>());
            }

        }


    }
    private Vector3 GetDirection(Transform target)
    {

        Vector3 targetPos = new Vector3(target.position.x, target.position.y + 1, target.position.z);
        return (targetPos - transform.position).normalized;
    }
 
    private IEnumerator UnitDetect()
    {

        while (true)
        {
            
            SetUnitsInRadius();

            if (unitsInRadiusList.Count > 0)
            {
                target = GetClosestUnit().transform;
            }

            yield return new WaitForSeconds(.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Unit") && duration > collectDelay)
        {

            ResourceManager.Instance.IncreaseCurrentSoulAmount(1);

            visual.SetActive(false);
            collectParticle.Play();
            PlayCollectSound();
            Destroy(this.gameObject,1f);
            

        }
    }
    
    public void PlayCollectSound()
    {
        audioSource.PlayOneShot(collectSound);
    }
}
