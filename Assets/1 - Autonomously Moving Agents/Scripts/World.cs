using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class World
{
    private static readonly World instance = new World();
    private static GameObject[] hidingGameObj;

    static World()
    {
        hidingGameObj = GameObject.FindGameObjectsWithTag("hide");
    }

    private World() { }

    public static World Instance
    {
        get { return instance; }
    }

    public GameObject[] GetHidingGameObj()
    {
        return hidingGameObj;
    }
}
