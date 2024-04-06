using UnityEngine;

public class BTGuardHasWeapon : BTBaseNode
{
    private Weapon weapon;
    private readonly Guard guard;
    
    public BTGuardHasWeapon(Weapon _weapon, Guard _guard)
    {
        this.weapon = _weapon;
        this.guard = _guard;
    }

    protected override void OnEnter()
    {
        if (weapon == null)
        {
            weapon = guard.Weapon;
        }
        EventManager.InvokeEvent(EventType.GuardText, GetType().Name);
    }

    protected override TaskStatus OnUpdate()
    {
        if (weapon != null)
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failed;
        } 
    }
}