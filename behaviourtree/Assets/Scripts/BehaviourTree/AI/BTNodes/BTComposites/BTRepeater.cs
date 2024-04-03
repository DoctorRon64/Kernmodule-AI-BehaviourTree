///
/// The Repeater node runs its child a number of times before returning success
///
public class BTRepeater : BTDecorator
{
    private int amount = 0;
    private int currentLoop = 0;
    public BTRepeater(int amount, BTBaseNode child) : base(child){ this.amount = amount; }

    protected override TaskStatus OnUpdate()
    {
        if(child.Tick() != TaskStatus.Running)
        {
            currentLoop++;
        }
        if (currentLoop >= amount)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }

    protected override void OnEnter()
    {
        currentLoop = 0;
    }

    protected override void OnExit()
    {
        currentLoop = 0;
    }

    public override void OnReset()
    {
        amount = 0;
        child.OnReset();
    }
}

