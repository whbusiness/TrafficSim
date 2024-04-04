using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSpeedOfOvertakingCar : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Car") && transform.root.GetComponent<CarController>().isMoreThan1Lane && other.gameObject.GetComponent<CarController>().moveToOtherLane 
            || other.gameObject.CompareTag("Car") && transform.root.GetComponent<CarController>().isMoreThan1Lane && other.gameObject.GetComponent<CarController>().rotateBack)
        {
            print("Slow Down");
            print("This rot " + transform.root.transform.eulerAngles.y);
            transform.root.GetComponent<Rigidbody>().velocity *= .3f;
        }
    }
}
