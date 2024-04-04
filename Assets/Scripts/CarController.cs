using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum RoundaboutTurn
{
    RoundaboutLeft,
    RoundaboutStraight,
    RoundaboutRight
}

public class CarController : MonoBehaviour
{
    public float speed, aroundCarSpeed;
    [SerializeField]
    private float maxDistanceDelta, rotTime, radius;
    public float originalSpeed;
    private Rigidbody _rb;
    public bool canMove = true;
    public bool isParking = false;
    public bool isTrafficLightRed = false;
    [SerializeField]
    private float dist = Mathf.Infinity;
    [SerializeField]
    private GameObject targetParkingSpace;
    [SerializeField]
    private Vector3 targetParkingSpacePos;
    [SerializeField]
    private List<Collider> colliders = new();
    public bool wantsToPark = false, moveForward, goAround;
    private List<CarController> otherCars = new();
    public float maxDist;
    [SerializeField]
    private Vector3 offset;
    public GameObject carToGoAround;
    [SerializeField]
    private Collider carToGoAroundCollider, thisCarCollider;
    public float thisCarX, thatCarX, rightMovementOffset, forwardMovementOffset;
    private bool hasReachedRightAroundCar, hasReachedForwardAroundCar, hasReachedLeftAroundCar, completedMovingAroundCar;
    public bool hasStopped;
    private bool movingRight, movingLeft;
    private Quaternion currentRot;
    [SerializeField]
    private float rotSpeed, rotSpeedRight, rotSpeedLeft;
    public float rotIncrement;
    [SerializeField]
    private bool turnRight, turnLeft, forwardMovement;
    [SerializeField]
    private float timer, aroundCarRightTime, aroundCarLeftTime, forwardMovementTime, returnLeftTime, returnRightTime;
    [SerializeField]
    private float t, crossingRadius;
    public float getCurrentRot, rotationBeforeRoundabout;
    [Header("Traffic Lights Direction")]
    public bool trafficLightsLeft, trafficLightsRight;
    public float trafficLightsTime;
    [SerializeField]
    private float trafficLightsLeftTimer, trafficLightsRightTimer, waitLeftTime, waitRightTime;
    private float timeWithinTrafficLightTrigger;
    [SerializeField]
    private Transform carSpherecastPoint;
    [SerializeField]
    private float checkForTurningCarsRadius, checkForCarBehindRadius;
    public bool stopTurningCounters;
    public Transform currentRoadSystem;
    [SerializeField]
    private float timeToRotate;
    [SerializeField]
    private Transform leftRoadCheck, rightRoadCheck, checkInfront, checkRoundaboutRoad;
    public bool hasRoadRot;
    [SerializeField]
    private GameObject interSection;
    public bool hasPriority;
    public GameObject carInfrontFound, carBehindFound, carBehindCheckPos;
    public bool carIsTurning;
    public bool runOnce;
    public Collider[] collidersFound;
    public bool isAlreadyWithin;
    public bool getDir;
    public GameObject checkIfCarIsInProgressOfTurning;
    public bool hasTurned, isOnRoundabout;
    public bool roundaboutLeft, roundaboutRight, roundaboutForward, getRoundaboutDirection;
    private bool hasAlreadyJoinedRoundabout, alreadyHasSignalDetector;
    public bool signalLeft, signalRight;
    public GameObject detectSignal;
    public bool goAtRoundabout, moveToOtherLane, rotateBack, isMoreThan1Lane, isOnSecondLane, moveBackToSingleLane, moveToSingleLane, rotToOriginal;
    private int detectSignalLayer;
    private Quaternion targetRot, rotBeforeChangingLanes;
    public Transform raycastOrigin;
    [SerializeField]
    private Transform inLineOfParkingSpot;
    [SerializeField]
    private float parkingSpaceRot;
    private List<Collider> listOfColliders = new();
    private bool stopTick = false;
    private float carDist = Mathf.Infinity;
    [SerializeField]
    private GameObject closestCarAtRoundabout;
    private bool runOnlyOnce;
    private PauseScript isPaused;
    private float timerForRoundaboutRot;
    public bool isInTrafficLightArea = false, isOtherCarSignallingLeftAtRoundabout = false;
    void Start()
    {
        hasPriority = true;
        moveForward = true;
        hasRoadRot = false;
        goAround = false;
        carIsTurning = false;
        hasReachedForwardAroundCar = false;
        hasReachedRightAroundCar = false;
        isAlreadyWithin = false;
        hasTurned = false;
        isOnRoundabout = false;
        getDir = true;
        dist = Mathf.Infinity;
        canMove = true;
        hasAlreadyJoinedRoundabout = false;
        goAtRoundabout = true;
        signalLeft = false;
        signalRight = false;
        moveToOtherLane = false;
        moveToSingleLane = false;
        alreadyHasSignalDetector = false;
        isMoreThan1Lane = false;
        runOnlyOnce = false;
        originalSpeed = speed;
        stopTick = false;
        isOtherCarSignallingLeftAtRoundabout = false;
        detectSignalLayer = 1 << 10;
        _rb = GetComponent<Rigidbody>();
        thisCarCollider = GetComponent<Collider>();
        rotSpeedRight = rotSpeed;
        getCurrentRot = transform.localEulerAngles.y;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).CompareTag("CarSphereCastPos"))
            {
                carSpherecastPoint = transform.GetChild(i);
            }
        }
        var col = Physics.OverlapSphere(transform.position, 2);
        foreach(var c in col)
        {
            if (c.transform.root.CompareTag("2Lane") && !isMoreThan1Lane)
            {
                print("Found 2 lane");
                isMoreThan1Lane = true;
            }
        }
        timer = 0;
        timeToRotate = 3;
    }

    private void LateUpdate()
    {
        if (!stopTick)
        {
            if (transform.localEulerAngles.y > 179 && transform.localEulerAngles.y <= 181 && offset.z != -1 && offset.x != 0)
            {
                offset.x = 0;
                offset.z = -1;
            }
            else if (transform.localEulerAngles.y >= 0 && transform.localEulerAngles.y < 1 && offset.z != 1 && offset.x != 0 || transform.localEulerAngles.y > 359 && transform.localEulerAngles.y <= 360 && offset.z != 1 && offset.x != 0)
            {
                offset.x = 0;
                offset.z = 1;
            }
            else if (transform.localEulerAngles.y > 269 && transform.localEulerAngles.y <= 271 && offset.z != 0 && offset.x != -1)
            {
                offset.x = -1;
                offset.z = 0;
            }
            else if (transform.localEulerAngles.y > 89 && transform.localEulerAngles.y <= 91 && offset.z != 0 && offset.x != 1)
            {
                offset.x = 1;
                offset.z = 0;
            }


            if (currentRoadSystem != null && transform.localEulerAngles != currentRoadSystem.transform.localEulerAngles)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, currentRoadSystem.transform.localRotation, timeToRotate * Time.deltaTime);
            }

            if (moveToSingleLane)
            {
                _rb.velocity = 100 * Time.fixedDeltaTime * transform.forward;

                if (transform.localEulerAngles.y > targetRot.eulerAngles.y - 1 && transform.localEulerAngles.y < targetRot.eulerAngles.y + 1)
                {
                    _rb.velocity = Vector3.zero;
                    rotToOriginal = true;
                    moveToSingleLane = false;
                }
                else
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 20 * Time.deltaTime);
                }

            }
            if (rotToOriginal)
            {
                _rb.velocity = 100 * Time.fixedDeltaTime * transform.forward;
                print(rotBeforeChangingLanes.eulerAngles.y);
                if (transform.localEulerAngles.y > rotBeforeChangingLanes.eulerAngles.y - 1 && transform.localEulerAngles.y < rotBeforeChangingLanes.eulerAngles.y + 1)
                {
                    transform.rotation = rotBeforeChangingLanes;
                    moveBackToSingleLane = false;
                    rotToOriginal = false;
                    isOnSecondLane = false;
                }
                else
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotBeforeChangingLanes, 20 * Time.deltaTime);
                }
            }
            if (moveToOtherLane && GetComponentInChildren<DetectOtherCarsBeforeOvertake>().carsWithinDetection == 0)
            {
                print("Move To Other Lane");
                _rb.velocity = 100 * Time.fixedDeltaTime * transform.forward;
                print(targetRot.eulerAngles.y);

                if (transform.localEulerAngles.y > targetRot.eulerAngles.y - 1 && transform.localEulerAngles.y < targetRot.eulerAngles.y + 1)
                {
                    _rb.velocity = Vector3.zero;
                    rotateBack = true;
                    moveToOtherLane = false;
                }
                else
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 30 * Time.deltaTime);
                }

            }
            if (rotateBack)
            {
                _rb.velocity = 100 * Time.fixedDeltaTime * transform.forward;
                if (transform.localEulerAngles.y > rotBeforeChangingLanes.eulerAngles.y - 1 && transform.localEulerAngles.y < rotBeforeChangingLanes.eulerAngles.y + 1)
                {
                    transform.rotation = rotBeforeChangingLanes;
                    isOnSecondLane = true;
                    print(transform.eulerAngles.y + " Finished Rot");
                    rotateBack = false;
                }
                else
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotBeforeChangingLanes, 30 * Time.deltaTime);
                }
            }

        }

    }

    private void Update()
    {
        Debug.DrawLine(leftRoadCheck.position, leftRoadCheck.position - transform.up * 2, Color.red);
        Debug.DrawLine(rightRoadCheck.position, rightRoadCheck.position - transform.up * 2, Color.red);
        if (!stopTick)
        {

            if(isOnRoundabout && currentRoadSystem == null)
            {
                timerForRoundaboutRot += Time.deltaTime;
                if(timerForRoundaboutRot >= 2)
                {
                    print("Not Got Roundabout Rotation");
                    if (transform.GetComponentInChildren<Camera>() != null)
                    {
                        transform.GetComponentInChildren<Camera>().transform.SetParent(null);
                    }
                    Destroy(gameObject);
                }
            }

            if (rotationBeforeRoundabout > 90 && rotationBeforeRoundabout < 91)
            {
                rotationBeforeRoundabout = 90;
            }

            if (detectSignal != null)
            {
                if (!detectSignal.GetComponent<DetectSignal>().carsInArea.Contains(closestCarAtRoundabout))
                {
                    closestCarAtRoundabout = null;
                }


                if (detectSignal.GetComponent<DetectSignal>().carsInArea.Count > 0)
                {
                    carDist = Mathf.Infinity;
                    foreach (var cars in detectSignal.GetComponent<DetectSignal>().carsInArea)
                    {
                        if(cars != null)
                        {
                            float getDist = (transform.position - cars.transform.position).sqrMagnitude;
                            if (getDist < carDist)
                            {
                                closestCarAtRoundabout = cars;
                                carDist = getDist;
                            }
                        }
                    }
                }
                else
                {
                    goAtRoundabout = true;
                }

                if (closestCarAtRoundabout != null)
                {
                    if (closestCarAtRoundabout.GetComponent<CarController>().signalLeft)
                    {
                        isOtherCarSignallingLeftAtRoundabout = true;
                        goAtRoundabout = true;
                    }
                    else
                    {
                        isOtherCarSignallingLeftAtRoundabout = false;
                        goAtRoundabout = false;
                    }
                }
                else
                {
                    goAtRoundabout = true;
                }
            }

            

            if (transform.eulerAngles.y < .2f && transform.eulerAngles.y != 0 && !isOnRoundabout)
            {
                print("Update euler");
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
            }

            Debug.DrawLine(transform.position + new Vector3(0, .1f, 0), transform.position + new Vector3(0, .1f, 0) - transform.up * 2, Color.white);
            if (interSection != null)
            {
                if (interSection.TryGetComponent<Intersection>(out Intersection intersecScript))
                {
                    if (!trafficLightsRight && intersecScript.amountOfCars < 2 || TrafficLights.amountTurningLeft >= 1 && trafficLightsLeft && intersecScript.amountOfCars < 2)
                    {
                        hasPriority = true;
                    }
                    if (trafficLightsRight && TrafficLights.amountTurningRight == 2 && checkIfCarIsInProgressOfTurning != null && checkIfCarIsInProgressOfTurning.GetComponent<MidSection>().carsTurningInProgress.Count < 1)
                    {
                        hasPriority = true;
                    }
                    if (trafficLightsRight && TrafficLights.amountTurningRight == 1 && TrafficLights.amountTurningLeft == 0 && TrafficLights.amountGoingForward == 0)
                    {
                        hasPriority = true;
                    }
                    if (trafficLightsRight && TrafficLights.amountTurningLeft != 0 && TrafficLights.amountTurningRight < 2 && !isAlreadyWithin || trafficLightsRight && TrafficLights.amountGoingForward != 0 && TrafficLights.amountTurningRight < 2 && !isAlreadyWithin)
                    {
                        _rb.velocity = Vector3.zero;
                        hasPriority = false;
                    }
                    if (!trafficLightsLeft && !trafficLightsRight && intersecScript.amountOfCars < 2 || TrafficLights.amountGoingForward == 2 && !trafficLightsLeft && !trafficLightsRight && intersecScript.amountOfCars <= 2)
                    {
                        hasPriority = true;
                    }
                }
            }

            if (getRoundaboutDirection)
            {
                var randint = Random.Range((int)RoundaboutTurn.RoundaboutLeft, (int)RoundaboutTurn.RoundaboutRight + 1);
                switch (randint)
                {
                    case (int)RoundaboutTurn.RoundaboutLeft:
                        roundaboutLeft = true;
                        break;
                    case (int)RoundaboutTurn.RoundaboutStraight:
                        roundaboutForward = true;
                        break;
                    case (int)RoundaboutTurn.RoundaboutRight:
                        roundaboutRight = true;
                        break;
                }

                if (roundaboutRight)
                {
                    signalRight = true;
                }
                else if (roundaboutLeft)
                {
                    signalLeft = true;
                }
                else
                {
                    signalLeft = false;
                    signalRight = false;
                }

                getRoundaboutDirection = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (!stopTick)
        {
            Debug.DrawLine(transform.position + offset, transform.position + offset + transform.forward * maxDist, Color.blue);
            //Debug.DrawLine(transform.position, transform.position + new Vector3(0,1,0) - transform.up * 2, Color.red);
            if (canMove)
            {
                CarMovement();
            }

            if (isTrafficLightRed || !hasPriority || !goAtRoundabout)
            {
                _rb.velocity = Vector3.zero;
            }

            if (isOnRoundabout && !hasRoadRot && goAtRoundabout)
            {
                var colliders = Physics.OverlapSphere(checkRoundaboutRoad.position, .1f);
                foreach (var c in colliders)
                {
                    if (c.gameObject.CompareTag("RoundaboutRoad"))
                    {
                        currentRoadSystem = c.gameObject.transform.parent;
                    }
                }
                _rb.velocity = 100 * Time.fixedDeltaTime * transform.forward;
            }

            if (isOnRoundabout && hasRoadRot)
            {
                _rb.velocity = 70 * Time.fixedDeltaTime * transform.forward;
            }

            /*if (!stopTurningCounters)
            {
                if (trafficLightsLeft && !isTrafficLightRed)
                {
                    trafficLightsTime += Time.deltaTime;
                    if (trafficLightsTime > waitLeftTime && trafficLightsTime <= trafficLightsLeftTimer)
                    {
                        rotIncrement += Time.deltaTime * rotSpeed * 3;
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.x, getCurrentRot - rotIncrement, transform.rotation.z), t);
                    }
                    else if (trafficLightsTime > trafficLightsLeftTimer)
                    {
                        _rb.velocity = Vector3.zero;
                        trafficLightsLeft = false;
                        trafficLightsTime = 0;
                    }

                    _rb.AddForce(15 * Time.fixedDeltaTime * transform.forward, ForceMode.Force);
                }
                else if (trafficLightsRight && !isTrafficLightRed)
                {
                    trafficLightsTime += Time.deltaTime;
                    if (trafficLightsTime > waitRightTime && trafficLightsTime <= trafficLightsRightTimer)
                    {
                        rotIncrement += Time.deltaTime * rotSpeed * 3;
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.x, getCurrentRot + rotIncrement, transform.rotation.z), t);
                    }
                    else if (trafficLightsTime > trafficLightsRightTimer)
                    {
                        _rb.velocity = Vector3.zero;
                        trafficLightsRight = false;
                        trafficLightsTime = 0;
                    }

                    _rb.AddForce(15 * Time.fixedDeltaTime * transform.forward, ForceMode.Force);
                }
            }*/
            if (!isTrafficLightRed && trafficLightsLeft && hasPriority && !isOnRoundabout && !moveBackToSingleLane || !isTrafficLightRed && trafficLightsRight && hasPriority && !isOnRoundabout && !moveBackToSingleLane)
            {
                _rb.velocity = 70 * Time.fixedDeltaTime * transform.forward;
            }

            if (hasRoadRot && !trafficLightsLeft && !trafficLightsRight && !isOnRoundabout)
            {
                hasRoadRot = false;
            }

            if (StopCarsMovingBackIntoLane.canMoveBack && moveBackToSingleLane)
            {
                ReturnToSingleLane();
            }

            if (isParking)
            {
                if (canMove)
                {
                    _rb.velocity = Vector3.zero;
                    dist = Mathf.Infinity;
                    canMove = false;
                }

                if (targetParkingSpace != null)
                {
                    if (targetParkingSpace.GetComponent<ParkingCollision>().amountOfCars > 0
                            && targetParkingSpace.GetComponent<ParkingCollision>().carWithin != gameObject)
                        {
                        // print(gameObject.name + " Chose Parking Space With Other Car In");
                            var parkingSpacesFound = Physics.OverlapSphere(transform.position, 15);
                            foreach (var go in parkingSpacesFound)
                            {
                                if (go.gameObject.CompareTag("ParkingSpace") && go.isTrigger)
                                {
                                    if (go.gameObject.GetComponent<ParkingCollision>().amountOfCars < 1)
                                    {
                                        float getDist = (transform.position - go.transform.position).sqrMagnitude;
                                        if (getDist < dist)
                                        {
                                            targetParkingSpacePos = go.transform.position;
                                            targetParkingSpace = go.gameObject;
                                            parkingSpaceRot = go.gameObject.transform.localEulerAngles.y;
                                            inLineOfParkingSpot = go.transform.GetChild(0);
                                            dist = getDist;
                                        }
                                    }
                                }
                            }
                        }


                    if (inLineOfParkingSpot != null)
                    {
                        if ((inLineOfParkingSpot.position - transform.position).sqrMagnitude <= 2)
                        {
                            inLineOfParkingSpot = null;
                        }
                        else
                        {
                            transform.position = Vector3.MoveTowards(transform.position, inLineOfParkingSpot.position, maxDistanceDelta * Time.fixedDeltaTime);
                            if (inLineOfParkingSpot.CompareTag("180Parking"))
                            {
                                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.x, 180, transform.rotation.z), 2 * Time.deltaTime);
                            }
                            else
                            {
                                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.x, 90, transform.rotation.z), 2 * Time.deltaTime);
                            }
                        }
                    }
                    else
                    {
                        if ((targetParkingSpacePos - transform.position).sqrMagnitude <= .1)
                        {
                            print("Stop Update Methods");
                            stopTick = true;
                        }
                        else
                        {
                            transform.position = Vector3.MoveTowards(transform.position, targetParkingSpacePos, maxDistanceDelta * Time.fixedDeltaTime);
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.x, parkingSpaceRot, transform.rotation.z), 2 * Time.deltaTime);
                        }
                    }
                    //

                    /*Vector3 delta = (targetParkingSpacePos - transform.position).normalized;
                    Vector3 cross = Vector3.Cross(delta, transform.forward);

                    if (cross == Vector3.zero)
                    {
                        print("Straight");
                        // Target is straight ahead
                    }
                    else if (cross.y > 0)
                    {
                        print("Right");
                        // Target is to the right
                    }
                    else
                    {
                        print("Left");
                        // Target is to the left
                    }*/
                }
                else
                {
                    canMove = true;
                    wantsToPark = false;
                    isParking = false;
                }

            }

            /*if (goAround)
            {
                GoAroundCar();
                //MoveAroundParkedCar();
            }*/
            else if (!goAround && speed == aroundCarSpeed)
            {
                speed = originalSpeed;
            }

        }
    }

    void ReturnToSingleLane()
    {
        if (!moveToSingleLane && !rotToOriginal)
        {
            print("How Many Times Is This Running");
            targetRot = Quaternion.Euler(new Vector3(0, transform.localEulerAngles.y - 38, 0));
            rotBeforeChangingLanes = transform.rotation;
            moveToSingleLane = true;
        }
    }

    void CarMovement()
    {
        if (moveForward && !trafficLightsRight && !trafficLightsLeft && !isOnRoundabout && !moveToOtherLane && !rotateBack && !moveBackToSingleLane)
        {
            _rb.AddForce(speed * Time.fixedDeltaTime * transform.forward, ForceMode.Force);
            //print("Moving Forward");
            if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out RaycastHit hit, maxDist) && !carIsTurning && !moveToSingleLane)
            {
                
                /*if (hit.collider.gameObject.CompareTag("Car") && hit.collider.gameObject != gameObject && hit.collider.gameObject.GetComponent<CarController>().isParking && !hit.collider.gameObject.GetComponent<CarController>().isTrafficLightRed)
                {
                    print("Go Around");
                    carToGoAround = hit.collider.gameObject;
                    moveForward = false;
                }*/
                if(hit.collider.gameObject.CompareTag("Car") && hit.collider.gameObject != gameObject && hit.collider.gameObject.GetComponent<CarController>().isTrafficLightRed)
                {
                    //print("Car Ahead Stopped At Traffic Lights" + gameObject.name);
                    isTrafficLightRed = true;
                }
                else if(hit.collider.gameObject.CompareTag("Car") && hit.collider.gameObject != gameObject && !hit.collider.gameObject.GetComponent<CarController>().isTrafficLightRed && !carIsTurning)
                {
                    isTrafficLightRed = false;
                }
                if(hit.collider.gameObject.CompareTag("Car") && hit.collider.gameObject != gameObject && hit.collider.gameObject.GetComponent<CarController>().goAround)
                {
                    _rb.velocity = Vector3.zero;
                }
                if(hit.collider.gameObject.CompareTag("Car") && hit.collider.gameObject != gameObject && hit.collider.gameObject.GetComponent<CarController>().isParking == false && hit.collider.gameObject.GetComponent<CarController>().speed == speed && hit.collider.transform.eulerAngles.y == transform.eulerAngles.y)
                {
                    _rb.velocity = hit.collider.gameObject.GetComponent<Rigidbody>().velocity;
                }
               /* if(hit.collider.gameObject.CompareTag("Car") && hit.collider.gameObject != gameObject && hit.collider.gameObject.GetComponent<CarController>().isParking == false && hit.collider.gameObject.GetComponent<CarController>().speed != speed && hit.collider.transform.eulerAngles.y == transform.eulerAngles.y
                    && !hit.collider.gameObject.GetComponent<CarController>().isTrafficLightRed && isMoreThan1Lane)
                {
                    if (isMoreThan1Lane && GetComponentInChildren<DetectOtherCarsBeforeOvertake>().carsWithinDetection == 0)
                    {
                        targetRot = Quaternion.Euler(new Vector3(0, transform.localEulerAngles.y + 38, 0));
                        _rb.velocity = Vector3.zero;
                        rotBeforeChangingLanes = transform.rotation;
                        moveToOtherLane = true;
                    }else if(isMoreThan1Lane && GetComponentInChildren<DetectOtherCarsBeforeOvertake>().carsWithinDetection > 0)
                    {
                        _rb.velocity = hit.collider.gameObject.GetComponent<Rigidbody>().velocity;
                    }
                }*/
                if(hit.collider.gameObject.CompareTag("Car") && hit.collider.gameObject != gameObject && hit.collider.gameObject.GetComponent<CarController>().trafficLightsLeft || hit.collider.gameObject.CompareTag("Car") && hit.collider.gameObject != gameObject && hit.collider.gameObject.GetComponent<CarController>().trafficLightsRight)
                {
                    _rb.velocity = Vector3.zero;
                }
            }
            else
            {
                if (isTrafficLightRed)
                {
                    print("Fix Issue Of Not Moving");
                    isTrafficLightRed = false;
                }
            }
        }
        else
        {
            if(!goAround && !trafficLightsLeft && !trafficLightsRight && !isOnRoundabout && !moveToOtherLane && !rotateBack && !moveBackToSingleLane)
            {
                _rb.velocity = Vector3.zero;
                carToGoAroundCollider = carToGoAround.GetComponent<Collider>();
                turnRight = true;
                goAround = true;
            }
        }


    }

    void ChangeToSignalLeft()
    {
        signalLeft = true;
    }

    void GoAroundCar()
    {
        timer += Time.deltaTime;
        rotIncrement += Time.deltaTime * rotSpeed;

        if(timer <= aroundCarRightTime && !turnRight)
        {
            print("GET CURRENT ROTATION!!!");
            getCurrentRot = transform.localEulerAngles.y;
            rotSpeed = rotSpeedRight;
            rotIncrement = 0;
            forwardMovement = false;
            turnLeft = false;
            turnRight = true;
        }else if(timer > aroundCarRightTime  && timer <= aroundCarLeftTime && !turnLeft)
        {
            _rb.velocity = Vector3.zero;
            rotSpeed = rotSpeedLeft;
            rotIncrement = 0;
            getCurrentRot = transform.localEulerAngles.y;
            turnRight = false;
            turnLeft = true;
        }else if(timer > aroundCarLeftTime && timer <= forwardMovementTime && !forwardMovement)
        {
            turnLeft = false;
            getCurrentRot = transform.localEulerAngles.y;
            _rb.velocity = Vector3.zero;
            forwardMovement = true;
        }else if(timer > forwardMovementTime && timer <= returnLeftTime && !turnLeft)
        {
            forwardMovement = false;
            rotIncrement = 0;
            getCurrentRot = transform.localEulerAngles.y;
            _rb.velocity = Vector3.zero;
            turnLeft = true;
        }else if(timer > returnLeftTime && timer <= returnRightTime && !turnRight)
        {
            _rb.velocity = Vector3.zero;
            rotIncrement = 0;
            turnLeft = false;
            getCurrentRot = transform.localEulerAngles.y;
            turnRight = true;
        }else if(timer > returnRightTime && turnRight)
        {
            _rb.velocity = Vector3.zero;
            moveForward = true;
            timer = 0;
            getCurrentRot = transform.eulerAngles.y;
            rotIncrement = 0;
            goAround = false;
            turnRight = false;
        }


        if (turnRight)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.x, getCurrentRot + rotIncrement, transform.rotation.z), t);
        }else if (turnLeft)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.x, getCurrentRot -rotIncrement, transform.rotation.z), t);
        }

        _rb.AddForce(aroundCarSpeed * Time.fixedDeltaTime * transform.forward, ForceMode.VelocityChange);
    }

    void MoveAroundParkedCar()
    {
        thisCarX = thisCarCollider.bounds.max.x;
        thatCarX = carToGoAroundCollider.bounds.min.x;

        if (!hasReachedLeftAroundCar && thisCarCollider.bounds.max.x + rightMovementOffset > carToGoAroundCollider.bounds.min.x)
        {
            if (!movingRight)
            {
                movingLeft = false;
                movingRight = true;
            }
            _rb.AddForce(speed * Time.fixedDeltaTime * transform.right, ForceMode.VelocityChange);
        }
        else
        {
            if(!hasReachedRightAroundCar)
            {
                _rb.velocity = Vector3.zero;
                hasReachedRightAroundCar = true;
            }

            if(hasReachedRightAroundCar && thisCarCollider.bounds.max.z + forwardMovementOffset > carToGoAroundCollider.bounds.min.z)
            {
                _rb.AddForce(speed * Time.fixedDeltaTime * transform.forward, ForceMode.VelocityChange);
            }
            else
            {
                if(!hasReachedLeftAroundCar)
                {
                    _rb.velocity = Vector3.zero;
                    hasReachedLeftAroundCar = true;
                }

                if(hasReachedLeftAroundCar && thisCarCollider.bounds.max.x < carToGoAroundCollider.bounds.max.x)
                {
                    if (!movingLeft)
                    {
                        movingRight = false;
                        movingLeft = true;
                    }
                    _rb.AddForce(speed * Time.fixedDeltaTime * -transform.right, ForceMode.VelocityChange);
                }
                else
                {
                    if (!completedMovingAroundCar)
                    {
                        _rb.velocity = Vector3.zero;
                        hasReachedForwardAroundCar = false;
                        hasReachedRightAroundCar = false;
                        hasReachedLeftAroundCar = false;
                        moveForward = true;
                        goAround = false;
                        carToGoAround = null;
                        completedMovingAroundCar = true;

                    }
                }

            }
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car") && !collision.gameObject.GetComponent<CarController>().isParking && collision.gameObject != gameObject && !isParking)
        {
            print("DESTROYED");
            if (transform.GetComponentInChildren<Camera>() != null)
            {
                transform.GetComponentInChildren<Camera>().transform.SetParent(null);
            }
            if (transform.GetComponentInChildren<Camera>() != null)
            {
                transform.GetComponentInChildren<Camera>().transform.SetParent(null);
            }
            if (trafficLightsLeft)
            {
                TrafficLights.amountTurningLeft--;
                if (TrafficLights.amountTurningLeft < 0)
                {
                    print("Correct Left Negative Value");
                    TrafficLights.amountTurningLeft = 0;
                }
            }
            if (trafficLightsRight)
            {
                TrafficLights.amountTurningRight--;
                if (TrafficLights.amountTurningRight < 0)
                {
                    print("Correct Right Negative Value");
                    TrafficLights.amountTurningRight = 0;
                }
            }
            if (isAlreadyWithin && !trafficLightsRight && !trafficLightsLeft)
            {
                TrafficLights.amountGoingForward--;
                if (TrafficLights.amountGoingForward < 0)
                {
                    print("Correct Forward Negative Value");
                    TrafficLights.amountGoingForward = 0;
                }
            }
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnteredRoundabout"))
        {
            goAtRoundabout = true;
            closestCarAtRoundabout = null;
            detectSignal = null;
        }
        if (other.gameObject.CompareTag("DespawnPoint"))
        {
            if (transform.GetComponentInChildren<Camera>() != null)
            {
                transform.GetComponentInChildren<Camera>().transform.SetParent(null);
            }
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("RoundaboutStart"))
        {
            _rb.velocity = Vector3.zero;
            if (!alreadyHasSignalDetector)
            {
                var col = Physics.OverlapSphere(transform.position, 4, detectSignalLayer, QueryTriggerInteraction.Collide);
                foreach (var c in col)
                {
                    print(c.gameObject.name);
                    detectSignal = c.gameObject;
                    alreadyHasSignalDetector = true;
                }

            }
        }
        if (other.gameObject.CompareTag("RoundaboutEnter") && !hasAlreadyJoinedRoundabout)
        {
            if (!isOnRoundabout)
            {
                rotationBeforeRoundabout = transform.localEulerAngles.y;
                getRoundaboutDirection = true;
                detectSignal = null;
                isOnRoundabout = true;
                hasAlreadyJoinedRoundabout = true;
            }
        }
        if (other.gameObject.CompareTag("RoundaboutLeave"))
        {
            if (isOnRoundabout)
            {
                hasRoadRot = false;
                roundaboutRight = false;
                roundaboutForward = false;
                roundaboutLeft = false;
                hasAlreadyJoinedRoundabout = false;
                signalLeft = false;
                signalRight = false;
                alreadyHasSignalDetector = false;
                isOnRoundabout = false;
            }
        }
        if (other.gameObject.CompareTag("Parking") && !goAround)
        {
            print("Park");
            var randInt = Random.Range(0, 100);
            if(randInt <= 99)
            {
                wantsToPark = true;
            }
            else
            {
                wantsToPark = false;
            }


            if (wantsToPark)
            {
                var parkingSpacesFound = Physics.OverlapSphere(transform.position, 15);
                foreach (var go in parkingSpacesFound)
                {
                    if (go.gameObject.CompareTag("ParkingSpace") && go.isTrigger)
                    {
                        if (go.gameObject.GetComponent<ParkingCollision>().amountOfCars <1)
                        {
                            float getDist = (transform.position - go.transform.position).sqrMagnitude;
                            if (getDist < dist)
                            {
                                targetParkingSpacePos = go.transform.position;
                                targetParkingSpace = go.gameObject;
                                parkingSpaceRot = go.gameObject.transform.localEulerAngles.y;
                                inLineOfParkingSpot = go.transform.GetChild(0);
                                dist = getDist;
                            }
                        }
                    }
                }
                isParking = true;
            }
            //isLeftTurn = true;

        }
        if (other.gameObject.CompareTag("WithinTrafficLights"))
        {
            isAlreadyWithin = true;
        }
        if (other.gameObject.CompareTag("LeftTurningCheck") && trafficLightsLeft && !hasRoadRot && other.gameObject.transform.localEulerAngles.y == transform.localEulerAngles.y)
        {
            print("Hit Left Turnbing Check");
            if (Physics.Raycast(leftRoadCheck.position, -transform.up, out RaycastHit hit, 2))
            {
                print("Got Road Going Left: " + hit.collider.transform.parent.parent.name);
                currentRoadSystem = hit.collider.transform.parent.parent;
                hasRoadRot = true;
            }
        }
        if(other.gameObject.CompareTag("LeftTurningCheck") && isOnRoundabout && other.gameObject.transform.localEulerAngles.y == rotationBeforeRoundabout)
        {
            //print("Hit Turning Point For Car Rot");
            if (Physics.Raycast(leftRoadCheck.position, -transform.up, out RaycastHit hit, 2))
            {
                print("Road Found is: " + hit.collider.gameObject);
                if(other.gameObject.layer == 7 && roundaboutLeft)
                {
                    print(hit.collider.gameObject.name);
                    _rb.velocity = Vector3.zero;
                    rotationBeforeRoundabout = 0;
                    currentRoadSystem = hit.collider.transform.parent;
                    hasRoadRot = true;
                }
                if (other.gameObject.layer == 8 && roundaboutRight)
                {
                    print(hit.collider.gameObject.name);
                    _rb.velocity = Vector3.zero;
                    rotationBeforeRoundabout = 0;
                    currentRoadSystem = hit.collider.transform.parent;
                    signalRight = false;
                    Invoke(nameof(ChangeToSignalLeft), 2f);
                    hasRoadRot = true;
                }
                if (other.gameObject.layer == 9 && roundaboutForward)
                {
                    print("Signal Left");
                    signalRight = false;
                    Invoke(nameof(ChangeToSignalLeft), 2f);
                    print(hit.collider.gameObject.name);
                    _rb.velocity = Vector3.zero;
                    rotationBeforeRoundabout = 0;
                    currentRoadSystem = hit.collider.transform.parent;
                    hasRoadRot = true;
                }
                if(other.gameObject.layer ==9 && roundaboutRight)
                {
                    print("Change Signal From Right TO Left");
                    Invoke(nameof(ChangeToSignalLeft), 2f);
                    signalRight = false;
                }
                if(other.gameObject.layer == 7 && roundaboutForward)
                {
                    Invoke(nameof(ChangeToSignalLeft), 2f);
                }
            }
        }
        if (other.gameObject.CompareTag("RightTurningCheck") && trafficLightsRight && !hasRoadRot && other.gameObject.transform.localEulerAngles.y == transform.localEulerAngles.y)
        {
            if (Physics.Raycast(rightRoadCheck.position, -transform.up, out RaycastHit hit, 2))
            {
                print("Going Right: " + hit.collider.transform.parent.parent.name);
                currentRoadSystem = hit.collider.transform.parent.parent;
                hasRoadRot = true;
            }
        }
        if (other.gameObject.CompareTag("TrafficLight"))
        {
            var collidersFound = Physics.OverlapSphere(carSpherecastPoint.position, checkForTurningCarsRadius);
            foreach(var collider in collidersFound)
            {
                if (collider.gameObject.CompareTag("MidSection"))
                {
                    interSection = collider.gameObject;
                }
                if (collider.gameObject.CompareTag("WithinTrafficLights"))
                {
                    checkIfCarIsInProgressOfTurning = collider.gameObject;
                }
            }
        }
        if (other.gameObject.CompareTag("MidSection"))
        {
            carIsTurning = true;
            runOnce = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Crossings"))
        {
            if (other.gameObject.GetComponent<PedestrianCrossing>().amountOfCitizensCrossing >= 1)
            {
                isTrafficLightRed = true;
            }
            else
            {
                isTrafficLightRed = false;
            }
        }


        if (other.gameObject.CompareTag("TrafficLight"))
        {
            if(TrafficLights.amountTurningRight > 2)
            {
                hasPriority = false;
            }
            if(interSection!=null && interSection.GetComponent<Intersection>().amountOfCars == 2)
            {
                hasPriority = false;
            }
        }/*
        if (other.gameObject.CompareTag("MidSection"))
        {
                collidersFound = Physics.OverlapSphere(checkInfront.position, 3);
                foreach (var collider in collidersFound)
                {
                    if (collider.gameObject.CompareTag("Car") && trafficLightsRight && !collider.gameObject.GetComponent<CarController>().trafficLightsRight)
                    {
                        //print("Car Infront");
                        carInfrontFound = collider.gameObject;
                        _rb.velocity = Vector3.zero;
                    }
                    if(collider.gameObject.CompareTag("Car") && trafficLightsRight && collider.gameObject.GetComponent<CarController>().trafficLightsRight && collider.gameObject.GetComponent<CarController>().isAlreadyWithin)
                    {
                        carInfrontFound = collider.gameObject;
                        _rb.velocity = Vector3.zero;
                    }
                }
        }*/
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("TrafficLight"))
        {
            timeWithinTrafficLightTrigger = 0; 
        }
        if (other.gameObject.CompareTag("RoundaboutLeave"))
        {
            if (isOnRoundabout)
            {
                isOnRoundabout = false;
            }
        }
        if (other.gameObject.CompareTag("MidSection"))
        {
            if (trafficLightsRight)
            {
                print("Turning Right");
                TrafficLights.amountTurningRight--;
            }
            if (trafficLightsLeft)
            {
                print("Turning Left");
                TrafficLights.amountTurningLeft--;
            }
            if(!trafficLightsRight && !trafficLightsLeft)
            {
                print("Turning Forward");
                TrafficLights.amountGoingForward--;
            }

            if (trafficLightsLeft || trafficLightsRight)
            {
                trafficLightsRight = false;
                trafficLightsLeft = false;
            }

            if(interSection != null)
            {
                interSection = null;
            }
            if(checkIfCarIsInProgressOfTurning != null)
            {
                checkIfCarIsInProgressOfTurning = null;
            }
            carIsTurning = false;
            carInfrontFound = null;
            runOnce = false;
            getDir = true;
        }
        if (other.gameObject.CompareTag("RightTurningCheck") && !runOnce && trafficLightsRight)
        {
            runOnce = true;
        }
        if (other.gameObject.CompareTag("WithinTrafficLights"))
        {
            if (interSection != null)
            {
                interSection = null;
            }
            if (checkIfCarIsInProgressOfTurning != null)
            {
                checkIfCarIsInProgressOfTurning = null;
            }
            isAlreadyWithin = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 4);
    }

}
