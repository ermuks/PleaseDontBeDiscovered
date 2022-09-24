using System.Collections.Generic;

public class MissionListCamera : Mission
{
    public override Mission InitializeMission()
    {
        missionType = MissionType.Common;
        missionCode = "commonMission-Camera";
        missionName = "촬영하기";
        missionDescript = "모닥불에 불을 지펴 불을 피우세요.";
        isFirst = true;
        missionConditions = 1;
        return this;
    }

    public override void RefreshMission()
    {
        if (--missionRemainConditions == 0)
        {
            CompleteMission();
        }
    }
}
