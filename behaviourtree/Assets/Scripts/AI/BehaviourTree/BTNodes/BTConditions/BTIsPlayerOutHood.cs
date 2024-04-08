using UnityEngine;

public class BTIsPlayerOutHood : BTIsPlayerInHood
{
    public BTIsPlayerOutHood(float _playerDetectDistance, Vector2 _agentPosition) : base(_playerDetectDistance, _agentPosition)
    {
        
    }
    
    protected override TaskStatus OnUpdate()
    {
        if (PlayerObject == null)  return TaskStatus.Failed;
        
        Vector3 playerPosition = PlayerObject.transform.position;
        float distanceToPlayer = Vector2.Distance(AgentPosition, playerPosition);

        if (distanceToPlayer >= PlayerDetectDistance)
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failed;
        }
    }
}