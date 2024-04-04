using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectSignal : MonoBehaviour
{
    public List<GameObject> carsInArea = new();
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            if (other.gameObject.GetComponent<CarController>().isOnRoundabout)
            {
                carsInArea.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            if (carsInArea.Contains(other.gameObject))
            {
                carsInArea.Remove(other.gameObject);
            }
        }
    }
}
