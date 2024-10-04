using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class Wave
{
    [Tooltip("Second")]    [Range(0, 900)]    [InspectorName("Preparation Time in Minutes")]
    public float prepareTime;
    public float totalWaveClearTime;
   
    public bool isSpawned = false;


    public List<WavePhase> wavePhasesList = new List<WavePhase>();
}
