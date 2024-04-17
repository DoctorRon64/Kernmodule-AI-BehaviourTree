public class BTPlayerAlive : BTBaseNode
{
    private bool isPlayerDead;
    
    public BTPlayerAlive()
    {
        EventManager.AddListener<bool>(EventType.OnPlayerDied, PlayerDeadToggle);
    }

    private void PlayerDeadToggle(bool _toggle)
    {
        isPlayerDead = _toggle;
    }
    
    protected override TaskStatus OnUpdate()
    {
        return isPlayerDead ? TaskStatus.Success : TaskStatus.Failed;
    }
}