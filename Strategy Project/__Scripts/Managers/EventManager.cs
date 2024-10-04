using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EventManager : MonoBehaviour
{
    public static Action<StructureDataSO> OnBuildingCopySelectPhaseStarted;
    public static Action OnBuildingPlaced;

    public static Action<Building> OnAnyBuildingSelected;

    public static Action<Unit> OnAnyUnitSelected;

    public static Action<Worker> OnWorkerSelected;

    public static Action OnResourcesUpdated;

}
