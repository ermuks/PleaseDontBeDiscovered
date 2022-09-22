using System.Collections.Generic;

public class MissionListTurnOnLight : Mission
{
    public override Mission InitializeMission()
    {
        missionType = MissionType.Personal;
        missionCode = "commonMission-TurnOnLight";
        missionName = "모닥불에 불 지피기";
        missionDescript = "모닥불에 불을 지펴 불을 피우세요.";
        isFirst = false;
        missionConditions = 1;
        return this;
    }
}
