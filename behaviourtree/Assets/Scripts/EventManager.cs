using System;
using System.Collections.Generic;

public enum EventType
{
    GuardText,
    NinjaText,
    OnPlayerDied,
    OnPlayerAttacked,
}

public static class EventManager
{
    private static readonly Dictionary<EventType, Delegate> pmEventDictionary = new Dictionary<EventType, Delegate>();
    private static readonly Dictionary<EventType, Action> eventDictionary = new Dictionary<EventType, Action>();

    public static void AddListener<T>(EventType _type, Action<T> _action)
    {
        if (!pmEventDictionary.ContainsKey(_type))
        {
            pmEventDictionary.Add(_type, null);
        }

        pmEventDictionary[_type] = (Action<T>)pmEventDictionary[_type] + _action;
    }

    public static void AddListener(EventType _type, Action _action)
    {
        if (!eventDictionary.ContainsKey(_type))
        {
            eventDictionary.Add(_type, null);
        }

        eventDictionary[_type] += _action;
    }

    public static void InvokeEvent<T>(EventType _type, T _parameter)
    {
        if (pmEventDictionary.ContainsKey(_type) && pmEventDictionary[_type] != null)
        {
            ((Action<T>)pmEventDictionary[_type])?.Invoke(_parameter);
        }
    }

    public static void InvokeEvent(EventType _type)
    {
        eventDictionary[_type]?.Invoke();
    }

    public static void RemoveListener<T>(EventType _type, Action<T> _action)
    {
        if (!pmEventDictionary.TryGetValue(_type, out Delegate currentEvent)) return;
        if (currentEvent != null)
        {
            pmEventDictionary[_type] = (Action<T>)currentEvent - _action;
        }
    }

    public static void RemoveListener(EventType _type, Action _action)
    {
        if (eventDictionary.ContainsKey(_type) && eventDictionary[_type] != null)
        {
            eventDictionary[_type] -= _action;
        }
    }

    public static void RemoveAllListeners()
    {
        pmEventDictionary.Clear();
        eventDictionary.Clear();
    }
}