using UnityEngine;
using UnityEngine.AI;

public class BTMoveToPlayer : BTBaseNode
{
    private readonly NavMeshAgent agent;
    private readonly float moveSpeed;
    private readonly float keepDistance;
    
    private Vector3 targetPosition;

    public BTMoveToPlayer(NavMeshAgent _agent, float _moveSpeed, float _keepDistance)
    {
        this.agent = _agent;
        this.moveSpeed = _moveSpeed;
        this.keepDistance = _keepDistance;
    }

    protected override void OnEnter()
    {
        agent.speed = moveSpeed;
        agent.stoppingDistance = keepDistance;
        targetPosition = blackboard.GetVariable<Transform>(VariableNames.PlayerTransform).position;
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