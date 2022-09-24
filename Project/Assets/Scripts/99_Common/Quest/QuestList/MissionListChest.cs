using System.Collections.Generic;

public class MissionListChest : Mission
{
    public override Mission InitializeMission()
    {
        missionType = MissionType.Personal;
        missionCode = "personalMission-Table";
        missionName = "창고 정리하기";
        missionDescript = "창고 물품을 종류에 맞게 정리하세요.";
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
