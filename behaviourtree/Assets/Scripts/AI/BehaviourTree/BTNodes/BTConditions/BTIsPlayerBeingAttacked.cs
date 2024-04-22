public class BTIsPlayerBeingAttacked : BTBaseNode
{
    private bool isPlayerBeingAttacked;

    public BTIsPlayerBeingAttacked()
    {
        EventManager.Parameterless.AddListener(EventType.OnPlayerAttack, HandlePlayerAttacked);
    }

    private void HandlePlayerAttacked()
    {
        isPlayerBeingAttacked = true;
    }

    protected override TaskStatus OnUpdate()
    {
        if (isPlayerBeingAttacked)
        {
            EventManager.Parameterless.InvokeEvent(EventType.OnPlayerAttack);
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failed;
        }
    }
}