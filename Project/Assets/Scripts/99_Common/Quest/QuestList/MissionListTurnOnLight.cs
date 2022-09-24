using System.Collections.Generic;

public class MissionListTurnOnLight : Mission
{
    public override Mission InitializeMission()
    {
        missionType = MissionType.Common;
        missionCode = "commonMission-TurnOnLight";
        missionName = "가로등 불 켜기";
        missionDescript = "주변 시야가 어둡습니다. 가로등을 켜서 시야를 밝히세요.";
        isFirst = true;
        missionConditions = 3;
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
