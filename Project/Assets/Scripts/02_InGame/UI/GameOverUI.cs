using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtWinner;
    [SerializeField] private Image imgWinnerBackground;
    [SerializeField] private Button btnReturnRoom;

    [SerializeField] private GameObject playerWinObject;
    [SerializeField] private GameObject murdererWinObject;

    private void Awake()
    {
        btnReturnRoom.onClick.AddListener(() =>
        {
            EventManager.SendEvent("OpenScene", "Main");
        });
    }

    public void SetGameOver(bool murderWin)
    {
        playerWinObject.SetActive(!murderWin);
        murdererWinObject.SetActive(murderWin);
        if (murderWin)
        {
            txtWinner.text = Strings.GetString(StringKey.InGameMurderWin);
            imgWinnerBackground.color = new Color(.8f, .0f, .2f);
        }
        else
        {
            txtWinner.text = Strings.GetString(StringKey.InGamePlayerWin);
            imgWinnerBackground.color = new Color(.2f, .6f, .8f);
        }
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            var roomInfo = PhotonNetwork.CurrentRoom.CustomProperties;
            roomInfo["isStart"] = false;
            MissionManager.Clear();
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomInfo);
        }
    }
}
