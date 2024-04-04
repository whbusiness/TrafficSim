using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopCarsMovingBackIntoLane : MonoBehaviour
{
    public static bool canMoveBack = false;
    public GameObject carForcedToStop;

    private void Start()
    {
        canMoveBack = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car") && canMoveBack)
        {
            canMoveBack = false;
        }
        if(other.gameObject.CompareTag("Car") && transform.root.GetComponentInChildren<MoveToSingleLane>().amountOfCars > 0)
        {
            other.gameObject.GetComponent<CarController>().isTrafficLightRed = true;
            carForcedToStop = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Car") && !canMoveBack)
        {
            canMoveBack = true;
        }
    }
}
