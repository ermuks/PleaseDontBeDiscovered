using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Photon.Pun;

public enum WorkMessage
{
    None = 0,
    Treezone,
    WaterZone,
    FishZone,
    OpenVote,
}

public class TriggerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtKey;
    [SerializeField] private TMP_Text txtMessage;

    private WorkMessage currentWork;

    private bool isWorking = false;

    private void Awake()
    {
        EventManager.AddData("Player :: Working", (p) => isWorking);
        EventManager.AddEvent("Trigger :: EndWork", (p) =>
        {
            WorkMessage msg = (WorkMessage)p[0];
            isWorking = false;
        });
    }

    public void SetMessage(WorkMessage msg)
    {
        currentWork = msg;
        txtKey.text = ((char)Settings.instance.GetKey(KeySettings.Work)).ToString();
        switch (msg)
        {
            case WorkMessage.None:
                txtMessage.text = Strings.GetString(StringKey.InGameWorkNoMessage);
                break;
            case WorkMessage.Treezone:
                txtMessage.text = Strings.GetString(StringKey.InGameWorkTree);
                break;
            case WorkMessage.WaterZone:
                txtMessage.text = Strings.GetString(StringKey.InGameWorkWater);
                break;
            case WorkMessage.FishZone:
                txtMessage.text = Strings.GetString(StringKey.InGameWorkFish);
                break;
            case WorkMessage.OpenVote:
                txtMessage.text = Strings.GetString(StringKey.InGameWorkOpenVote);
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (!(bool)EventManager.GetData("InGameUI >> VoteUIActive"))
        {
            if (Input.GetKeyDown(Settings.instance.GetKey(KeySettings.Work)) && !isWorking)
            {
                bool isDead = (bool)Photon.Pun.PhotonNetwork.LocalPlayer.CustomProperties["isDead"];

                if (!isDead)
                {
                    bool isReport = false;
                    string err = "";
                    switch (currentWork)
                    {
                        case WorkMessage.None:
                            break;
                        case WorkMessage.Treezone:
                            if ((bool)EventManager.GetData("Inventory >> ExistBlankCell", "0007", 2)) isWorking = true;
                            else err = Strings.GetString(StringKey.InGameMessageInventoryIsFull);
                            break;
                        case WorkMessage.WaterZone:
                            if ((bool)EventManager.GetData("Inventory >> ExistBlankCell", "0003", 1))
                            {
                                if ((bool)EventManager.GetData("Inventory >> ExistItem", "0002"))
                                {
                                    isWorking = true;
                                }
                                else
                                {
                                    err = Strings.GetString(StringKey.InGameMessageNotExistItem, ItemManager.GetItem("0002").itemName);
                                }
                            }
                            else
                            {
                                err = Strings.GetString(StringKey.InGameMessageInventoryIsFull);
                            }
                            break;
                        case WorkMessage.FishZone:
                            if ((bool)EventManager.GetData("Inventory >> ExistBlankCell", "0000", 1)) isWorking = true;
                            else err = Strings.GetString(StringKey.InGameMessageInventoryIsFull);
                            break;
                        case WorkMessage.OpenVote:
                            isReport = true;
                            break;
                        default:
                            break;
                    }
                    if (isReport)
                    {
                        var properties = PhotonNetwork.CurrentRoom.CustomProperties;
                        properties["Vote"] = true;
                        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
                    }
                    else
                    {
                        if (isWorking)
                        {
                            EventManager.SendEvent("Player :: WorkStart", currentWork);
                            EventManager.SendEvent("InGameUI :: WorkStart", currentWork);
                        }
                        else
                        {
                            EventManager.SendEvent("InGameUI :: CreateMessage", err);
                        }
                    }
                }
            }
            if (Input.GetKeyDown(Settings.instance.GetKey(KeySettings.CancelWork)) && isWorking)
            {
                isWorking = false;
                EventManager.SendEvent("Player :: WorkEnd");
                EventManager.SendEvent("InGameUI :: WorkEnd");
            }
        }
    }
}
