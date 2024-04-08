using Unity.VisualScripting;

public class BTPlayerDied : BTBaseNode
{
    private bool isPlayerDead;
    
    public BTPlayerDied()
    {
        EventManager.AddListener<bool>(EventType.OnPlayerDied, PlayerDeadToggle);
    }

    private void PlayerDeadToggle(bool _toggle)
    {
        isPlayerDead = _toggle;
    }
    
    protected override TaskStatus OnUpdate()
    {
        return isPlayerDead ? TaskStatus.Failed : TaskStatus.Success;
    }
}