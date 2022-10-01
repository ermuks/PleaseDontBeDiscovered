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
    CampFire,
    LightZone,
    TableZone,
    ChestZone,
    PictureZone,
}

public class TriggerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtKey;
    [SerializeField] private TMP_Text txtMessage;

    private WorkMessage currentWork;
    private Collider currentCollider;

    private bool isWorking = false;

    private void Awake()
    {
        EventManager.AddData("Player :: Working", (p) => isWorking);
        EventManager.AddEvent("Trigger :: EndWork", (p) =>
        {
            WorkMessage msg = (WorkMessage)p[0];
            isWorking = false;
        });
        EventManager.AddEvent("Trigger :: HideTrigger", (p) =>
        {
            EventManager.SendEvent("InGameUI :: TriggerExit", null);
        });
    }

    public void SetMessage(WorkMessage msg, Collider col)
    {
        currentWork = msg;
        currentCollider = col;
        txtKey.text = ((char)Settings.instance.GetKey(KeySettings.Work)).ToString().ToUpper();
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
            case WorkMessage.LightZone:
                txtMessage.text = Strings.GetString(StringKey.InGameWorkLight);
                break;
            case WorkMessage.TableZone:
                txtMessage.text = Strings.GetString(StringKey.InGameWorkTable);
                break;
            case WorkMessage.ChestZone:
                txtMessage.text = Strings.GetString(StringKey.InGameWorkChest);
                break;
            case WorkMessage.PictureZone:
                txtMessage.text = Strings.GetString(StringKey.InGameWorkPicture);
                break;
            case WorkMessage.CampFire:
                txtMessage.text = Strings.GetString(StringKey.InGameWorkCampFire);
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
                    string err = "";
                    switch (currentWork)
                    {
                        case WorkMessage.None:
                            break;
                        case WorkMessage.Treezone:
                            isWorking = true;
                            break;
                        case WorkMessage.WaterZone:
                            isWorking = true;
                            break;
                        case WorkMessage.FishZone:
                            isWorking = true;
                            break;
                        case WorkMessage.LightZone:
                            isWorking = true;
                            break;
                        case WorkMessage.CampFire:
                            isWorking = true;
                            break;
                        case WorkMessage.TableZone:
                            isWorking = true;
                            break;
                        case WorkMessage.ChestZone:
                            isWorking = true;
                            break;
                        case WorkMessage.PictureZone:
                            isWorking = true;
                            break;
                        default:
                            break;
                    }
                    if (isWorking)
                    {
                        EventManager.SendEvent("Player :: WorkStart", currentWork);
                        EventManager.SendEvent("InGameUI :: WorkStart", currentWork, currentCollider);
                    }
                    else
                    {
                        EventManager.SendEvent("InGameUI :: CreateMessage", err);
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
        else
        {
            EventManager.SendEvent("InGameUI :: TriggerExit", null);
        }
    }
}
