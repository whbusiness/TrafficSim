using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    [SerializeField]
    private Transform cam;
    private void Start()
    {
        cam = FindObjectOfType<Camera>().transform;
    }
    public void ChangeCameraRotandPos(Transform carHit)
    {
        print("Update Cam Pos");
        cam.position = carHit.position;
    }
}
