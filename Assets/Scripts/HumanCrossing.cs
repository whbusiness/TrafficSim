using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanCrossing : MonoBehaviour
{
    NavMeshAgent _agent;
    Transform dest;
    CameraScript _cameraScript;
    Animator _animator;
    public bool hasReachedDest = false;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _cameraScript = FindObjectOfType<CameraScript>();
        dest = GameObject.FindGameObjectWithTag("WalkingDest").transform;
        _agent.isStopped = true;
        _animator.SetBool("isWalking", false);
        hasReachedDest = false;
    }

    public void OnCarStopped()
    {
        _agent.isStopped = false;
        _animator.SetBool("isWalking", true);
        _agent.SetDestination(dest.position);
    }

    public void OnFinishedWalking()
    {
        hasReachedDest = true;
        _animator.SetBool("isWalking", false);
        _cameraScript.situation = Situations.FREEROAM;
        _cameraScript.FreeRoam();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("WalkingDest"))
        {
            Invoke(nameof(OnFinishedWalking), .5f);
        }
    }
}
