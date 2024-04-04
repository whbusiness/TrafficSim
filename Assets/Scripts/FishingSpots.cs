using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingSpots : MonoBehaviour
{
    public bool isOccupied = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Citizen"))
        {
            isOccupied = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Citizen"))
        {
            isOccupied = false;
        }
    }
}
