using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public enum WorkMessage
{
    None = 0,
    Treezone,
    WaterZone,
    FishZone,
}

public class TriggerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtKey;
    [SerializeField] private TMP_Text txtMessage;

    private WorkMessage currentWork;

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
            default:
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(Settings.instance.GetKey(KeySettings.Work)))
        {
            bool isDead = (bool)Photon.Pun.PhotonNetwork.LocalPlayer.CustomProperties["isDead"];

            if (!isDead)
            {
                EventManager.SendEvent("Player :: WorkStart", currentWork);
                EventManager.SendEvent("InGameUI :: WorkStart", currentWork);
            }
        }
        if (Input.GetKeyDown(Settings.instance.GetKey(KeySettings.CancelWork)))
        {
            EventManager.SendEvent("Player :: WorkEnd", currentWork);
            EventManager.SendEvent("InGameUI :: WorkEnd");
        }
    }
}
