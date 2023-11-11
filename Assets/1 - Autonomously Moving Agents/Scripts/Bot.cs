using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    [SerializeField] GameObject copReference;
    Drive copDriveReference;

    NavMeshAgent botAgent;

    void Start()
    {
        botAgent = GetComponent<NavMeshAgent>();
        copDriveReference = copReference.GetComponent<Drive>();
    }

    // Update is called once per frame
    void Update()
    {
        Evade();
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

    void Pursue()
    {
        Vector3 targetDir = copReference.transform.position - transform.position;
        float relativeHeading = Vector3.Angle(transform.forward, transform.TransformVector(copReference.transform.forward));
        float angleToTarget = Vector3.Angle(transform.forward, transform.TransformVector(targetDir));

        if (angleToTarget > 90f && relativeHeading < 20f || copDriveReference.currentSpeed < 0.01f)
        { Seek(copReference.transform.position); return; }

        float lookAhead = targetDir.magnitude / botAgent.speed + copDriveReference.currentSpeed;
        Seek(copReference.transform.position + copReference.transform.forward * lookAhead);
    }

    void Evade()
    {
        Vector3 targetDir = copReference.transform.position - transform.position;
        float relativeHeading = Vector3.Angle(transform.forward, transform.TransformVector(copReference.transform.forward));
        float angleToTarget = Vector3.Angle(transform.forward, transform.TransformVector(targetDir));

        if (angleToTarget > 90f && relativeHeading < 20f || copDriveReference.currentSpeed < 0.01f)
        { Flee(copReference.transform.position); return; }

        float lookAhead = targetDir.magnitude / botAgent.speed + copDriveReference.currentSpeed;
        Flee(copReference.transform.position + copReference.transform.forward * lookAhead);
    }
}
