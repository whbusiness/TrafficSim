using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCars : MonoBehaviour
{
    [SerializeField]
    private GameObject[] cars;
    [SerializeField]
    private Transform[] spawnPoints;
    public List<GameObject> Cars = new();
    public bool canSpawn = true;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(Spawner), 4, 10);
    }

    void Spawner()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i].gameObject.GetComponent<CheckIfEmpty>().isEmpty && canSpawn)
            {
                int random = Random.Range(0, 2);
                GameObject carSpawn = Instantiate(cars[0], new Vector3(spawnPoints[i].position.x, 0.09000345f, spawnPoints[i].position.z), spawnPoints[i].rotation);
                carSpawn.name = "Car";
                Cars.Add(carSpawn);
            }
        }
        /*
        foreach(var t in spawnPoints)
        {
            int random = Random.Range(0, 2);
            GameObject carSpawn = Instantiate(cars[random], new Vector3(t.position.x, 0.09000345f, t.position.z), t.rotation);
            carSpawn.name = "Car";
        }*/
    }

    void SpawnerFastCar()
    {
        foreach(var t in spawnPoints)
        {
            if (t.CompareTag("FastSpawnPoint"))
            {
                GameObject carSpawn = Instantiate(cars[1], t.position, t.rotation);
                carSpawn.name = "FastCar";
            }
        }
    }
}
