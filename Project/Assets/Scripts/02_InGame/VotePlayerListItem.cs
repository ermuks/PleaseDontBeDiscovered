using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class VotePlayerListItem : MonoBehaviour
{
    public Player player;
    [SerializeField] private GameObject areaVoteButtons;

    [SerializeField] private Image imgProfile;
    [SerializeField] private GameObject imgDie;
    [SerializeField] private Text txtNickname;

    [SerializeField] private Button btnSelect;
    [SerializeField] private Button btnCancel;

    private bool voteable = false;

    public void Init(Player player)
    {
        this.player = player;

        GetComponent<Button>().onClick.AddListener(() => OnClick());
        btnSelect.onClick.AddListener(() => ButtonSelect());
        btnCancel.onClick.AddListener(() => ButtonCancel());

        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["isMurder"])
        {
            if ((bool)player.CustomProperties["isMurder"])
            {
                txtNickname.color = Color.red;
            }
        }

        bool isAlreadyVoted = (bool)PhotonNetwork.LocalPlayer.CustomProperties["alreadyVoted"];
        if (isAlreadyVoted) SetVote();

        txtNickname.text = player.NickName;
        if (imgProfile != null)
        {
            Debug.Log(player.NickName);
            Debug.Log(player.CustomProperties);
            imgProfile.color = PlayerData.GetColor((int)player.CustomProperties["color"]);
        }
    }

    private void OnClick()
    {
        bool isDead = (bool)PhotonNetwork.LocalPlayer.CustomProperties["isDead"];
        bool isAlreadyVoted = (bool)PhotonNetwork.LocalPlayer.CustomProperties["alreadyVoted"];
        if (voteable && !isDead && !isAlreadyVoted)
        {
            areaVoteButtons.SetActive(!areaVoteButtons.activeInHierarchy);
        }
    }

    private void ButtonSelect()
    {
        bool isAlreadyVoted = (bool)PhotonNetwork.LocalPlayer.CustomProperties["alreadyVoted"];
        if (!isAlreadyVoted)
        {
            EventManager.SendEvent("InGameUI :: CompleteVote", PhotonNetwork.LocalPlayer, player);
            EventManager.SendEvent("InGameUI :: CloseVoteButtons");
        }
        EventManager.SendEvent("InGameData :: AlreadyVoted", true);
        CloseVoteButton();
    }

    private void ButtonCancel()
    {
        CloseVoteButton();
    }


    public void CloseVoteButton()
    {
        areaVoteButtons.SetActive(false);
    }

    public void SetDie(bool isDead)
    {
        voteable = !isDead;
        if (isDead) GetComponent<Image>().color = Color.black;
        else GetComponent<Image>().color = Color.white;
        imgDie.SetActive(isDead);
    }

    public void SetVote()
    {
        GetComponent<Image>().color = new Color(.2f, .6f, .8f);
    }
}
