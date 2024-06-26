﻿using UnityEngine;

public class BTAttackPlayer : BTBaseNode
{
    private readonly Guard owner;
    private readonly Transform shootingPoint;
    private Transform playerTransform;
    private Gun gun;
    
    public BTAttackPlayer(Gun _gun, Guard _owner, Transform _shootingPoint)
    {
        gun = _gun;
        owner = _owner;
        shootingPoint = _shootingPoint;
    }
    
    public override void SetupBlackboard(Blackboard _blackboard)
    {
        base.SetupBlackboard(_blackboard);
        playerTransform = blackboard.GetVariable<GameObject>(VariableNames.TargetPlayer).transform;
    }

    protected override void OnEnter()
    {
        EventManager.InvokeEvent(EventType.GuardText, GetType().Name);
        EventManager.InvokeEvent(EventType.AttackerTarget, owner.transform);
        if (gun == null) gun = (Gun)owner.item;
    }

    protected override TaskStatus OnUpdate()
    {
        if (gun == null || playerTransform == null)
        {
            Debug.LogError("Gun or player transform is null." + gun + playerTransform);
            return TaskStatus.Failed;
        }
        
        //shoot
        Vector2 direction = (playerTransform.position - owner.transform.position).normalized;
        gun.ShootBullet(direction, shootingPoint);
        
        //rotate
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle); 
        owner.transform.rotation = rotation;
        
        return TaskStatus.Success;
    }
}