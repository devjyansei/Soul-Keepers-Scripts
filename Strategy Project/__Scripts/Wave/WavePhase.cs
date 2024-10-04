using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class WavePhase
{
    

    [Tooltip("Minute")]    [Range(0, 15)]    [InspectorName("Preparation Time in Minutes")]
    public float prepareTime;


    //[Range(0, 15)]
    //public float delayForNextPhase;
    public Vector3 spawnPoint;
    public bool isSpawned = false;

    public List<GameObject> invaderPrefabs = new List<GameObject>();
}
