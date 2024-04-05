using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour, IDamageable
{
    public int MaxHealth { get; } = 100;
    public int Health { get; set; }

    private AIBehaviourSelector AISelector { get; set; }
    private BlackBoard BlackBoard { get; set; }
    void Start()
    {
        Health = MaxHealth;
        OnInitialize();
    }

    private void OnInitialize()
    {
        AISelector = GetComponent<AIBehaviourSelector>();
        BlackBoard = GetComponent<BlackBoard>();
        BlackBoard.OnInitialize();
        AISelector.OnInitialize(BlackBoard);
    }

    void Update()
    {
        AISelector.OnUpdate();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }
        var distance = BlackBoard.GetFloatVariableValue(VariableType.Distance);
        distance.Value = transform.position.magnitude;
    }
    
    public void TakeDamage(int _damage)
    {
        FloatValue res = BlackBoard.GetFloatVariableValue(VariableType.Health);
        if (res)
        {
            res.Value -= _damage;
        }
        AISelector.EvaluateBehaviours();
    }

    public void Die()
    {
        //die
    }
}


public interface IDamageable
{
    int MaxHealth { get; }
    int Health { get; set; }
    void TakeDamage(int _damage);
}