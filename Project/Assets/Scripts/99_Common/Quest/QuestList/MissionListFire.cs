﻿using System.Collections.Generic;

public class MissionListFire : Mission
{
    public override Mission InitializeMission()
    {
        missionType = MissionType.Personal;
        missionCode = "personalMission-Fire";
        missionName = "모닥불에 불 지피기";
        missionDescript = "모닥불에 불을 지펴 불을 피우세요.";
        isFirst = false;
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