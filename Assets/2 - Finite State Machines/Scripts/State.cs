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
    float attackDistance = 7f;

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

    public bool CanSeePlayer()
    {
        Vector3 direction = playerTransform.position - npcGameObject.transform.position;
        float angle = Vector3.Angle(direction, npcGameObject.transform.forward);

        if (direction.magnitude < visionDistance && angle < visionAngle)
        {
            return true;
        }
        return false;
    }

    public bool CanAttackPlayer()
    {
        Vector3 direction = playerTransform.position - npcGameObject.transform.position;

        if (direction.magnitude < attackDistance) { return true; }

        return false;
    }
}

public class Idle : State
{
    public Idle(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
    : base(_npc, _agent, _anim, _player)
    {
        name = STATE.IDLE;
    }

    public override void Enter()
    {
        npcAnimator.SetTrigger("isIdle");
        base.Enter();
    }

    public override void Update()
    {
        if (CanSeePlayer())
        {
            nextState = new Pursue(npcGameObject, npcAgent, npcAnimator, playerTransform);
            stage = EVENT.EXIT;
        }

        if (Random.Range(0, 100) < 10)
        {
            nextState = new Patrol(npcGameObject, npcAgent, npcAnimator, playerTransform);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        npcAnimator.ResetTrigger("isIdle");
        base.Exit();
    }
}

public class Patrol : State
{
    int currentCheckpointIndex = -1;

    public Patrol(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
    : base(_npc, _agent, _anim, _player)
    {
        name = STATE.PATROL;
        npcAgent.speed = 2f;
        npcAgent.isStopped = false;
    }

    public override void Enter()
    {
        float shortestDistance = Mathf.Infinity;
        for (int i = 0; i < GameEnvironment.Singleton.Checkpoints.Count; i++)
        {
            GameObject thisWaypoint = GameEnvironment.Singleton.Checkpoints[i];
            float distance = Vector3.Distance(npcGameObject.transform.position, thisWaypoint.transform.position);

            if (distance < shortestDistance) { currentCheckpointIndex = i - 1; shortestDistance = distance; }
        }

        npcAnimator.SetTrigger("isWalking");
        base.Enter();
    }

    public override void Update()
    {
        if (npcAgent.remainingDistance < 1f)
        {
            if (currentCheckpointIndex >= GameEnvironment.Singleton.Checkpoints.Count - 1)
            { currentCheckpointIndex = 0; }
            else { currentCheckpointIndex++; }

            npcAgent.SetDestination(GameEnvironment.Singleton.Checkpoints[currentCheckpointIndex].transform.position);
        }

        if (CanSeePlayer())
        {
            nextState = new Pursue(npcGameObject, npcAgent, npcAnimator, playerTransform);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        npcAnimator.ResetTrigger("isWalking");
        base.Exit();
    }
}

public class Pursue : State
{
    public Pursue(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
    : base(_npc, _agent, _anim, _player)
    {
        name = STATE.PURSUE;
        npcAgent.speed = 5f;
        npcAgent.isStopped = false;
    }

    public override void Enter()
    {
        npcAnimator.SetTrigger("isRunning");
        base.Enter();
    }

    public override void Update()
    {
        npcAgent.SetDestination(playerTransform.position);

        if (!npcAgent.hasPath) { return; }

        if (CanAttackPlayer())
        {
            nextState = new Attack(npcGameObject, npcAgent, npcAnimator, playerTransform);
            stage = EVENT.EXIT;
        }
        else if (!CanSeePlayer())
        {
            nextState = new Patrol(npcGameObject, npcAgent, npcAnimator, playerTransform);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        npcAnimator.ResetTrigger("isRunning");
        base.Exit();
    }
}

public class Attack : State
{
    float rotationSpeed = 2f;
    AudioSource shootAudioSource;

    public Attack(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
    : base(_npc, _agent, _anim, _player)
    {
        name = STATE.ATTACK;
        shootAudioSource = _npc.GetComponent<AudioSource>();
    }

    public override void Enter()
    {
        npcAnimator.SetTrigger("isShooting");
        npcAgent.isStopped = true;
        shootAudioSource.Play();
        base.Enter();
    }

    public override void Update()
    {
        Vector3 direction = playerTransform.position - npcGameObject.transform.position;
        float angle = Vector3.Angle(direction, npcGameObject.transform.forward);
        direction.y = 0f;

        npcAgent.transform.rotation = Quaternion.Slerp(npcGameObject.transform.rotation,
                                                        Quaternion.LookRotation(direction),
                                                        Time.deltaTime * rotationSpeed);

        if (!CanAttackPlayer())
        {
            nextState = new Idle(npcGameObject, npcAgent, npcAnimator, playerTransform);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        npcAnimator.ResetTrigger("isShooting");
        shootAudioSource.Stop();
        base.Exit();
    }
}
