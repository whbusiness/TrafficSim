using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectOtherCarsBeforeOvertake : MonoBehaviour
{
    public int carsWithinDetection;

    private void Start()
    {
        carsWithinDetection = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car") && transform.root.GetComponent<CarController>().isMoreThan1Lane && gameObject != other.gameObject)
        {
            carsWithinDetection++;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Car") && transform.root.GetComponent<CarController>().isMoreThan1Lane && gameObject != other.gameObject)
        {
            carsWithinDetection--;
        }
    }
}
