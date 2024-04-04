using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationExample : MonoBehaviour
{
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = transform.rotation.y;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * 2;
        RotRight(timer);
    }

    void RotRight(float value)
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x, value, transform.rotation.z);
    }
}
