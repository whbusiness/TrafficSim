using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidSection : MonoBehaviour
{
    public List<GameObject> carsTurningInProgress = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            if (other.gameObject.GetComponent<CarController>().trafficLightsRight)
            {
                carsTurningInProgress.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            if (other.gameObject.GetComponent<CarController>().trafficLightsRight)
            {
                carsTurningInProgress.Remove(other.gameObject);
            }
        }
    }


}
