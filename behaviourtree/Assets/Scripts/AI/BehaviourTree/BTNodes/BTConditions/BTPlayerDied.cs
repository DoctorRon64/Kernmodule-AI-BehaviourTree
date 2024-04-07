public class BTPlayerDied : BTBaseNode
{
    private bool isPlayerDead;
    
    public BTPlayerDied()
    {
        EventManager.Parameterless.AddListener(EventType.onPlayerDied, PlayerDeadToggle);
    }

    private void PlayerDeadToggle()
    {
        isPlayerDead = !isPlayerDead;
    }
    
    protected override TaskStatus OnUpdate()
    {
        return isPlayerDead ? TaskStatus.Failed : TaskStatus.Success;
    }
}