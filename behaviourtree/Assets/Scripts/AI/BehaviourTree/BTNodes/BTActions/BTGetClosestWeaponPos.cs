using UnityEngine;
using UnityEngine.AI;

public class BTGetClosestWeaponPos : BTBaseNode
{
    private readonly NavMeshAgent agent;
    private readonly float detectionRadius;
    private Transform closestWeapon;
    private TaskStatus status;
    
    public BTGetClosestWeaponPos(NavMeshAgent _agent, float _detectionRadius)
    {
        Debug.Log(this.GetType().Name);
        
        agent = _agent;
        detectionRadius = _detectionRadius;
        status = TaskStatus.Running;
    }
    
    protected override void OnEnter()
    {
        agent.stoppingDistance = detectionRadius;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(agent.transform.position, detectionRadius);

        foreach (Collider2D collider in colliders)
        {
            if (!collider.TryGetComponent(out Weapon weapon)) continue;
            Debug.Log(weapon + "weapon found!");
            if (!(Vector2.Distance(agent.transform.position, weapon.transform.position) <= detectionRadius)) continue;
            Debug.Log(weapon + "weapon nearby!");
            
            closestWeapon = weapon.transform;
        }
        
        if (closestWeapon != null)
        {
            blackboard.SetVariable(VariableNames.TargetWeaponPosition, closestWeapon.position);
        }
        else
        {
            Debug.LogWarning("No nearby weapons found.");
        }

        status = TaskStatus.Success;
    }
    
    protected override TaskStatus OnUpdate()
    {
        return status;
    }
}