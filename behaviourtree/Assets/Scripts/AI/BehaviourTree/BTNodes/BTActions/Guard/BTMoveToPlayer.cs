using UnityEngine;
using UnityEngine.AI;

public class BTMoveToPlayer : BTMoveToPosition
{
    public BTMoveToPlayer(NavMeshAgent _agent, EventType _eventType, float _moveSpeed, float _keepDistance) 
        : base(_agent, _eventType ,_moveSpeed, _keepDistance)
    {
        
    }
    
    protected override void OnEnter()
    {
        TargetPosition = blackboard.GetVariable<GameObject>(VariableNames.TargetPlayer).transform.position;
        base.OnEnter();
    }
}