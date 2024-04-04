using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Light
{
    RED,
    AMBER,
    GREEN
}

public enum Direction
{
    FORWARD,
    LEFT,
    RIGHT
}

public class TrafficLights : MonoBehaviour
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
    private Intersection intersection;
    public int turningRight, turningLeft, goingForward;
    private void Awake()
    {
        state = startingState;
    }
    // Start is called before the first frame update
    void Start()
    {
        if(state == Light.RED)
        {
            lightObject.localPosition = redPos;
        }
        else if(state == Light.AMBER)
        {
            lightObject.localPosition = amberPos;
        }
        else if(state == Light.GREEN)
        {
            lightObject.localPosition = greenPos;
        }
        intersection = FindObjectOfType<Intersection>();
        InvokeRepeating(nameof(ChangeLight), 1, 1);
    }

    private void Update()
    {
        if(amountTurningLeft < 0)
        {
            print("Left is Negative");
        }
        if(amountTurningRight < 0)
        {
            print("Right is Negative");
        }
        if(amountGoingForward < 0)
        {
            print("Forward Is Negative");
        }
        turningRight = amountTurningRight;
        turningLeft = amountTurningLeft;
        goingForward = amountGoingForward;
    }

    void ChangeLight()
    {
        switch (state)
        {
            case Light.RED:
                if (howManyLoops >= 20)
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
                if(howManyLoops >= 4)
                {
                    if (prevState == Light.RED)
                    {
                        prevState = state;
                        state = Light.GREEN;
                        print("Reset");
                        intersection.amountOfCars = 0;
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
                if (howManyLoops >= 20)
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
            other.gameObject.GetComponent<CarController>().isInTrafficLightArea = true;
            if(state == Light.RED && other.GetComponent<CarController>().canMove)
            {
                print("Stop Car Moving");
                other.GetComponent<CarController>().canMove = false;
                other.GetComponent<CarController>().isTrafficLightRed = true;
                if (!other.gameObject.GetComponent<CarController>().getDir)
                {
                    if (other.gameObject.GetComponent<CarController>().trafficLightsLeft)
                    {
                        amountTurningLeft--;
                    }
                    if (other.gameObject.GetComponent<CarController>().trafficLightsRight)
                    {
                        amountTurningRight--;
                    }
                    if(!other.gameObject.GetComponent<CarController>().trafficLightsRight && !other.gameObject.GetComponent<CarController>().trafficLightsLeft)
                    {
                        amountGoingForward--;
                    }
                    other.gameObject.GetComponent<CarController>().trafficLightsLeft = false;
                    other.gameObject.GetComponent<CarController>().trafficLightsRight = false;
                    other.gameObject.GetComponent<CarController>().getDir = true;

                }
            }
            if(state == Light.GREEN && !other.GetComponent<CarController>().canMove)
            {
                other.GetComponent<CarController>().canMove = true;
                other.GetComponent<CarController>().isTrafficLightRed = false;
            }
            if(state == Light.AMBER && other.GetComponent<CarController>().canMove)
            {
                print("Stop Car Moving");
                other.GetComponent<CarController>().canMove = false;
                other.GetComponent<CarController>().isTrafficLightRed = true;
                if (!other.gameObject.GetComponent<CarController>().getDir)
                {
                    if (other.gameObject.GetComponent<CarController>().trafficLightsLeft)
                    {
                        amountTurningLeft--;
                    }
                    if (other.gameObject.GetComponent<CarController>().trafficLightsRight)
                    {
                        amountTurningRight--;
                    }
                    if (!other.gameObject.GetComponent<CarController>().trafficLightsRight && !other.gameObject.GetComponent<CarController>().trafficLightsLeft)
                    {
                        amountGoingForward--;
                    }
                    other.gameObject.GetComponent<CarController>().trafficLightsLeft = false;
                    other.gameObject.GetComponent<CarController>().trafficLightsRight = false;
                    other.gameObject.GetComponent<CarController>().getDir = true;

                }
            }
            if (other.gameObject.GetComponent<CarController>().getDir && state == Light.GREEN)
            {
                dir = Random.Range((int)Direction.FORWARD, (int)Direction.RIGHT+1);
                print("Car Has Reached Traffic Lights THEY WILL MOVE :  " + dir);
                switch (dir)
                {
                    case (int)Direction.FORWARD:
                        print("Moving Forward");
                        amountGoingForward++;
                        other.gameObject.GetComponent<CarController>().getDir = false;
                        break;
                    case (int)Direction.LEFT:
                        amountTurningLeft++;
                        other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        other.gameObject.GetComponent<CarController>().rotIncrement = 0;
                        other.gameObject.GetComponent<CarController>().getCurrentRot = other.gameObject.transform.eulerAngles.y;
                        other.gameObject.GetComponent<CarController>().trafficLightsLeft = true;
                        other.gameObject.GetComponent<CarController>().getDir = false;
                        break;
                    case (int)Direction.RIGHT:
                        amountTurningRight++;
                        carsTurningRight.Add(other.gameObject);
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
