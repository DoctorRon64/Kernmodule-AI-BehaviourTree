using System.Collections.Generic;
using UnityEngine;

public class BTConditional : BTBaseNode
{
    private readonly List<BTBaseNode> conditionNodes;
    private readonly BTBaseNode actionNode;

    public BTConditional(List<BTBaseNode> _conditionNodes, BTBaseNode _actionNode)
    {
        this.conditionNodes = _conditionNodes;
        this.actionNode = _actionNode;
    }

    public BTConditional(BTBaseNode _conditionNode, BTBaseNode _actionNode)
    {
        this.conditionNodes = new List<BTBaseNode> { _conditionNode };
        this.actionNode = _actionNode;
    }

    protected override TaskStatus OnUpdate()
    {
        foreach (BTBaseNode conditionNode in conditionNodes)
        {
            TaskStatus conditionResult = conditionNode.Tick();
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
        foreach (BTBaseNode conditionNode in conditionNodes)
        {
            conditionNode.SetupBlackboard(_blackboard);
        }
        actionNode.SetupBlackboard(_blackboard);
    }
}