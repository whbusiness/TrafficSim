using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianCrossing : MonoBehaviour
{
    public int amountOfCitizensCrossing;

    private void Start()
    {
        amountOfCitizensCrossing = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Citizen"))
        {
            amountOfCitizensCrossing++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Citizen"))
        {
            amountOfCitizensCrossing--;
        }
    }
}
