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
            return ProcessMethod();

        return Status.FAILURE;
    }
}
