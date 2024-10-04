using UnityEngine;

public interface ICollectable 
{
    public void Collect(float productivityMultiplier);
    public void DecreseHitPoint(Worker worker,float productivityMultiplier);
    public float GetCurrentHitPoint();
    


}
