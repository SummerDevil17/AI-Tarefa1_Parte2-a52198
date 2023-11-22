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

    struct NodeLevel
    {
        public int level;
        public Node node;
    }

    public void PrintTree()
    {
        string treeDebugLog = "";
        Stack<NodeLevel> nodeStack = new Stack<NodeLevel>();
        Node currentNode = this;

        nodeStack.Push(new NodeLevel { level = 0, node = currentNode });

        while (nodeStack.Count != 0)
        {
            NodeLevel nextNode = nodeStack.Pop();
            treeDebugLog += ">" + new string('-', nextNode.level) + nextNode.node.name + "\n";

            for (int i = nextNode.node.children.Count - 1; i >= 0; i--)
            {
                nodeStack.Push(new NodeLevel { level = nextNode.level + 1, node = nextNode.node.children[i] });
            }
        }
        Debug.Log(treeDebugLog);

        /*Debug.Log("Root Node: " + this.name);
        foreach (Node child in this.children)
        {
            Debug.Log("Child Node: " + child.name);
            foreach (Node grandchild in child.children)
            {
                Debug.Log("Grandchild Node: " + grandchild.name);
            }
        }*/
    }
}
