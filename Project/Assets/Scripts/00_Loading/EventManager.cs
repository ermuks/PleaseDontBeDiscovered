using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void OnEvent(params object[] param);
    public delegate object GetDataFunction(params object[] param);

    private static Dictionary<string, OnEvent> eventList = new Dictionary<string, OnEvent>();
    private static Dictionary<string, GetDataFunction> dataList = new Dictionary<string, GetDataFunction>();

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

    public static void AddData(string key, GetDataFunction func)
    {
        if (dataList.ContainsKey(key))
        {
            dataList[key] = func;
        }
        else
        {
            dataList.Add(key, func);
        }
    }

    public static object GetData(string key, params object[] param)
    {
        if (key == "") return null;
        if (dataList.ContainsKey(key))
        {
            return dataList[key](param);
        }
        else
        {
            return null;
        }
    }
}