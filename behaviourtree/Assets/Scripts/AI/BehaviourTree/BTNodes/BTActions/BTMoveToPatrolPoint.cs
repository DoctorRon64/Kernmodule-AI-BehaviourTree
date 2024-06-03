using UnityEngine;
using UnityEngine.AI;

public class BTMoveToPatrolPoint : BTMoveToPosition
{
    
    public BTMoveToPatrolPoint(NavMeshAgent _agent, float _moveSpeed, float _keepDistance) 
        : base(_agent, _moveSpeed, _keepDistance)
    {
    }
    
    protected override void OnEnter()
    {
        TargetPosition = blackboard.GetVariable<Vector3>(VariableNames.TargetPatrolPosition); 
        base.OnEnter();
    }
}