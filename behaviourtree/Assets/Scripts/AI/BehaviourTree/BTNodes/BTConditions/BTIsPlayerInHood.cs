using UnityEngine;

public class BTIsPlayerInHood : BTBaseNode
{
    private readonly float playerDetectDistance;
    private Vector3 agentPosition;

    public BTIsPlayerInHood(float _playerDetectDistance, Vector2 _agentPosition)
    {
        this.playerDetectDistance = _playerDetectDistance;
        this.agentPosition = _agentPosition;
    }

    protected override void OnEnter()
    {
    }

    protected override TaskStatus OnUpdate()
    {
        Vector3 playerPosition = blackboard.GetVariable<Transform>(VariableNames.PlayerTransform).position;
        float distanceToPlayer = Vector2.Distance(agentPosition, playerPosition);

        if (distanceToPlayer <= playerDetectDistance)
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failed;
        }
    }
}
