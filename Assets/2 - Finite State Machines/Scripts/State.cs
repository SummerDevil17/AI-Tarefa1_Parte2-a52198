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
            if (Random.Range(0, 100) < 10)
            {
                nextState = new Patrol(npcGameObject, npcAgent, npcAnimator, playerTransform);
                stage = EVENT.EXIT;
            }
            base.Update();
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
            currentCheckpointIndex = 0;
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
            base.Update();
        }

        public override void Exit()
        {
            npcAnimator.ResetTrigger("isWalking");
            base.Exit();
        }
    }
}
