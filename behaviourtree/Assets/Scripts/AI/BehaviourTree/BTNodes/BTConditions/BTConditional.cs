public class BTConditional : BTBaseNode
{
    private readonly BTBaseNode conditionNode;
    private readonly BTBaseNode actionNode;

    public BTConditional(BTBaseNode conditionNode, BTBaseNode actionNode)
    {
        this.conditionNode = conditionNode;
        this.actionNode = actionNode;
    }

    protected override TaskStatus OnUpdate()
    {
        TaskStatus conditionResult = conditionNode.Tick();

        if (conditionResult == TaskStatus.Success)
        {
            return actionNode.Tick();
        }
        else
        {
            return TaskStatus.Failed;
        }
    }
}