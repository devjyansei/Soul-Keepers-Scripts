using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class WindmillRotator : MonoBehaviour
{
    void Start()
    {
        
        transform.DORotate(new Vector3(360, 0, 0), 2f, RotateMode.LocalAxisAdd)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }
}
