using System.Collections.Generic;
using UnityEngine;

public enum MissionType
{
    None = 0,
    Common, Personal, Job
}

public class MissionManager
{
    private static string[] commonMissionCodes =
    {
        "commonMission-TurnOnLight",
        "commonMission-Camera"
    };
    private static string[] personalMissionCodes =
    {
        "personalMission-Fish",
        "personalMission-Wood",
        "personalMission-Fire",
        "personalMission-Water",
        "personalMission-Hanging"
    };
    private static string[] jobMissionCodes =
    {
        "jobMission-Fish"
    };
    private static Dictionary<string, Mission> commonMissionList = new Dictionary<string, Mission>();
    private static Dictionary<string, Mission> personalMissionList = new Dictionary<string, Mission>();
    private static Dictionary<string, Mission> jobMissionList = new Dictionary<string, Mission>();
    private static Dictionary<string, Mission> myMissions = new Dictionary<string, Mission>();

    public static List<Mission> GetMissions() => new List<Mission>(myMissions.Values);

    public static string GetMissionToString()
    {
        string result = "";
        foreach (var mission in myMissions.Values)
        {
            result += $"{mission.missionCode} :: {mission.missionName}\n";
        }
        return result;
    }

    public static void Initialize()
    {
        Queue<string> codes = new Queue<string>();

        // Common mission setting
        for (int i = 0; i < commonMissionCodes.Length; i++)
        {
            codes.Enqueue(commonMissionCodes[i]);
        }
        commonMissionList.Add(codes.Dequeue(), new MissionListTurnOnLight());
        commonMissionList.Add(codes.Dequeue(), new MissionListTurnOnLight());
        //////////////////////////////

        // Personal mission setting
        for (int i = 0; i < personalMissionCodes.Length; i++)
        {
            codes.Enqueue(personalMissionCodes[i]);
        }
        personalMissionList.Add(codes.Dequeue(), new MissionListFish());
        personalMissionList.Add(codes.Dequeue(), new MissionListWood());
        personalMissionList.Add(codes.Dequeue(), new MissionListFire());
        personalMissionList.Add(codes.Dequeue(), new MissionListWater());
        //////////////////////////////
    }

    public static void ProcessMission(string missionCode)
    {
        if (myMissions.ContainsKey(missionCode))
        {
            myMissions[missionCode].RefreshMission();
            EventManager.SendEvent("Mission :: Refresh", myMissions[missionCode]);
        }
    }

    public static void Clear()
    {
        myMissions.Clear();
    }

    public static void SetMission(int common, int personal, int job)
    {
        int addCount;
        Debug.Log("Mission initializing... ");
        Debug.Log($"common : {common}, personal : {personal}, job : {job}");
        addCount = 0;
        while (addCount < common)
        {
            string code = commonMissionCodes[Random.Range(0, commonMissionCodes.Length)];
            Debug.Log($"Try to add Mission. Code : {code}");
            if (myMissions.ContainsKey(code)) continue;
            Mission mission = commonMissionList[code].InitializeMission();
            if (!mission.isFirst) continue;
            myMissions.Add(code, mission);
            addCount++;
        }

        addCount = 0;
        while (addCount < personal)
        {
            string code = personalMissionCodes[Random.Range(0, personalMissionCodes.Length)];
            if (myMissions.ContainsKey(code)) continue;
            Mission mission = personalMissionList[code].InitializeMission();
            if (!mission.isFirst) continue;
            myMissions.Add(code, mission);
            addCount++;
        }

        addCount = 0;
        while (addCount < job)
        {
            string code = jobMissionCodes[Random.Range(0, jobMissionCodes.Length)];
            if (myMissions.ContainsKey(code)) continue;
            Mission mission = jobMissionList[code].InitializeMission();
            if (!mission.isFirst) continue;
            myMissions.Add(code, mission);
            addCount++;
        }
    }

    public static void SwitchMission(Mission origin, string code)
    {
        myMissions.Remove(origin.missionCode);

        Mission mission;
        string type = code.Split('-')[0];
        switch (type)
        {
            case "common":
                mission = commonMissionList[code];
                break;
            case "personal":
                mission = personalMissionList[code];
                break;
            case "job":
                mission = jobMissionList[code];
                break;
            default:
                mission = null;
                break;
        }
        mission.InitializeMission();
        myMissions.Add(code, mission);
    }

    public static void CompleteMission(string code)
    {
        Mission mission;
        string type = code.Split('-')[0];
        switch (type)
        {
            case "common":
                mission = commonMissionList[code];
                break;
            case "personal":
                mission = personalMissionList[code];
                break;
            case "job":
                mission = jobMissionList[code];
                break;
            default:
                mission = null;
                break;
        }
        mission.CompleteMission();
    }
}
