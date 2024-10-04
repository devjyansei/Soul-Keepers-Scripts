using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SpecialAttack/ProtectionShield")]

public class ProtectionShieldSO : SpecialAttackSO
{
    public ParticleSystem effect;

    public void UseSpecialAttack(Vector3 originPosition)
    {
        ParticleSystem shieldEffect = Instantiate(effect);
        shieldEffect.transform.position = originPosition;
        shieldEffect.Play();
    }
}
