using UnityEngine;
using UnityEngine.AI;

public class BTMoveToWeapon : BTMoveToPosition
{
    public BTMoveToWeapon(NavMeshAgent _agent, float _moveSpeed, float _keepDistance) : base(_agent, _moveSpeed, _keepDistance)
    {
    }

    protected override void OnEnter()
    {
        base.OnEnter();
        TargetPosition = blackboard.GetVariable<GameObject>(VariableNames.TargetWeapon).transform.position;
    }
}