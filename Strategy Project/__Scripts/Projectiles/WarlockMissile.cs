using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarlockMissile : MonoBehaviour
{
    Rigidbody rb;
    [HideInInspector] public Warlock owner;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Unit"))
        {
            other.GetComponent<Unit>().TakeDamage(owner.GetDamage());
            gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        Vector3 direction = owner.currentEnemy.transform.position - this.transform.position+Vector3.up;
        MoveThrougEnemy(direction);
    }

    private void MoveThrougEnemy(Vector3 direction)
    {
        rb.velocity += direction * owner.GetMissileSpeed() * Time.fixedDeltaTime;
    }


}
