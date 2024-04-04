using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToSingleLane : MonoBehaviour
{
    public int amountOfCars = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            if (other.gameObject.GetComponent<CarController>().moveToOtherLane || other.gameObject.GetComponent<CarController>()
                .isOnSecondLane)
            {
                amountOfCars++;
                other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.gameObject.GetComponent<CarController>().moveBackToSingleLane = true;
                print("Stop");
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            amountOfCars--;
            if(transform.root.GetComponentInChildren<StopCarsMovingBackIntoLane>().carForcedToStop != null)
            {
                transform.root.GetComponentInChildren<StopCarsMovingBackIntoLane>().carForcedToStop.GetComponent<CarController>().isTrafficLightRed = false;
            }
        }
    }
}
