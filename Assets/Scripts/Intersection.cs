using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    public int amountOfCars;
    public bool pause = true;

    private void Start()
    {
        amountOfCars = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            amountOfCars++;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            amountOfCars--;
        }
    }

}
