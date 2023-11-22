using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : MonoBehaviour
{
    [SerializeField] GameObject diamond;
    [SerializeField] GameObject van;
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
        Leaf goToBackDoor = new Leaf("Go To Back Door", GoToBackDoor);

        steal.AddChild(goToBackDoor);
        steal.AddChild(goToDiamond);
        steal.AddChild(goToBackDoor);
        steal.AddChild(goToVan);
        tree.AddChild(steal);

        //tree.PrintTree();
    }

    public Node.Status GotToDiamond()
    {
        return GoToLocation(diamond.transform.position);
    }

    public Node.Status GoToBackDoor()
    {
        return GoToLocation(backDoor.transform.position);
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
