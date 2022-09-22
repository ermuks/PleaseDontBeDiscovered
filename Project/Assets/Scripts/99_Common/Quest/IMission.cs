using System.Collections.Generic;

public interface IMission
{
    public Mission InitializeMission();
    public void RefreshMission();
    public void CompleteMission();
    public void RemoveMission();
}
