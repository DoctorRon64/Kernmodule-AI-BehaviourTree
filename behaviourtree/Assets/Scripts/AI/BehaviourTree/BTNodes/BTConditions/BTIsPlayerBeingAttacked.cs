using UnityEngine;

public class BTIsPlayerBeingAttacked : BTBaseNode
{
    private bool isPlayerBeingAttacked;

    public BTIsPlayerBeingAttacked()
    {
        EventManager.AddListener(EventType.OnPlayerAttack, HandlePlayerAttacked);
    }
    
    private void HandlePlayerAttacked()
    {
        Debug.Log("Player Is Being Attacked");
        isPlayerBeingAttacked = true;
    }

    protected override TaskStatus OnUpdate()
    {
        if (isPlayerBeingAttacked)
        {
            EventManager.InvokeEvent(EventType.OnPlayerAttack);
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failed;
        }
    }
}