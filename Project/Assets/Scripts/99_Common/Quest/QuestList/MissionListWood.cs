using System.Collections.Generic;

public class MissionListWood : Mission
{
    public override Mission InitializeMission()
    {
        missionType = MissionType.Personal;
        missionCode = "personalMission-Wood";
        missionName = "나무 구하기";
        missionDescript = "나무에 다가가서 나무를 3개 캐세요";
        isFirst = true;
        missionConditions = 3;
        return this;
    }

    public override void RefreshMission()
    {
        if (--missionConditions == 0)
        {
            CompleteMission();
        }
    }

    public override void CompleteMission()
    {
        MissionManager.SwitchMission(this, "Quest-Fire");
    }
}
