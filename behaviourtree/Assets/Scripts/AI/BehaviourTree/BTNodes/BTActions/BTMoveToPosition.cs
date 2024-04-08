using System.Collections;
using System.Collections.Generic;
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
        agent.speed = moveSpeed;
        agent.stoppingDistance = keepDistance;
    }

    protected override TaskStatus OnUpdate()
    {
        if (agent == null) { return TaskStatus.Failed; }
        if (agent.pathPending) { return TaskStatus.Running; }
        if (agent.hasPath && agent.path.status == NavMeshPathStatus.PathInvalid) { return TaskStatus.Failed; }
        
        agent.SetDestination(TargetPosition);
            
        //rotate towards targetpos
        Vector2 direction = (TargetPosition - agent.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        agent.transform.rotation = rotation;

        if(Vector2.Distance(agent.transform.position, TargetPosition) <= keepDistance)
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
