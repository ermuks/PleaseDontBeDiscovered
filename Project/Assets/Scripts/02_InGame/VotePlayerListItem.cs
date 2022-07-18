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

    public void Init(Player player, Texture2D img)
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            bool isDead = (bool)PhotonNetwork.LocalPlayer.CustomProperties["isDead"];
            if (voteable && !isDead && !(bool)EventManager.GetData("InGameData >> AlreadyVoted"))
            {
                areaVoteButtons.SetActive(!areaVoteButtons.activeInHierarchy);
            }
        });
        btnSelect.onClick.AddListener(() =>
        {
            EventManager.SendEvent("InGameUI :: CompleteVote", PhotonNetwork.LocalPlayer, player);
            EventManager.SendEvent("InGameData :: AlreadyVoted", true);
            CloseVoteButton();
        });
        btnCancel.onClick.AddListener(() =>
        {
            CloseVoteButton();
        });
        bool isMurder = (bool)PhotonNetwork.LocalPlayer.CustomProperties["isMurder"];
        this.player = player;
        if (img != null)
        {
            imgProfile.sprite = Sprite.Create(img, new Rect(Vector2.zero, new Vector2(img.width, img.height)), Vector2.zero);
        }
        if (isMurder)
        {
            if ((bool)player.CustomProperties["isMurder"])
            {
                txtNickname.color = Color.red;
            }
        }
        txtNickname.text = player.NickName;
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
