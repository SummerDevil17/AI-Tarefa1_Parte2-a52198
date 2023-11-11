using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    [SerializeField] GameObject copReference;
    Drive copDriveReference;
    Vector3 wanderTargetVector = Vector3.zero;

    NavMeshAgent botAgent;

    void Start()
    {
        botAgent = GetComponent<NavMeshAgent>();
        copDriveReference = copReference.GetComponent<Drive>();
    }

    // Update is called once per frame
    void Update()
    {
        Wander();
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

    void Wander()
    {
        float wanderRadius = 10f, wanderDistance = 10f, wanderJitter = 1.5f;

        wanderTargetVector += new Vector3(wanderJitter * Random.Range(-1f, 1f), 0, wanderJitter * Random.Range(-1f, 1f));

        wanderTargetVector.Normalize();
        wanderTargetVector *= wanderRadius;

        Vector3 targetLocal = wanderTargetVector + new Vector3(0, 0, wanderDistance);
        Vector3 targetWorld = transform.InverseTransformVector(targetLocal);

        Seek(targetWorld);
    }
}
