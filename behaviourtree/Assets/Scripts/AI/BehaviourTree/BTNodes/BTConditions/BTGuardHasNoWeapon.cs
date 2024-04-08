public class BTGuardHasNoWeapon : BTBaseNode
{
    private Weapon weapon;
    private readonly Guard guard;
    
    public BTGuardHasNoWeapon(Weapon _weapon, Guard _guard)
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
            return TaskStatus.Failed;
        }
        else
        {
            return TaskStatus.Success;
        } 
    }
}