using System.Collections.Generic;

public class MissionListTable : Mission
{
    public override Mission InitializeMission()
    {
        missionType = MissionType.Personal;
        missionCode = "personalMission-Table";
        missionName = "테이블 치우기";
        missionDescript = "테이블을 정리하세요";
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
