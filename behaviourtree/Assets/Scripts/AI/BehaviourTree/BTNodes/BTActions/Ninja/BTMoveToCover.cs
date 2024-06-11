using UnityEngine;
using UnityEngine.AI;

public class BTMoveToCover : BTMoveToPosition
{
    public BTMoveToCover(NavMeshAgent _agent, EventType _eventType, float _moveSpeed, float _keepDistance) 
        : base(_agent, _eventType ,_moveSpeed, _keepDistance)
    {
        
    }
    
    protected override void OnEnter()
    {
        TargetPosition = blackboard.GetVariable<Vector3>(VariableNames.CoverPos);
        base.OnEnter();
    }
}