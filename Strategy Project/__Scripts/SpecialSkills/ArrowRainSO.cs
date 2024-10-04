using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SpecialAttack/ArrowRain")]
public class ArrowRainSO : SpecialAttackSO
{
    public ParticleSystem effect;
    public int arrowRainDamage;
    public float arrowRainRadius;
    public LayerMask enemy;


  
    public void UseSpecialAttack(Vector3 originPosition)
    {
        ParticleSystem arrowRainEffect = Instantiate(effect);
        arrowRainEffect.transform.position = originPosition;
        arrowRainEffect.Play();
        DealDamageInRadius(originPosition);
    }
    protected void DealDamageInRadius(Vector3 originPosition)
    {


        Collider[] enemyColliders = Physics.OverlapSphere(originPosition, arrowRainRadius, enemy);

        foreach (var enemyCollider in enemyColliders)
        {
            if (enemyCollider.GetComponent<Enemy>().GetCurrentHealth() > 0)
            {
                enemyCollider.GetComponent<Enemy>().TakeDamage(arrowRainDamage);
            }

        }


    }
}
