using System.Collections.Generic;
using UnityEngine;

public class BTConditional : BTBaseNode
{
    private readonly BTBaseNode conditionNode;
    private readonly BTBaseNode actionNode;

    public BTConditional(BTBaseNode _conditionNode, BTBaseNode _actionNode)
    {
        this.conditionNode = _conditionNode;
        this.actionNode = _actionNode;
    }

    protected override TaskStatus OnUpdate()
    {
        TaskStatus conditionResult = conditionNode.Tick();
        if (conditionResult != TaskStatus.Success)
        {
            return TaskStatus.Failed;
        }

        return actionNode.Tick();
    }
    
    public override void SetupBlackboard(Blackboard _blackboard)
    {
        base.SetupBlackboard(_blackboard);
        conditionNode.SetupBlackboard(_blackboard);
        actionNode.SetupBlackboard(_blackboard);
    }
}