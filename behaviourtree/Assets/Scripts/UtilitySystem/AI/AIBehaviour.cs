using System.Linq;
using UnityEngine;

public abstract class AIBehaviour : MonoBehaviour
{
    [SerializeField] public UtilityEvaluator[] utilities; 
     
    public void OnInitialize(BlackBoard bb)
    {
        foreach(var utility in utilities)
        {
            utility.OnInitialize(bb);
        }
    }

    public float GetNormalizedScore()
    {
        return Mathf.Clamp01(utilities.ToList().Sum(x => x.GetNormalizedScore()) / utilities.Length);
    }

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void Execute() { }
}
