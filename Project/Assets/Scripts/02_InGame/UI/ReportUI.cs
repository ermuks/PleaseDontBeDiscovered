using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Photon.Pun;

public class ReportUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtKey;
    [SerializeField] private TMP_Text txtMessage;

    private bool isWorking = false;

    private void Awake()
    {
        EventManager.AddEvent("Report :: HideReport", (p) =>
        {
            EventManager.SendEvent("InGameUI :: ReportExit");
        });
    }

    public void SetMessage()
    {
        txtKey.text = ((char)Settings.instance.GetKey(KeySettings.Report)).ToString().ToUpper();
        txtMessage.text = Strings.GetString(StringKey.InGameWorkOpenVote);
    }

    private void Update()
    {
        if (!(bool)EventManager.GetData("InGameUI >> VoteUIActive"))
        {
            if (Input.GetKeyDown(Settings.instance.GetKey(KeySettings.Report)) && !isWorking)
            {
                bool isDead = (bool)Photon.Pun.PhotonNetwork.LocalPlayer.CustomProperties["isDead"];

                if (!isDead)
                {
                    var properties = PhotonNetwork.CurrentRoom.CustomProperties;
                    properties["Vote"] = true;
                    PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
                }
            }
        }
        else
        {
            EventManager.SendEvent("InGameUI :: ReportExit");
        }
    }
}
