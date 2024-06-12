using System.Collections;
using UnityEngine;

public class BTStunNode : BTBaseNode
{
    public BTStunNode()
    {
        
    }
    
    protected override TaskStatus OnUpdate()
    {
        bool isStunned = blackboard.GetVariable<bool>(VariableNames.GuardStunned);
        return isStunned ? TaskStatus.Running : TaskStatus.Success;
    }
}