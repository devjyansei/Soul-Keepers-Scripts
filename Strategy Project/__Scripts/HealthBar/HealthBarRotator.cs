using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//in healthbar canvas
public class HealthBarRotator : MonoBehaviour
{
    private void Update()
    {
        //transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                                                               Camera.main.transform.rotation * Vector3.up);
    }
}
