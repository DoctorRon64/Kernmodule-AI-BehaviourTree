///
/// The Repeater node runs its child a number of times before returning success
///
public class BTRepeater : BTDecorator
{
    private readonly int amount;
    private int currentLoop;

    public BTRepeater(int _amount, BTBaseNode _child) : base(_child)
    {
        this.amount = _amount;
    }

    protected override TaskStatus OnUpdate()
    {
        TaskStatus childStatus = child.Tick();
        if (childStatus != TaskStatus.Running)
        {
            currentLoop++;
            if (amount == -1 || currentLoop < amount)
            {
                child.OnReset();
                return TaskStatus.Running;
            }
        }

        if (amount != -1 && currentLoop >= amount)
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
        currentLoop = 0;
        child.OnReset();
    }
}