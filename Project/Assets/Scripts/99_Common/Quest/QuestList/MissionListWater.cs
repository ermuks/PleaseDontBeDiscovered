using System.Collections.Generic;

public class MissionListWater : Mission
{
    public override Mission InitializeMission()
    {
        missionType = MissionType.Personal;
        missionCode = "personalMission-Water";
        missionName = "물통 채우기";
        missionDescript = "물병을 3개 채우세요";
        isFirst = true;
        missionConditions = 3;
        return this;
    }
}
