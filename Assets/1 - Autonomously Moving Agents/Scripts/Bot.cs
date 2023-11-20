using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    [SerializeField] GameObject copReference;
    Drive copDriveReference;
    Vector3 wanderTargetVector = Vector3.zero;
    private bool hiding = false;

    NavMeshAgent botAgent;

    void Start()
    {
        botAgent = GetComponent<NavMeshAgent>();
        copDriveReference = copReference.GetComponent<Drive>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hiding) { return; }

        if (!IsCopInRange()) { Wander(); }
        else if (CanSeeCop() && CanSeeMe()) { CleverHide(); hiding = true; Invoke("FinishedHiding", 5f); }
        else { Pursue(); }
    }

    bool CanSeeCop()
    {
        RaycastHit raycastInfo;
        Vector3 directionToCop = copReference.transform.position - this.transform.position;
        float lookAngle = Vector3.Angle(transform.forward, directionToCop);

        if (lookAngle < 60f && Physics.Raycast(transform.position, directionToCop, out raycastInfo))
        {
            if (raycastInfo.transform.gameObject.CompareTag("Cop")) { return true; }
        }
        return false;
    }

    bool CanSeeMe()
    {
        Vector3 copDirection = this.transform.position - copReference.transform.position;
        float copLookAngle = Vector3.Angle(copDirection, copReference.transform.forward);

        if (copLookAngle < 60f) { return true; }

        return false;
    }

    bool IsCopInRange() { return Vector3.Distance(copReference.transform.position, transform.position) < 10f; }

    private void FinishedHiding() { hiding = false; }

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

    void Hide()
    {
        float dist = Mathf.Infinity;
        Vector3 chosenHidingGO = Vector3.zero;

        for (int i = 0; i < World.Instance.GetHidingGameObj().Length; i++)
        {
            Vector3 hidingDir = World.Instance.GetHidingGameObj()[i].transform.position - copReference.transform.position;
            Vector3 hidingPos = World.Instance.GetHidingGameObj()[i].transform.position + hidingDir.normalized * 10f;

            if (Vector3.Distance(this.transform.position, hidingPos) < dist)
            {
                chosenHidingGO = hidingPos;
                dist = Vector3.Distance(this.transform.position, hidingPos);
            }
        }
        Seek(chosenHidingGO);
    }

    void CleverHide()
    {
        float dist = Mathf.Infinity;
        Vector3 chosenHidingPosition = Vector3.zero;
        Vector3 chosenDir = Vector3.zero;
        GameObject chosenGO = World.Instance.GetHidingGameObj()[0];

        for (int i = 0; i < World.Instance.GetHidingGameObj().Length; i++)
        {
            Vector3 hidingDir = World.Instance.GetHidingGameObj()[i].transform.position - copReference.transform.position;
            Vector3 hidingPos = World.Instance.GetHidingGameObj()[i].transform.position + hidingDir.normalized * 10f;

            if (Vector3.Distance(this.transform.position, hidingPos) < dist)
            {
                chosenHidingPosition = hidingPos;
                chosenDir = hidingDir;
                chosenGO = World.Instance.GetHidingGameObj()[i];
                dist = Vector3.Distance(this.transform.position, hidingPos);
            }
        }
        Collider hideCollider = chosenGO.GetComponent<Collider>();
        Ray backRay = new Ray(chosenHidingPosition, -chosenDir.normalized);

        RaycastHit hitInfo;
        float distance = 100f;
        hideCollider.Raycast(backRay, out hitInfo, distance);

        Seek(hitInfo.point + chosenDir.normalized * 3f);
    }
}
