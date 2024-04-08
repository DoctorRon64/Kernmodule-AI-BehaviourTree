using UnityEngine;

public class BTIsPlayerInHood : BTBaseNode
{
    protected readonly float PlayerDetectDistance;
    protected readonly Vector3 AgentPosition;
    protected GameObject PlayerObject;

    public BTIsPlayerInHood(float _playerDetectDistance, Vector2 _agentPosition)
    {
        this.PlayerDetectDistance = _playerDetectDistance;
        this.AgentPosition = _agentPosition;
    }
    
    
    public override void SetupBlackboard(Blackboard _blackboard)
    {
        base.SetupBlackboard(_blackboard);
        PlayerObject = blackboard.GetVariable<GameObject>(  VariableNames.TargetPlayer);
    }

    protected override void OnEnter()
    {
        EventManager.InvokeEvent(EventType.GuardText, GetType().Name);
    }

    protected override TaskStatus OnUpdate()
    {
        if (PlayerObject == null)  return TaskStatus.Failed;
        
        Vector3 playerPosition = PlayerObject.transform.position;
        float distanceToPlayer = Vector2.Distance(AgentPosition, playerPosition);

        if (distanceToPlayer <= PlayerDetectDistance)
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failed;
        }
    }
}
