using System.Collections.Generic;
using System.Linq;

public enum TaskStatus { Success, Failed, Running }
public abstract class BTBaseNode
{
    //Members
    protected Blackboard blackboard;
    private bool wasEntered = false;

    //Public Methods
    public virtual void OnReset() { }

    public TaskStatus Tick()
    {
        if (!wasEntered)
        {
            OnEnter();
            wasEntered = true;
        }

        var result = OnUpdate();
        if(result != TaskStatus.Running)
        {
            OnExit();
            wasEntered = false;
        }
        return result;
    }

    public virtual void SetupBlackboard(Blackboard _blackboard)
    {
        this.blackboard = _blackboard;
    }

    //Protected Methods
    protected abstract TaskStatus OnUpdate();
    protected virtual void OnEnter() { }
    protected virtual void OnExit() { }
}

public abstract class BTComposite : BTBaseNode
{
    protected BTBaseNode[] children;
    public BTComposite (params BTBaseNode[] _children)
    {
        this.children = _children;
    }

    public override void SetupBlackboard(Blackboard _blackboard)
    {
        base.SetupBlackboard(_blackboard);
        foreach(BTBaseNode node in children)
        {
            node.SetupBlackboard(_blackboard);
        }
    }
}

public abstract class BTDecorator : BTBaseNode
{
    protected BTBaseNode child;
    public BTDecorator(BTBaseNode _child)
    {
        this.child = _child;
    }

    public override void SetupBlackboard(Blackboard _blackboard)
    {
        base.SetupBlackboard(_blackboard);
        child.SetupBlackboard(_blackboard);
    }
}
