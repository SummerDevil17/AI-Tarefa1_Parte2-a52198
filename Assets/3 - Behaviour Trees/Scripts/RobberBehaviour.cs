using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : MonoBehaviour
{
    [SerializeField] GameObject diamond;
    [SerializeField] GameObject van;
    [SerializeField] GameObject frontDoor;
    [SerializeField] GameObject backDoor;

    NavMeshAgent agent;
    BehaviourTree tree;

    public enum ActionState { IDLE, WORKING }
    ActionState state = ActionState.IDLE;

    Node.Status treeStatus = Node.Status.RUNNING;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        tree = new BehaviourTree();
        Sequence steal = new Sequence("Steal Something");

        Leaf goToDiamond = new Leaf("Go To Diamond", GotToDiamond);
        Leaf goToVan = new Leaf("Go To Van", GotToVan);
        Leaf goToFrontDoor = new Leaf("Go To Front Door", GoToFrontDoor);
        Leaf goToBackDoor = new Leaf("Go To Back Door", GoToBackDoor);

        Selector openDoor = new Selector("Open Door");

        openDoor.AddChild(goToFrontDoor);
        openDoor.AddChild(goToBackDoor);

        steal.AddChild(openDoor);
        steal.AddChild(goToDiamond);
        steal.AddChild(goToVan);
        tree.AddChild(steal);

        //tree.PrintTree();
    }

    public Node.Status GotToDiamond()
    {
        Node.Status status = GoToLocation(diamond.transform.position);

        if (status == Node.Status.SUCCESS)
        {
            diamond.transform.parent = transform;
            return Node.Status.SUCCESS;
        }
        return status;
    }

    public Node.Status GoToFrontDoor()
    {
        return GoToDoor(frontDoor);
    }

    public Node.Status GoToBackDoor()
    {
        return GoToDoor(backDoor);
    }

    public Node.Status GoToDoor(GameObject doorToUse)
    {
        Node.Status status = GoToLocation(doorToUse.transform.position);

        if (status == Node.Status.SUCCESS)
        {
            if (!doorToUse.GetComponent<Lock>().isLocked)
            {
                doorToUse.SetActive(false);
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILURE;
        }
        else { return status; }
    }

    public Node.Status GotToVan()
    {
        return GoToLocation(van.transform.position);
    }

    Node.Status GoToLocation(Vector3 destination)
    {
        float distanceToTarget = Vector3.Distance(destination, transform.position);
        if (state == ActionState.IDLE)
        {
            agent.SetDestination(destination);
            state = ActionState.WORKING;
        }
        else if (Vector3.Distance(agent.pathEndPosition, destination) >= 2)
        {
            state = ActionState.IDLE;
            return Node.Status.FAILURE;
        }
        else if (distanceToTarget < 2)
        {
            state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }

    void Update()
    {
        if (treeStatus == Node.Status.RUNNING)
            treeStatus = tree.Process();
    }
}
