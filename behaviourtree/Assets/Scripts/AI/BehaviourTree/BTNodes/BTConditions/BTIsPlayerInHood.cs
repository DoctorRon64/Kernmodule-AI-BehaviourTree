﻿using UnityEngine;

public class BTIsPlayerInHood : BTBaseNode
{
    private readonly float playerDetectDistance;
    private readonly Vector3 agentPosition;
    private GameObject playerObject;

    public BTIsPlayerInHood(float _playerDetectDistance, Vector2 _agentPosition)
    {
        this.playerDetectDistance = _playerDetectDistance;
        this.agentPosition = _agentPosition;
        playerObject = null;
    }

    protected override void OnEnter()
    {
        EventManager.InvokeEvent(EventType.GuardText, GetType().Name);
        playerObject = blackboard.GetVariable<GameObject>(  VariableNames.TargetPlayer);
    }

    protected override TaskStatus OnUpdate()
    {
        if (playerObject == null)  return TaskStatus.Failed;
        
        Vector3 playerPosition = playerObject.transform.position;
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