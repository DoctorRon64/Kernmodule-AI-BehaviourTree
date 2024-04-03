using UnityEngine;

[CreateAssetMenu(fileName = "FloatValue_" , menuName = "Variables/FloatValue")]
public class FloatValue : BaseScriptableObject
{
    public VariableType Type;

    [SerializeField] private float value;
    public float Value { get => value; set => this.value = value; }

    [SerializeField] private float minValue = 0;
    public float MinValue { get { return minValue; } }

    [SerializeField] private float maxValue;
    public float MaxValue { get { return maxValue; } }

}
