using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfEmpty : MonoBehaviour
{
    public bool isEmpty = true;

    private void Start()
    {
        isEmpty = true;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            isEmpty = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            isEmpty = true;
        }
    }
}
