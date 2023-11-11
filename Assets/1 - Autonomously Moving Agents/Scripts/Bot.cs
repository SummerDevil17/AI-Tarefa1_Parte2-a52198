using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    [SerializeField] GameObject copReference;

    NavMeshAgent botAgent;

    void Start()
    {
        botAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Flee(copReference.transform.position);
    }

    void Seek(Vector3 location)
    {
        botAgent.SetDestination(location);
    }

    void Flee(Vector3 location)
    {
        Vector3 fleeVector = location - transform.position;
        botAgent.SetDestination(transform.position - fleeVector);
    }
}
