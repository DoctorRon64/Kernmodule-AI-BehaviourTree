using UnityEngine;
using UnityEngine.AI;

public class BTGetClosestWeaponPos : BTBaseNode
{
    private readonly NavMeshAgent agent;
    private readonly float detectionRadius;
    private GameObject closestWeapon;
    private TaskStatus status;
    
    public BTGetClosestWeaponPos(NavMeshAgent _agent, float _detectionRadius)
    {
        agent = _agent;
        detectionRadius = _detectionRadius;
        status = TaskStatus.Running;
    }
    
    protected override void OnEnter()
    {
        EventManager.InvokeEvent(EventType.GuardText, GetType().Name);
        agent.stoppingDistance = detectionRadius;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(agent.transform.position, detectionRadius);

        foreach (Collider2D collider in colliders)
        {
            if (!collider.TryGetComponent(out Item weapon)) continue;
            if (!(Vector2.Distance(agent.transform.position, weapon.transform.position) <= detectionRadius)) continue;
            
            closestWeapon = weapon.gameObject;
        }
        Debug.Log("weapon nearby! on pos: " + closestWeapon.transform.position);
        
        if (closestWeapon != null)
        {
            blackboard.SetVariable(VariableNames.TargetWeapon, closestWeapon);
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