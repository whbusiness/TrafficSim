using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CitizensController : MonoBehaviour
{
    private NavMeshAgent _agent;
    private bool hasTargetLocation = false;
    [SerializeField]
    private float speed, t, distToTarget = Mathf.Infinity;
    [SerializeField]
    private Vector3 targetPos, prevPos, dir, posToMoveTo;
    [SerializeField]
    private GameObject[] crossings;
    [SerializeField]
    private GameObject nextCrossingPoint, buildingToMoveTo;
    private int randInt;
    [SerializeField]
    private float alphaTransparency;
    [SerializeField]
    private SkinnedMeshRenderer mesh;
    private Material mat;
    private List<CitizensController> otherCitizens = new();
    [SerializeField]
    private List<GameObject> Locations = new(), spotsToFish = new();
    private int amountOfFishingSpotsOccupied;
    [SerializeField]
    private float checkForCarsRadius;
    private Animator _anim;
    // Start is called before the first frame update
    void Start()
    {
        distToTarget = Mathf.Infinity;
        amountOfFishingSpotsOccupied = 0;
        _agent = GetComponent<NavMeshAgent>();
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        _anim = GetComponent<Animator>();
        mat = mesh.material;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasTargetLocation)
        {
            FindTargetLocation();
        }
        if (hasTargetLocation)
        {
            MoveToTargetLocation();
        }

        if (buildingToMoveTo == GameObject.FindGameObjectWithTag("FishingSpots"))
        {
            foreach (var others in otherCitizens)
            {
                if (others.targetPos == targetPos)
                {
                    print("Other Citizen is moving here");
                    randInt = Random.Range(0, Locations.Count);
                    targetPos = spotsToFish[randInt].transform.position;
                    buildingToMoveTo = spotsToFish[randInt];
                }
            }
            print("RotatePlayer");
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position - buildingToMoveTo.transform.position), t * Time.deltaTime);
        }

    }

    void FindTargetLocation()
    {
        Locations.AddRange(GameObject.FindGameObjectsWithTag("Buildings"));
        Locations.Add(GameObject.FindGameObjectWithTag("Fishing"));

        randInt = Random.Range(0, Locations.Count);
        targetPos = Locations[randInt].transform.position;
        buildingToMoveTo = Locations[randInt];

        if (buildingToMoveTo == GameObject.FindGameObjectWithTag("Fishing"))
        {
            spotsToFish.AddRange(GameObject.FindGameObjectsWithTag("FishingSpots"));
            otherCitizens.AddRange(FindObjectsOfType<CitizensController>());
            foreach(var others in otherCitizens)
            {
                if(others.targetPos == targetPos)
                {
                    print("Other Citizen is moving here");
                    randInt = Random.Range(0, Locations.Count);
                    targetPos = spotsToFish[randInt].transform.position;
                    buildingToMoveTo = spotsToFish[randInt];
                }
            }
            foreach(var fishingspot in spotsToFish)
            {
                if (fishingspot.GetComponent<FishingSpots>().isOccupied)
                {
                    amountOfFishingSpotsOccupied++;
                }
            }
            if(amountOfFishingSpotsOccupied == spotsToFish.Count)
            {
                    randInt = Random.Range(0, Locations.Count);
                    targetPos = Locations[randInt].transform.position;
                    buildingToMoveTo = Locations[randInt];
            }
        }

        prevPos = targetPos;
        hasTargetLocation = true;
    }

    void MoveToTargetLocation()
    {
        NavMeshPath navpath = new NavMeshPath();
        distToTarget = Vector3.Distance(_agent.transform.position, _agent.destination);
        if (_agent.hasPath && distToTarget <=2)
        {
            Color color = mat.color;
            color.a += 1f * Time.deltaTime;
            mat.color = color;
            _anim.SetBool("isWalking", false);
            //Start Fading Alpha Transparency
            //When faded enough teleport it to another location rather than destroy and make another
        }
        else
        {
            if (!_anim.GetBool("isWalking"))
            {
                _anim.SetBool("isWalking", true);
            }

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 20, NavMesh.AllAreas))
            {
                posToMoveTo = hit.position;
            }
            _agent.SetDestination(posToMoveTo);
        }
        //transform.position += nextCrossingPoint.transform.position; 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, checkForCarsRadius);
    }
}
