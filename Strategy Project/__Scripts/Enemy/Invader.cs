using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invader : Enemy
{
    protected Core core;
    [SerializeField] private ParticleSystem hitEffect;
    public void DealDamageToCore()
    {
        core.TakeDamage(enemyDataSO.damage);
    }
    private void OnDestroy()
    {
        WaveManager.Instance.aliveInvaderCount--;
        if (WaveManager.Instance.aliveInvaderCount == 0  && WaveManager.Instance.currentWave.isSpawned )
        {
            UiManager.Instance.OpenWaveClearedText();
        }
    }
    public void PlayHitEffect()
    {
        hitEffect.Play();
    }
}
