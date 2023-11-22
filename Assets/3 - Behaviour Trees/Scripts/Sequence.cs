using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence(string sequenceName)
    {
        name = sequenceName;
    }

    public override Status Process()
    {
        Debug.Log("Running -> " + name + " // Status -> " + status);

        Status childStatus = children[currentChildIndex].Process();
        if (childStatus == Status.RUNNING) { return Status.RUNNING; }

        if (childStatus == Status.FAILURE) { return Status.FAILURE; }

        currentChildIndex++;
        if (currentChildIndex >= children.Count)
        {
            currentChildIndex = 0;
            return Status.SUCCESS;
        }
        return Status.RUNNING;
    }
}
