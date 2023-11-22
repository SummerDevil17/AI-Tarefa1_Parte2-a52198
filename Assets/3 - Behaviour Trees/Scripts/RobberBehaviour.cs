using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : MonoBehaviour
{
    [SerializeField] GameObject diamond;
    [SerializeField] GameObject van;

    NavMeshAgent agent;
    BehaviourTree tree;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        tree = new BehaviourTree();
        Node steal = new Node("Steal Something");
        Leaf goToDiamond = new Leaf("Go To Diamond", GotToDiamond);
        Leaf goToVan = new Leaf("Go To Van", GotToVan);

        steal.AddChild(goToDiamond);
        steal.AddChild(goToVan);
        tree.AddChild(steal);

        tree.PrintTree();
        tree.Process();
    }

    public Node.Status GotToDiamond()
    {
        agent.SetDestination(diamond.transform.position);
        return Node.Status.SUCCESS;
    }

    public Node.Status GotToVan()
    {
        agent.SetDestination(van.transform.position);
        return Node.Status.SUCCESS;
    }

    void Update()
    {

    }
}
