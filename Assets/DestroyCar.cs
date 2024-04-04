using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCar : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            print("DestroyRoundaboutCar");
            if(other.transform.GetComponentInChildren<Camera>() != null)
            {
                other.transform.GetComponentInChildren<Camera>().transform.SetParent(null);
            }
            Destroy(other.gameObject);
        }
    }
}
