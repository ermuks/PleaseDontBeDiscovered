using System.Collections.Generic;

public class MissionListPicture : Mission
{
    public override Mission InitializeMission()
    {
        missionType = MissionType.Personal;
        missionCode = "personalMission-Picture";
        missionName = "동굴 사진찍기";
        missionDescript = "동굴에서 사진을 찍으세요";
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
