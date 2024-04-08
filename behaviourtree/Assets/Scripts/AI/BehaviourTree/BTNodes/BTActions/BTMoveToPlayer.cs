using UnityEngine;
using UnityEngine.AI;

public class BTMoveToPlayer : BTMoveToPosition
{
    public BTMoveToPlayer(NavMeshAgent _agent, float _moveSpeed, float _keepDistance)
        : base(_agent, _moveSpeed, _keepDistance)
    {
        
    }
    
    protected override void OnEnter()
    {
        base.OnEnter();
        TargetPosition = blackboard.GetVariable<GameObject>(VariableNames.TargetPlayer).transform.position;
    }
}