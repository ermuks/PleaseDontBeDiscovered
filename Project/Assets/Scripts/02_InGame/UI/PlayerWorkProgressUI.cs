using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerWorkProgressUI : MonoBehaviour
{
    [SerializeField] private Image imgProgressMask;
    [SerializeField] private TMP_Text txtProcessWork;
    [SerializeField] private TMP_Text txtProgressValue;

    private Coroutine timer;

    private float workProcessTimer;
    private float workEndTime;
    private WorkMessage currentWork;
    private Collider currentCollider;

    private void Awake()
    {
        EventManager.AddEvent("Player :: StopWorking", (p) =>
        {
            if (timer != null)
            {
                StopCoroutine(timer);
            }
        });
    }

    public void SetWork(WorkMessage msg, Collider col)
    {
        string message = "";
        currentWork = msg;
        currentCollider = col;
        workProcessTimer = 0;
        switch (msg)
        {
            case WorkMessage.None:
                
                break;
            case WorkMessage.Treezone:
                message = Strings.GetString(StringKey.InGameWorkProcessTree);
                workEndTime = 8f;
                break;
            case WorkMessage.WaterZone:
                message = Strings.GetString(StringKey.InGameWorkProcessWater);
                workEndTime = 1.5f;
                break;
            case WorkMessage.FishZone:
                message = Strings.GetString(StringKey.InGameWorkProcessFish);
                workEndTime = 3f;
                break;
            case WorkMessage.LightZone:
                message = Strings.GetString(StringKey.InGameWorkProcessLight);
                workEndTime = 6f;
                break;
            case WorkMessage.CampFire:
                message = Strings.GetString(StringKey.InGameWorkProcessCampFire);
                workEndTime = 4f;
                break;
            case WorkMessage.TableZone:
                message = Strings.GetString(StringKey.InGameWorkProcessTable);
                workEndTime = 6f;
                break;
            case WorkMessage.ChestZone:
                message = Strings.GetString(StringKey.InGameWorkProcessChest);
                workEndTime = 7f;
                break;
            default:
                break;
        }
        txtProcessWork.text = message;
        txtProgressValue.text = "0%";
    }

    private void Update()
    {
        workProcessTimer += Time.deltaTime;
        imgProgressMask.fillAmount = workProcessTimer / workEndTime;
        txtProgressValue.text = $"{workProcessTimer / workEndTime * 100f:#,##0}%";

        if (workProcessTimer >= workEndTime)
        {
            gameObject.SetActive(false);
            EventManager.SendEvent("Player :: WorkSuccess", currentWork, currentCollider);
            EventManager.SendEvent("Trigger :: EndWork", currentWork);
        }
    }
}
