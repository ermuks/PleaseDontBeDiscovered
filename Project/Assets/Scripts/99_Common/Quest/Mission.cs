using System.Collections;
using System.Collections.Generic;

public class Mission : IMission
{ 
    public MissionType missionType;
    public bool isFirst;
    public string missionCode;
    public string missionName;
    public string missionDescript;
    public int missionConditions 
    {
        set
        {
            missionRemainConditions = value;
            missionMaxConditions = value;
        }
    }
    public int missionRemainConditions;
    public int missionMaxConditions;

    public virtual Mission InitializeMission()
    {
        return this;
    }

    public virtual void RefreshMission()
    {
    }

    public virtual void RemoveMission()
    {
    }

    public virtual void CompleteMission()
    {
    }
}
