using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
public class WaveManager : MonoSingleton<WaveManager>
{
    public Wave currentWave;
    public int aliveInvaderCount = 0;

    private float elapsedTime = 0f;
    [SerializeField] public List<Wave> waveList = new List<Wave>();

    [SerializeField] ParticleSystem portalEffect;
    private List<ParticleSystem> portalEffectsList = new List<ParticleSystem>();


    
    private void Start()
    {
        StartCoroutine(Countdown());
        
    }





    private IEnumerator Countdown()
    {

        for (int i = 0; i < waveList.Count; i++)
        {
            

             currentWave = waveList[i];

            if (currentWave.isSpawned == false)
            {
                // WAVE ICIN GERI SAYIM
                float wavePrepareTime = currentWave.prepareTime;
                while (elapsedTime < wavePrepareTime)
                {
                    // OnWaveSpawned Invoke here
                    UiManager.Instance.UpdateCountdownText(wavePrepareTime - elapsedTime);
                    //Debug.Log("wave için kalan zaman : " + (wavePrepareTime - elapsedTime));
                    yield return new WaitForSeconds(1f); // per sec
                }

                //WAVE BASLADIKTAN SONRA 
                UiManager.Instance.OpenWaveIsComingText();
                

                for (int j = 0; j < currentWave.wavePhasesList.Count; j++)
                {
                    float phasePrepareTime = currentWave.wavePhasesList[j].prepareTime;
                    yield return new WaitForSeconds(phasePrepareTime);
                    OpenPortalEffect(currentWave.wavePhasesList[j].spawnPoint);
                    yield return new WaitForSeconds(2f);

                    SpawnPhase(currentWave.wavePhasesList[j]);
                    currentWave.wavePhasesList[j].isSpawned = true;                  
                }
                currentWave.isSpawned = true;
                elapsedTime = 0f;
            }
            yield return new WaitForSeconds(currentWave.totalWaveClearTime);

            ClosePortalEffect();

            if (i == waveList.Count-1)
            {
                yield return new WaitForSeconds(currentWave.totalWaveClearTime);
                if (!GameManager.Instance.IsGameEnded())
                {
                    GameManager.Instance.Victory();
                }
                
            }
            
        }
        
        
    }



    private void SpawnPhase(WavePhase phase)
    {
        for (int i = 0; i < phase.invaderPrefabs.Count; i++)
        {
            
            

            GameObject invaderObj = Instantiate(phase.invaderPrefabs[i]);            

            Invader invader = invaderObj.GetComponent<Invader>();

            Vector3 randomPosAroundCenter = new Vector3(phase.spawnPoint.x + UnityEngine.Random.Range(-3, 3), 0, phase.spawnPoint.z + UnityEngine.Random.Range(-5, 5));

            invader.GetComponent<NavMeshAgent>().Warp(randomPosAroundCenter);
            invader.GetComponent<NavMeshAgent>().avoidancePriority = UnityEngine.Random.Range(0, 100);

            aliveInvaderCount++;
        }
    }


   public void OpenPortalEffect(Vector3 spawnPos)
    {
        ParticleSystem tempPortalEffect = Instantiate(portalEffect, spawnPos+Vector3.up*5, Quaternion.Euler(0,0,90));
        portalEffectsList.Add(tempPortalEffect);
    }

    public void ClosePortalEffect()
    {
        foreach (ParticleSystem effect in portalEffectsList)
        {
            Destroy(effect.gameObject);
        }
        portalEffectsList.Clear();
    }

   
    private void Update()
    {
        elapsedTime += Time.deltaTime;
    }




    
}
