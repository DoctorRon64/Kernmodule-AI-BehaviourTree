using System;

public class BTAction : BTBaseNode
{
    private readonly Func<TaskStatus> action;

    public BTAction(Func<TaskStatus> _action)
    {
        this.action = _action;
    }

    protected override TaskStatus OnUpdate()
    {
        return action.Invoke();
    }
}