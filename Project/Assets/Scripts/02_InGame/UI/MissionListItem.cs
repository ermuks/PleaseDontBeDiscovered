using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionListItem : MonoBehaviour
{
    private Mission mission;
    [SerializeField] private TMP_Text txtMissionName;
    [SerializeField] private TMP_Text txtMissionConditions;

    public void Init(Mission mission)
    {
        this.mission = mission;
        Refresh();
    }

    public void Refresh()
    {
        txtMissionName.text = mission.missionName;
        if (mission.missionConditions > 0)
        {
            txtMissionConditions.text = Strings.GetString(StringKey.InGameMissionRemain, mission.missionConditions);
        }
    }
}
