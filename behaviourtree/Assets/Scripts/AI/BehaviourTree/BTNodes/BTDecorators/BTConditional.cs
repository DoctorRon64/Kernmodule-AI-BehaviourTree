using System;

public class BTConditional : BTBaseNode
{
    private readonly Func<bool> condition;
    private readonly BTBaseNode actionNode;

    public BTConditional(Func<bool> _condition, BTBaseNode _actionNode)
    {
        condition = _condition;
        actionNode = _actionNode;
    }

    protected override TaskStatus OnUpdate()
    {
        if (condition.Invoke())
        {
            return actionNode.Tick();
        }
        else
        {
            return TaskStatus.Failed;
        }
    }

    public override void SetupBlackboard(Blackboard _blackboard)
    {
        base.SetupBlackboard(_blackboard);
        actionNode.SetupBlackboard(_blackboard);
    }
}