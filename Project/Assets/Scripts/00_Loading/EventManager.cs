using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void OnEvent(params object[] param);

    private static Dictionary<string, OnEvent> eventList = new Dictionary<string, OnEvent>();

    public static void AddEvent(string key, OnEvent func)
    {
        if (eventList.ContainsKey(key))
        {
            eventList[key] = func;
        }
        else
        {
            eventList.Add(key, func);
        }
    }

    public static void SendEvent(string key, params object[] param)
    {
        if (key == "") return;
        if (eventList.ContainsKey(key))
        {
            eventList[key](param);
        }
    }
}