using System.Collections.Generic;
using UnityEngine;

public class Blackboard
{
    private readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();

    public T GetVariable<T>(string _name)
    {
        if (dictionary.ContainsKey(_name))
        {
            return (T)dictionary[_name];
        }
        return default(T);
    }

    public void SetVariable<T>(string _name, T _variable)
    {
        if (dictionary.ContainsKey(_name))
        {
            dictionary[_name] = _variable;
        }
        else
        {
            dictionary.Add(_name, _variable);
        }
    }
}
