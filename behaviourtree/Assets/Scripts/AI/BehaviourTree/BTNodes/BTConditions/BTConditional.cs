using System.Collections.Generic;

public class BTConditional : BTBaseNode
{
    private readonly List<BTBaseNode> conditionNodes;
    private readonly BTBaseNode actionNode;

    public BTConditional(BTBaseNode[] _nodes, BTBaseNode _actionNode)
    {
        conditionNodes = new List<BTBaseNode>(_nodes.Length);
        foreach (BTBaseNode node in _nodes)
        {
            conditionNodes.Add(node);
        }
        actionNode = _actionNode;
    }
    
    public BTConditional(BTBaseNode _conditionNode, BTBaseNode _actionNode)
    {
        conditionNodes = new List<BTBaseNode> { _conditionNode };
        actionNode = _actionNode;
    }

    protected override TaskStatus OnUpdate()
    {
        foreach (BTBaseNode node in conditionNodes)
        {
            TaskStatus conditionResult = node.Tick();
            if (conditionResult != TaskStatus.Success)
            {
                return TaskStatus.Failed;
            }
        }

        return actionNode.Tick();
    }
    
    public override void SetupBlackboard(Blackboard _blackboard)
    {
        base.SetupBlackboard(_blackboard);
        foreach (BTBaseNode node in conditionNodes)
        {
            node.SetupBlackboard(_blackboard);
        }
        actionNode.SetupBlackboard(_blackboard);
    }
}