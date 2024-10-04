using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerProjectile : MonoBehaviour
{
    private Transform targetPosition;
    public int damage;
    
    private void Update()
    {
        transform.LookAt(targetPosition);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(this.gameObject);
        }
        if (collision.gameObject.CompareTag("Tile"))
        {
            Destroy(this.gameObject);
        }
    }
}
