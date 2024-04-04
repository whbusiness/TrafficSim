using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingCollision : MonoBehaviour
{
    public int amountOfCars;
    public GameObject carWithin;
    private void Start()
    {
        amountOfCars = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Car"))
        {
            amountOfCars++;
            carWithin = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            amountOfCars--;
            carWithin = null;
        }
    }
}
