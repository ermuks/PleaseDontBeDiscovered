using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionUIManager : MonoBehaviour
{
    [SerializeField] private Transform missionListParent;
    private GameObject missionListPrefab;

    private List<MissionListItem> missionListItems = new List<MissionListItem>();

    private void Awake()
    {
        missionListPrefab = Resources.Load<GameObject>("Prefabs/UI/MissionListItem");
        EventManager.AddEvent("Mission :: Refresh", (p) => Refresh());
        Init();
    }

    private void Init()
    {
        var missionList = MissionManager.GetMissions();
        for (int i = 0; i < missionList.Count; i++)
        {
            MissionListItem item = Instantiate(missionListPrefab, missionListParent).GetComponent<MissionListItem>();
            item.Init(missionList[i]);
            missionListItems.Add(item);
        }
    }

    private void Refresh()
    {
        var missionList = MissionManager.GetMissions();
        for (int i = 0; i < missionListItems.Count; i++)
        {
            missionListItems[i].Init(missionList[i]);
        }
    }
}
