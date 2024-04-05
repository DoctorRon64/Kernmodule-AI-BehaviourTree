using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTMoveToPosition : BTBaseNode
{
    private readonly NavMeshAgent agent;
    private readonly float moveSpeed;
    private readonly float keepDistance;
    private Vector3 targetPosition;
    private readonly string bBtargetPosition;

    public BTMoveToPosition(NavMeshAgent _agent, float _moveSpeed, string _bBtargetPosition, float _keepDistance)
    {
        this.agent = _agent;
        this.moveSpeed = _moveSpeed;
        this.bBtargetPosition = _bBtargetPosition;
        this.keepDistance = _keepDistance;
    }

    protected override void OnEnter()
    {
        agent.speed = moveSpeed;
        agent.stoppingDistance = keepDistance;
        targetPosition = blackboard.GetVariable<Vector3>(bBtargetPosition);
    }

    protected override TaskStatus OnUpdate()
    {
        if (agent == null) { return TaskStatus.Failed; }
        if (agent.pathPending) { return TaskStatus.Running; }
        if (agent.hasPath && agent.path.status == NavMeshPathStatus.PathInvalid) { return TaskStatus.Failed; }

        if (agent.pathEndPosition != targetPosition)
        {
            agent.SetDestination(targetPosition);
            
            //rotate towards targetpos
            Vector2 direction = (targetPosition - agent.transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            agent.transform.rotation = rotation;
        }

        if(Vector2.Distance(agent.transform.position, targetPosition) <= keepDistance)
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
        Debug.Log("patrol fase");
        
        int currentIndex = blackboard.GetVariable<int>(VariableNames.CurrentPatrolIndex);
        currentIndex++;
        if(currentIndex >= wayPoints.Length)
        {
            currentIndex = 0;
        }
        blackboard.SetVariable<int>(VariableNames.CurrentPatrolIndex, currentIndex);
        blackboard.SetVariable<Vector3>(VariableNames.TargetPosition, wayPoints[currentIndex].position);
    }

    protected override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}
