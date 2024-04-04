using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleLane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Car") && other.gameObject.GetComponent<CarController>().isMoreThan1Lane)
        {
            other.gameObject.GetComponent<CarController>().isMoreThan1Lane = false;
        }
    }
}
