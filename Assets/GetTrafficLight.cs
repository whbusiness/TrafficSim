using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTrafficLight : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            other.gameObject.GetComponent<CarController>().isInTrafficLightArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            other.gameObject.GetComponent<CarController>().isInTrafficLightArea = false;
        }
    }
}
