using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ResourceBase : MonoBehaviour,ICollectable
{
    [SerializeField] public ResourceDataSO resourceDataSO;
    [SerializeField] private float minimumScale;
    [SerializeField] private float maximumScale;
    protected float currentHitPoint;
    public virtual void Collect(float productivityMultiplier)
    {
        
    }
    public virtual void Produce(float productValue)
    {
        
    }
    public virtual void DecreseHitPoint(Worker worker,float productivityMultiplier)
    {
        
    }

    
    float ICollectable.GetCurrentHitPoint()
    {
        return currentHitPoint;
    }

    protected void SetRandomScale()
    {
        float randomScale = UnityEngine.Random.Range(minimumScale, maximumScale);

        transform.localScale *= randomScale;
    }




}
