using System;
using System.Collections.Generic;

public static class EventBus
{
    private static Dictionary<Type, Delegate> eventDictionary = new Dictionary<Type, Delegate>();

    /// <summary> 監聽事件 </summary>
    public static void Listen<T>(Action<T> listener) {
        if (!eventDictionary.ContainsKey(typeof(T)))
        {
            eventDictionary[typeof(T)] = null;
        }
        eventDictionary[typeof(T)] = (Action<T>)eventDictionary[typeof(T)] + listener;
    }

    /// <summary> 停止監聽事件 </summary>
    public static void StopListen<T>(Action<T> listener) {
        if (eventDictionary.ContainsKey(typeof(T)))
        {
            eventDictionary[typeof(T)] = (Action<T>)eventDictionary[typeof(T)] - listener;
        }
    }

    /// <summary> 觸發（發佈）事件，通知所有監聽者 </summary>
    public static void Trigger<T>(T eventData) {
        if (eventDictionary.ContainsKey(typeof(T)) && eventDictionary[typeof(T)] != null)
        {
            ((Action<T>)eventDictionary[typeof(T)]).Invoke(eventData);
        }
    }
}
