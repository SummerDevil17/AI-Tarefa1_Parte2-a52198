using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : Node
{
    public delegate Status Tick();
    public Tick ProcessMethod;

    public Leaf() { }

    public Leaf(string leafName, Tick method)
    {
        name = leafName;
        ProcessMethod = method;
    }

    public override Status Process()
    {
        if (ProcessMethod != null)
        {
            Debug.Log("Running -> " + name + " // Status -> " + status);
            return ProcessMethod();
        }

        return Status.FAILURE;
    }
}
