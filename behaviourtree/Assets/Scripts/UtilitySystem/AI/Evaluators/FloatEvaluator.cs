using UnityEngine;

[CreateAssetMenu(fileName = "FloatEvaluator", menuName = "Evaluators/FloatEvaluator")]
public class FloatEvaluator : UtilityEvaluator
{
    private FloatValue floatValue;

    public override void OnInitialize(BlackBoard bb)
    {
        floatValue = bb.GetFloatVariableValue(VariableType);
    }

    public override float GetMaxValue()
    {
        return floatValue.MaxValue;
    }

    public override float GetValue()
    {
        return floatValue.Value;
    }
}
