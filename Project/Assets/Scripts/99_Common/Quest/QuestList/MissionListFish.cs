using System.Collections.Generic;

public class MissionListFish : Mission
{
    public override Mission InitializeMission()
    {
        missionType = MissionType.Personal;
        missionCode = "personalMission-Fish";
        missionName = "물고기 잡기";
        missionDescript = "물고기를 3마리를 낚시하세요.";
        isFirst = true;
        missionConditions = 3;
        return this;
    }
}
