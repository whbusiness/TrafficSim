using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallerTrafficLights : MonoBehaviour
{
    public Light state, startingState;
    private Light prevState;
    [SerializeField]
    private Transform lightObject;
    [SerializeField]
    private Vector3 startingPos, redPos, amberPos, greenPos;
    private int howManyLoops;
    private int dir;
    private bool ignoreReset = false;
    public static int amountTurningRight, amountTurningLeft, amountGoingForward;
    public static List<GameObject> carsTurningRight = new();
    private void Awake()
    {
        state = startingState;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (state == Light.RED)
        {
            lightObject.localPosition = redPos;
        }
        else if (state == Light.AMBER)
        {
            lightObject.localPosition = amberPos;
        }
        else if (state == Light.GREEN)
        {
            lightObject.localPosition = greenPos;
        }

        InvokeRepeating(nameof(ChangeLight), 1, 1);
    }

    void ChangeLight()
    {
        switch (state)
        {
            case Light.RED:
                if (howManyLoops >= 10)
                {
                    prevState = state;
                    state = Light.AMBER;
                    lightObject.localPosition = amberPos;
                    howManyLoops = 0;
                }
                else
                {
                    howManyLoops++;
                }
                break;
            case Light.AMBER:
                if (howManyLoops >= 4)
                {
                    if (prevState == Light.RED)
                    {
                        prevState = state;
                        state = Light.GREEN;
                        lightObject.localPosition = greenPos;
                    }
                    else if (prevState == Light.GREEN)
                    {
                        prevState = state;
                        state = Light.RED;
                        lightObject.localPosition = redPos;
                    }
                    howManyLoops = 0;
                }
                else
                {
                    howManyLoops++;
                }
                break;
            case Light.GREEN:
                if (howManyLoops >= 10)
                {
                    prevState = state;
                    state = Light.AMBER;
                    lightObject.localPosition = amberPos;
                    howManyLoops = 0;
                }
                else
                {
                    howManyLoops++;
                }
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            if (state == Light.RED && other.GetComponent<CarController>().canMove)
            {
                other.GetComponent<CarController>().canMove = false;
                other.GetComponent<CarController>().isTrafficLightRed = true;
            }
            if(state == Light.AMBER && !other.GetComponent<CarController>().trafficLightsLeft && !other.GetComponent<CarController>().trafficLightsRight)
            {
                other.GetComponent<CarController>().canMove = false;
                other.GetComponent<CarController>().isTrafficLightRed = true;
            }
            if (state == Light.GREEN && !other.GetComponent<CarController>().canMove)
            {
                other.GetComponent<CarController>().canMove = true;
                other.GetComponent<CarController>().isTrafficLightRed = false;
            }
            if (other.gameObject.GetComponent<CarController>().getDir && state == Light.GREEN && other.gameObject.transform.localEulerAngles.y == 180)
            {
                dir = Random.Range((int)Direction.LEFT, (int)Direction.RIGHT + 1);
                print("Car Has Reached Traffic Lights THEY WILL MOVE :  " + dir);
                switch (dir)
                {
                    case (int)Direction.LEFT:
                        other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        other.gameObject.GetComponent<CarController>().rotIncrement = 0;
                        other.gameObject.GetComponent<CarController>().getCurrentRot = other.gameObject.transform.eulerAngles.y;
                        other.gameObject.GetComponent<CarController>().trafficLightsLeft = true;
                        other.gameObject.GetComponent<CarController>().getDir = false;
                        break;
                    case (int)Direction.RIGHT:
                        other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        other.gameObject.GetComponent<CarController>().rotIncrement = 0;
                        other.gameObject.GetComponent<CarController>().getCurrentRot = other.gameObject.transform.eulerAngles.y;
                        other.gameObject.GetComponent<CarController>().trafficLightsRight = true;
                        other.gameObject.GetComponent<CarController>().getDir = false;
                        break;
                }
            }
        }
    }
}