using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
#if UNITY_EDITOR
    public static string[] ignoreKeys = new string[]
    {
        "Drag",
        "Refresh",
        "SetKillCooldown",
        "Sound"
    };
#endif

    public delegate void OnEvent(params object[] param);
    public delegate object GetDataFunction(params object[] param);

    private static Dictionary<string, OnEvent> eventList = new Dictionary<string, OnEvent>();
    private static Dictionary<string, GetDataFunction> dataList = new Dictionary<string, GetDataFunction>();

    public static Dictionary<string, OnEvent> GetEventList() => eventList;

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
        if (key != "")
        {
            if (eventList.ContainsKey(key))
            {
#if UNITY_EDITOR
                if (Array.FindIndex(ignoreKeys, e => key.IndexOf(e) != -1) == -1)
                {
                    string p = "";
                    if (param != null)
                    {
                        for (int i = 0; i < param.Length; i++)
                        {
                            p += $"\nParameter [ <color=#79d5d9>{i}</color> ] : <color=#c9f5f9>{param[i]}</color>";
                        }
                    }
                    Debug.Log($"<color=#fff335>[ Success ]</color>\nEvent Key : <color=#c9f5f9>{key}</color>{p}");
                }
#endif
                eventList[key](param);
            }
            else
            {
                if (Array.FindIndex(ignoreKeys, e => key.IndexOf(e) != -1) == -1)
                {
                    Debug.Log($"<color=#ff7b85>[ Fail ]</color>\nEvent Key : <color=#c9f5f9>{key}</color>");
                }
            }
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