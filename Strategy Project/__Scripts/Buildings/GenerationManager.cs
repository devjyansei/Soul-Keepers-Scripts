using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class GenerationManager : MonoSingleton<GenerationManager>
{

    public List<GenerationSlot> slotsList = new List<GenerationSlot>();

    public bool IsAnyGenerationSlotEmpty()
    {
        for (int i = 0; i < slotsList.Count; i++)
        {
            if (slotsList[i].isFull == false)
            {
                return true;
            }
        }
        return false;
    }

    public void UseGeneratareSlot(Action eventarg,int woodCost,int stoneCost,int goldCost,Sprite sprite,float generateSpeed)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GenerationSlot slot = slotsList[i];

            if (slot.isFull == false)
            {
                slot.isFull = true;
                slot.StartGenerate(eventarg,generateSpeed);
                slot.SetStoredResources(woodCost, stoneCost, goldCost);
                slot.transform.GetChild(0).GetComponent<Image>().sprite = sprite; // image

                return;
            }
        }
    }
}
