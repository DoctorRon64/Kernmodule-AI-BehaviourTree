using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics.Internal;
using UnityEngine;
using UnityEngine.AI;

public class BTMoveToPosition : BTBaseNode
{
    private readonly NavMeshAgent agent;
    private readonly float moveSpeed;
    private readonly float keepDistance;
    protected Vector3 TargetPosition;

    protected BTMoveToPosition(NavMeshAgent _agent, float _moveSpeed, float _keepDistance)
    {
        this.agent = _agent;
        this.moveSpeed = _moveSpeed;
        this.keepDistance = _keepDistance;
    }

    protected override void OnEnter()
    {
        EventManager.InvokeEvent(EventType.GuardText, GetType().Name);
        
        if (agent == null) { Debug.LogError("NavMeshAgent is null!"); return; }

        //TargetPosition.z = agent.transform.position.z;
        agent.speed = moveSpeed;
        agent.updatePosition = false;
        agent.stoppingDistance = keepDistance;
        agent.SetDestination(TargetPosition);
        
        //if (!NavMesh.SamplePosition(TargetPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas)) { Debug.LogError("Invalid Target Position!"); return; }
        
        Debug.Log(moveSpeed + "speed");
        Debug.Log(agent.stoppingDistance + "Distancestop");
    }

    protected override TaskStatus OnUpdate()
    {
        if (agent == null) { Debug.LogWarning("agent = null"); return TaskStatus.Failed; }
        if (agent.pathPending) { Debug.LogWarning("pathpending = running"); return TaskStatus.Running;}
        if (agent.isStopped) { Debug.LogWarning("Agent is stopped!"); return TaskStatus.Failed; }

        TargetPosition.z = 0;
        agent.speed = moveSpeed;
        agent.stoppingDistance = keepDistance;
        //Debug.Log("target destiiniatiiion = " + TargetPosition);
        if ((Vector2)agent.destination != (Vector2)TargetPosition)
        {
            agent.SetDestination(TargetPosition);
        }
        agent.transform.position = agent.nextPosition;

        //rotate towards targetpos
        Vector2 direction = (TargetPosition - agent.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        agent.transform.rotation = rotation;

        if (agent.remainingDistance <= keepDistance)// Vector2.Distance(agent.transform.position, TargetPosition) <= keepDistance)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }

}

public class BTGetNextPatrolPosition : BTBaseNode
{
    private readonly Transform[] wayPoints;
    public BTGetNextPatrolPosition(Transform[] _wayPoints) 
    {
        this.wayPoints = _wayPoints;
    }

    protected override void OnEnter()
    {
        EventManager.InvokeEvent(EventType.GuardText, "\"Get Next Patrol Pos\"");
        
        int currentIndex = blackboard.GetVariable<int>(VariableNames.CurrentPatrolIndex);
        currentIndex++;
        if(currentIndex >= wayPoints.Length)
        {
            currentIndex = 0;
        }
        blackboard.SetVariable<int>(VariableNames.CurrentPatrolIndex, currentIndex);
        blackboard.SetVariable<Vector3>(VariableNames.TargetPatrolPosition, wayPoints[currentIndex].position);
    }

    protected override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}