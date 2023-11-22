using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree : Node
{
    public BehaviourTree()
    {
        name = "Tree";
    }

    public BehaviourTree(string behaviourName)
    {
        name = behaviourName;
    }

    public void PrintTree()
    {
        Debug.Log("Root Node: " + this.name);
        foreach (Node child in this.children)
        {
            Debug.Log("Child Node: " + child.name);
            foreach (Node grandchild in child.children)
            {
                Debug.Log("Grandchild Node: " + grandchild.name);
            }
        }
    }
}
