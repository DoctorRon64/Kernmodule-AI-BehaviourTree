using UnityEngine;

public class BTAttackPlayer : BTBaseNode
{
    private readonly Guard owner;
    private Gun gun;
    private Transform playerTransform;
    
    public BTAttackPlayer(Gun _gun, Guard _owner)
    {
        gun = _gun;
        owner = _owner;
    }

    protected override void OnEnter()
    {
        EventManager.InvokeEvent(EventType.GuardText, GetType().Name);
        
        if (gun == null) gun = (Gun)owner.Weapon;
        playerTransform = blackboard.GetVariable<GameObject>(VariableNames.TargetPlayer).transform;
    }

    protected override TaskStatus OnUpdate()
    {
        if (gun == null || playerTransform == null)
        {
            Debug.LogError("Gun or player transform is null." + gun + playerTransform);
            return TaskStatus.Failed;
        }
        
        Transform guardTransform = owner.transform;
        Vector2 direction = (playerTransform.position - guardTransform.position).normalized;

        // Shoot bullet in direction
        gun.ShootBullet(direction, guardTransform);
        
        // Rotate guard to face the player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle); 
        guardTransform.rotation = rotation;
        
        return TaskStatus.Running;
    }
}