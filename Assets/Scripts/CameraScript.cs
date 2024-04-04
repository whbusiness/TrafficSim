using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Situations
{
    ROUNDABOUT,
    TRAFFICLIGHTS,
    FREEROAM
}


public class CameraScript : MonoBehaviour
{
    [SerializeField]
    private InputAction movement, looking;
    private Vector3 move, movementDir;
    private Vector2 look;
    [SerializeField]
    private float speed, sensY, sensX, timeToLerp;
    private float xClamp, lookingY, lookingX;
    [SerializeField]
    private Transform carToAttachTo;
    private Situations situation;
    private CheckIfEmpty[] emptyLocations;
    private SpawnCars spawners;
    private SpawnCars spawnCars;
    private GameObject[] parking;
    [SerializeField]
    private InputAction roundaboutScenario, trafficLightScenario, freeRoamScenario, randomCarSpectate, continueScenario;
    private float roundaboutFloat, trafficLightFloat, freeRoamFloat, randomCarFloat, continueScenarioFloat;
    private GameObject[] roundaboutSpawnPoints, trafficLightSpawnPoints;
    private GameObject roundaboutSpectatePoint, trafficLightSpectatePoint, trafficLightInScenario;
    [SerializeField]
    private GameObject carPref;
    private GameObject specCar;
    bool alreadyShown = false, alreadyShownRed = false, alreadyShownGreen = false, alreadyShownRoundaboutStart = false, alreadyShownCarGoingLeftAtRoundabout, alreadyShownRoundaboutSignalPanel;
    private GameObject isCloseToTrafficLights;
    private GameObject trafficLightLeftPanel, trafficLightRightPanel, trafficLightForwardPanel, trafficLightGreenPanel, trafficLightRedPanel;
    private GameObject RoundaboutStartPanel, roundaboutOtherCarGoingLeftPanel, roundaboutSignalPanel;
    // Start is called before the first frame update


    private void Awake()
    {
        xClamp = 90;
        movement.performed += ctx => move = ctx.ReadValue<Vector2>();
        movement.canceled += ctx => move = Vector2.zero;
        looking.performed += ctx => look = ctx.ReadValue<Vector2>();
        looking.canceled += ctx => look = Vector2.zero;
        roundaboutScenario.performed += ctx => roundaboutFloat = 1;
        roundaboutScenario.canceled += ctx => roundaboutFloat = 0;
        trafficLightScenario.performed += ctx => trafficLightFloat = 1;
        trafficLightScenario.canceled += ctx => trafficLightFloat = 0;
        freeRoamScenario.performed += ctx => freeRoamFloat = 1;
        freeRoamScenario.canceled += ctx => freeRoamFloat = 0;
        randomCarSpectate.performed += ctx => randomCarFloat = 1;
        randomCarSpectate.canceled += ctx => randomCarFloat = 0;
        continueScenario.performed += ctx => continueScenarioFloat = 1;
        continueScenario.canceled += ctx => continueScenarioFloat = 0;

    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        spawnCars = FindObjectOfType<SpawnCars>();
        emptyLocations = FindObjectsOfType<CheckIfEmpty>();
        spawners = FindObjectOfType<SpawnCars>();
        parking = GameObject.FindGameObjectsWithTag("Parking");
        isCloseToTrafficLights = GameObject.FindGameObjectWithTag("TrafficLightLight");
        roundaboutSpawnPoints = GameObject.FindGameObjectsWithTag("RoundAboutSituationSpawn");
        trafficLightSpawnPoints = GameObject.FindGameObjectsWithTag("TrafficLightSituationSpawn");
        roundaboutSpectatePoint = GameObject.FindGameObjectWithTag("RoundAboutSituationSpawnSpectate");
        trafficLightSpectatePoint = GameObject.FindGameObjectWithTag("TrafficLightSituationSpawnSpectate");
        trafficLightLeftPanel = GameObject.FindGameObjectWithTag("TrafficLightLeftPanel");
        trafficLightRightPanel = GameObject.FindGameObjectWithTag("TrafficLightRightPanel");
        trafficLightForwardPanel = GameObject.FindGameObjectWithTag("TrafficLightForwardPanel");
        trafficLightInScenario = GameObject.Find("TrafficLightInScenario");
        trafficLightGreenPanel = GameObject.FindGameObjectWithTag("TrafficLightGreenPanel");
        trafficLightRedPanel = GameObject.FindGameObjectWithTag("TrafficLightRedPanel");
        RoundaboutStartPanel = GameObject.FindGameObjectWithTag("RoundaboutStartPanel");
        roundaboutOtherCarGoingLeftPanel = GameObject.FindGameObjectWithTag("RoundaboutOtherCarGoingLeftPanel");
        roundaboutSignalPanel = GameObject.FindGameObjectWithTag("RoundaboutSignalPanel");

        RemovePreviousPanels();
        RemovePrevValues();
        situation = Situations.FREEROAM;
    }

    private void OnEnable()
    {
        movement.Enable();
        looking.Enable();
        roundaboutScenario.Enable();
        trafficLightScenario.Enable();
        freeRoamScenario.Enable();
        randomCarSpectate.Enable();
        continueScenario.Enable();
    }
    private void OnDisable()
    {
        movement.Disable();
        looking.Disable();
        roundaboutScenario.Disable();
        trafficLightScenario.Disable();
        freeRoamScenario.Disable();
        randomCarSpectate.Disable();
        continueScenario.Disable();
    }
    private void LateUpdate()
    {
        ApplyCameraRot();
    }
    private void Update()
    {
        if (move.x != 0 || move.y != 0)
        {
            if(carToAttachTo != null)
            {
                transform.SetParent(null);
                carToAttachTo = null;
            }
            ApplyMovement();
        } 

        if(roundaboutFloat != 0)
        {
            RemovePrevValues();
            ClearCars();
            RemovePreviousPanels();
            RoundAboutSituation();
            situation = Situations.ROUNDABOUT;
        }
        if (trafficLightFloat != 0)
        {
            ClearCars();
            RemovePrevValues();
            RemovePreviousPanels();
            TrafficLightSituation();
            situation = Situations.TRAFFICLIGHTS;
        }
        if (freeRoamFloat != 0)
        {
            RemovePreviousPanels();
            FreeRoam();
            RemovePrevValues();
            situation = Situations.FREEROAM;
        }
        if(randomCarFloat != 0)
        {
            var randInt = Random.Range(0, spawnCars.Cars.Count);
            if (spawnCars.Cars[randInt] != null)
            {
                carToAttachTo = spawnCars.Cars[randInt].transform;
            }
        }


        if(situation == Situations.TRAFFICLIGHTS && specCar != null)
        {
            if (specCar.GetComponent<CarController>().isInTrafficLightArea)
            {
                if (trafficLightInScenario.GetComponent<TrafficLights>().state == Light.RED && !alreadyShownRed)
                {
                    DisplayTrafficLightRedPanel();
                    alreadyShownRed = true;
                }
                if (trafficLightInScenario.GetComponent<TrafficLights>().state == Light.GREEN && !alreadyShownGreen)
                {
                    DisplayTrafficLightGreenPanel();
                    alreadyShownGreen = true;
                }
            }

            if (!alreadyShown)
            {
            if (specCar.GetComponent<CarController>().trafficLightsLeft)
            {
                DisplayTrafficLightsLeftInfo();
            }
            else if (specCar.GetComponent<CarController>().trafficLightsRight)
            {
                DisplayTrafficLightsRightInfo();
            }
            else if(!specCar.GetComponent<CarController>().trafficLightsLeft && !specCar.GetComponent<CarController>().trafficLightsRight && specCar.GetComponent<CarController>().isAlreadyWithin)
            {
                DisplayTrafficLightsForwardInfo();
            }

            }

            if (trafficLightLeftPanel.activeInHierarchy || trafficLightRightPanel.activeInHierarchy || trafficLightForwardPanel.activeInHierarchy)
            {
                if(Time.timeScale == 0 && continueScenarioFloat != 0)
                {
                    RemovePreviousPanels();
                    Time.timeScale = 1;
                    alreadyShown = true;
                }
            }

            if (trafficLightRedPanel.activeInHierarchy)
            {
                if (Time.timeScale == 0 && continueScenarioFloat != 0)
                {
                    RemovePreviousPanels();
                    Time.timeScale = 1;
                    alreadyShownRed = true;
                }
            }
            if (trafficLightGreenPanel.activeInHierarchy)
            {
                if (Time.timeScale == 0 && continueScenarioFloat != 0)
                {
                    RemovePreviousPanels();
                    Time.timeScale = 1;
                    alreadyShownGreen = true;
                }
            }
        }

        if(situation == Situations.ROUNDABOUT)
        {
            if(specCar.GetComponent<CarController>().detectSignal != null && !alreadyShownRoundaboutStart)
            {
                DisplayRoundaboutStartPanel();
            }
            if(specCar.GetComponent<CarController>().isOtherCarSignallingLeftAtRoundabout && !alreadyShownCarGoingLeftAtRoundabout)
            {
                DisplayRoundaboutOtherCarGoingLeft();
            }
            if(specCar.GetComponent<CarController>().signalLeft && !alreadyShownRoundaboutSignalPanel)
            {
                DisplayRoundaboutSignal();
            }

            if (RoundaboutStartPanel.activeInHierarchy)
            {
                if(Time.timeScale == 0 && continueScenarioFloat != 0)
                {
                    RemovePreviousPanels();
                    Time.timeScale = 1;
                    alreadyShownRoundaboutStart = true;
                }
            }
            if (roundaboutOtherCarGoingLeftPanel.activeInHierarchy)
            {
                if (Time.timeScale == 0 && continueScenarioFloat != 0)
                {
                    RemovePreviousPanels();
                    Time.timeScale = 1;
                    alreadyShownCarGoingLeftAtRoundabout = true;
                }
            }
            if (roundaboutSignalPanel.activeInHierarchy)
            {
                if (Time.timeScale == 0 && continueScenarioFloat != 0)
                {
                    RemovePreviousPanels();
                    Time.timeScale = 1;
                    alreadyShownRoundaboutSignalPanel = true;
                }
            }
        }


        if (Mouse.current.leftButton.wasPressedThisFrame && situation == Situations.FREEROAM)
        {
            if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.CompareTag("Car"))
                {
                    carToAttachTo = hit.collider.transform;
                }
            }
        }

        if(carToAttachTo != null)
        {
            transform.SetParent(carToAttachTo);
            transform.position = Vector3.Lerp(transform.position, carToAttachTo.position + new Vector3(0, 1, 0), timeToLerp * Time.deltaTime);
        }


    }
    void ApplyMovement()
    {
        movementDir = new(move.x, 0, move.y);
        movementDir = transform.TransformDirection(movementDir);

        transform.position += speed * Time.deltaTime * movementDir.normalized;
    }
    void ApplyCameraRot()
    {
        lookingX += look.x * sensX * Time.deltaTime;
        lookingY += look.y * sensY * Time.deltaTime;
        transform.eulerAngles = new Vector3(-lookingY, lookingX, 0);
    }

    void RemovePrevValues()
    {
        alreadyShown = false;
        alreadyShownGreen = false;
        alreadyShownRed = false;
        alreadyShownRoundaboutStart = false;
        alreadyShownCarGoingLeftAtRoundabout = false;
        alreadyShownRoundaboutSignalPanel = false;
    }

    void ClearCars()
    {
        transform.SetParent(null);
        carToAttachTo = null;
        for (int i = 0; i < spawnCars.Cars.Count; i++)
        {
            Destroy(spawnCars.Cars[i]);
        }
        foreach (var item in emptyLocations)
        {
            item.isEmpty = true;
        }
        if(specCar != null)
        {
            Destroy(specCar);
        }
        specCar = null;
        spawners.canSpawn = false;
        spawnCars.Cars.Clear();
    }

    void FreeRoam()
    {
        foreach(var item in parking)
        {
            if(!item.activeInHierarchy)
            {
                item.SetActive(true);
            }
        }
        spawners.canSpawn = true;
    }

    void RemovePreviousPanels()
    {
        if(Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }
        trafficLightLeftPanel.SetActive(false);
        trafficLightRightPanel.SetActive(false);
        trafficLightForwardPanel.SetActive(false);
        trafficLightGreenPanel.SetActive(false);
        trafficLightRedPanel.SetActive(false);
        RoundaboutStartPanel.SetActive(false);
        roundaboutOtherCarGoingLeftPanel.SetActive(false);
        roundaboutSignalPanel.SetActive(false);
    }

    void RoundAboutSituation()
    {
        foreach (var item in parking)
        {
            item.SetActive(false);
        }
        foreach(var spawnPoint in roundaboutSpawnPoints)
        {
            GameObject go = Instantiate(carPref, new Vector3(spawnPoint.transform.position.x, 0.09000345f, spawnPoint.transform.position.z), spawnPoint.transform.rotation);
            go.name = "car";
            spawnCars.Cars.Add(go);
        }
        GameObject spectateCar = Instantiate(carPref, new Vector3(roundaboutSpectatePoint.transform.position.x, 0.09000345f, roundaboutSpectatePoint.transform.position.z), roundaboutSpectatePoint.transform.rotation);
        spectateCar.name = "car";
        specCar = spectateCar;
        carToAttachTo = spectateCar.transform;
    }

    void TrafficLightSituation()
    {
        foreach (var item in parking)
        {
            item.SetActive(false);
        }
        foreach (var spawnPoint in trafficLightSpawnPoints)
        {
            GameObject go = Instantiate(carPref, new Vector3(spawnPoint.transform.position.x, 0.09000345f, spawnPoint.transform.position.z), spawnPoint.transform.rotation);
            go.name = "car";
            spawnCars.Cars.Add(go);
        }
        GameObject spectateCar = Instantiate(carPref, new Vector3(trafficLightSpectatePoint.transform.position.x, 0.09000345f, trafficLightSpectatePoint.transform.position.z), trafficLightSpectatePoint.transform.rotation);
        spectateCar.name = "car";
        specCar = spectateCar;
        carToAttachTo = spectateCar.transform;
    }

    void DisplayTrafficLightsLeftInfo()
    {
        Time.timeScale = 0;
        trafficLightLeftPanel.SetActive(true);
    }

    void DisplayTrafficLightsForwardInfo()
    {
        Time.timeScale = 0;
        trafficLightForwardPanel.SetActive(true);
    }

    void DisplayTrafficLightsRightInfo()
    {
        Time.timeScale = 0;
        trafficLightRightPanel.SetActive(true);
    }

    void DisplayTrafficLightGreenPanel()
    {
        Time.timeScale = 0;
        trafficLightGreenPanel.SetActive(true);
    }

    void DisplayTrafficLightRedPanel()
    {
        Time.timeScale = 0;
        trafficLightRedPanel.SetActive(true);
    }

    void DisplayRoundaboutStartPanel()
    {
        Time.timeScale = 0;
        RoundaboutStartPanel.SetActive(true);
    }

    void DisplayRoundaboutOtherCarGoingLeft()
    {
        Time.timeScale = 0;
        roundaboutOtherCarGoingLeftPanel.SetActive(true);
    }
    void DisplayRoundaboutSignal()
    {
        Time.timeScale = 0;
        roundaboutSignalPanel.SetActive(true);
    }
}
