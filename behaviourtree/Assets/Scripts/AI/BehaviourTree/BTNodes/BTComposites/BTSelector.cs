using System;

public class BTSelector : BTComposite
{
    public BTSelector(params BTBaseNode[] _children) : base(_children)
    {
        
    }

    protected override TaskStatus OnUpdate()
    {
        foreach (BTBaseNode t in children)
        {
            TaskStatus result = t.Tick();
            switch (result)
            {
                case TaskStatus.Success: 
                    return TaskStatus.Success;
                case TaskStatus.Failed: 
                    continue;
                case TaskStatus.Running: 
                    return TaskStatus.Running;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return TaskStatus.Failed;
    }

    public override void OnReset()
    {
        foreach (BTBaseNode c in children)
        {
            c.OnReset();
        }
    }
}