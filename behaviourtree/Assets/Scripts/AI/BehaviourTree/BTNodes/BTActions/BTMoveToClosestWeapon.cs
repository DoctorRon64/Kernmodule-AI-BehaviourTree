using UnityEngine;

public class BTMoveToClosestWeapon : BTBaseNode
{
    private readonly UnityEngine.AI.NavMeshAgent agent;
    private readonly float moveSpeed;
    private readonly float weaponDetectDistance;
    private Transform closestWeapon;
    private TaskStatus status;

    public BTMoveToClosestWeapon(UnityEngine.AI.NavMeshAgent _agent, float _moveSpeed, float _weaponDetectDistance)
    {
        this.agent = _agent;
        this.moveSpeed = _moveSpeed;
        this.weaponDetectDistance = _weaponDetectDistance;
        this.status = TaskStatus.Running;
    }
    
    protected override void OnEnter()
    {
        agent.speed = moveSpeed;
        agent.stoppingDistance = weaponDetectDistance;
        
        Collider[] colliders = Physics.OverlapSphere(agent.transform.position, weaponDetectDistance);
        float closestDistance = Mathf.Infinity;
        
        foreach (Collider collider in colliders)
        {
            if (!collider.TryGetComponent(out Weapon weapon)) continue;
            float distance = Vector3.Distance(agent.transform.position, weapon.transform.position);

            if (!(distance < closestDistance)) continue;
            closestDistance = distance;
            closestWeapon = weapon.transform;
        }
        
        if (closestWeapon != null)
        {
            agent.SetDestination(closestWeapon.position);
        }
        else
        {
            status = TaskStatus.Failed;
        }
    }

    protected override TaskStatus OnUpdate()
    {
        if (closestWeapon == null)
        {
            return TaskStatus.Failed;
        }

        if (Vector3.Distance(agent.transform.position, closestWeapon.position) <= weaponDetectDistance)
        {
            status = TaskStatus.Success;
        }
        else
        {
            agent.SetDestination(closestWeapon.position);
            status = TaskStatus.Running;
        }

        return status;
    }
}