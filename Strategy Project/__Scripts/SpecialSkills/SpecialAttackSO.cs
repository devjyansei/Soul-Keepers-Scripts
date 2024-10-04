using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttackSO : ScriptableObject
{
    public ArrowRainSO arrowRainSO;
    public ProtectionShieldSO protectionShieldSO;





    /*
    public ParticleSystem effect;
    public int arrowRainDamage;
    public float arrowRainRadius;
    public LayerMask enemy;


    public void ProtectionShield(Vector3 originPosition)
    {
        ParticleSystem arrowRainEffect = Instantiate(effect);
        arrowRainEffect.transform.position = originPosition;
        arrowRainEffect.Play();
        
    }

    public void UseArrowRain(Vector3 originPosition)
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
    */
}
