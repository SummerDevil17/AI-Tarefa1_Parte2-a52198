using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    public Selector(string sequenceName)
    {
        name = sequenceName;
    }

    public override Status Process()
    {
        Debug.Log("Running -> " + name + " // Status -> " + status);

        Status childStatus = children[currentChildIndex].Process();
        if (childStatus == Status.RUNNING) { return Status.RUNNING; }

        if (childStatus == Status.SUCCESS)
        {
            currentChildIndex = 0;
            return Status.SUCCESS;
        }

        currentChildIndex++;
        if (currentChildIndex >= children.Count)
        {
            currentChildIndex = 0;
            return Status.FAILURE;
        }
        return Status.RUNNING;
    }
}
