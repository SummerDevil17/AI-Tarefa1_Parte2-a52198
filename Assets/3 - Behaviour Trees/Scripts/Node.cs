using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public enum Status { SUCCESS, RUNNING, FAILURE }
    public Status status;

    public List<Node> children = new List<Node>();
    public int currentChild = 0;
    public string name;

    public Node() { }

    public Node(string nodeName)
    {
        name = nodeName;
    }

    public void AddChild(Node childNode)
    {
        children.Add(childNode);
    }
}
