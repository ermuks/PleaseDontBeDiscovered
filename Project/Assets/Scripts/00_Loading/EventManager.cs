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
        if (key == "")
        {
#if UNITY_EDITOR
            if (key.IndexOf("Refresh") == -1 && key.IndexOf("SetKillCooldown") == -1)
            {
                Debug.Log($"<b><color=#ff2025>[ Fail ]</color></b>\nEvent Key : <color=#c9f5f9>{key}</color>\nParameters Count : <color=#c9f5f9>{param.Length}</color>");
            }
#endif
            return;
        }
        if (eventList.ContainsKey(key))
        {
            eventList[key](param);
#if UNITY_EDITOR
            if (key.IndexOf("Refresh") == -1 && key.IndexOf("SetKillCooldown") == -1)
            {
                string p = "";
                for (int i = 0; i < param.Length; i++)
                {
                    p += $"\nParameter [ <color=#79d5d9>{i}</color> ] : <color=#c9f5f9>{param.Length}</color>";
                }
                Debug.Log($"<color=#fff335>[ Success ]</color>\nEvent Key : <color=#c9f5f9>{key}</color>{p}");
            }
#endif
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