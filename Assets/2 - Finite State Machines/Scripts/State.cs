using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public class State
{
    public enum STATE { IDLE, PATROL, PURSUE, ATTACK, SLEEP }

    public enum EVENT { ENTER, UPDATE, EXIT }

    public STATE name;
    protected EVENT stage;
    protected GameObject npcGameObject;
    protected NavMeshAgent npcAgent;
    protected Animator npcAnimator;
    protected Transform playerTransform;
    protected State nextState;

    float visionDistance = 10f;
    float visionAngle = 30f;
    float shootDistance = 7f;

    public State(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
    {
        npcGameObject = _npc;
        npcAgent = _agent;
        npcAnimator = _anim;
        stage = EVENT.ENTER;
        playerTransform = _player;
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }

    public State Process()
    {
        if (stage == EVENT.ENTER) { Enter(); }
        if (stage == EVENT.UPDATE) { Update(); }
        if (stage == EVENT.EXIT) { Exit(); return nextState; }

        return this;
    }
}
