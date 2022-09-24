using System.Collections.Generic;

public class MissionListHanging : Mission
{
    public override Mission InitializeMission()
    {
        missionType = MissionType.Personal;
        missionCode = "personalMission-Wood";
        missionName = "옷 정리하기";
        missionDescript = "옷장에 널브러진 옷을 정리하세요.";
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

    public override void CompleteMission()
    {
        
    }
}
