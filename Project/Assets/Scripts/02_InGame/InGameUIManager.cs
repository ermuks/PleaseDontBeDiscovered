using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InGameUIManager : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    private Transform canvas;
    private GameObject messageUIPrefab;
    private GameObject bloodUIPrefab;

    [SerializeField] private Image areaHPValue;
    [SerializeField] private Image areaHungryValue;
    [SerializeField] private Image areaThirstyValue;
    [SerializeField] private Image areaColdValue;
    [SerializeField] private Image areaWetValue;
    [SerializeField] private Image areaBreathValue;

    [SerializeField] private GameObject areaNormalPlayerUI;
    [SerializeField] private GameObject areaMurderPlayerUI;
    [SerializeField] private Image imgCooldownTimer;
    [SerializeField] private TMP_Text txtCooldownTimer;

    [SerializeField] private GameObject areaTrigger;
    [SerializeField] private GameObject areaWater;
    [SerializeField] private GameObject areaDieUI;
    [SerializeField] private GameObject areaInventory;
    [SerializeField] private GameObject areaPlayerUI;
    [SerializeField] private GameObject areaPlayerWorkProgressUI;
    [SerializeField] private GameObject areaWatcherUI;
    [SerializeField] private GameObject areaGameOver;

    [SerializeField] private GameObject areaVote;
    [SerializeField] private GameObject areaFinishVote;
    [SerializeField] private Transform playerListParent;
    private GameObject playerListPrefab;
    private List<VotePlayerListItem> playerList = new List<VotePlayerListItem>();
    private int voteCount = 0;
    private int voteAmount = 0;

    [SerializeField] private Animator areaFaded;
    [SerializeField] private Transform voteEndingCamPosition;
    [SerializeField] private Transform voteEndingCamAnimation;

    private void Awake()
    {
        messageUIPrefab = Resources.Load<GameObject>("Prefabs/UI/InGameMessageUI");
        bloodUIPrefab = Resources.Load<GameObject>("Prefabs/UI/BloodEffect");

        playerListPrefab = Resources.Load<GameObject>("Prefabs/UI/VotePlayerListItem");

        areaFinishVote.SetActive(false);
        areaDieUI.SetActive(false);
        areaPlayerUI.SetActive(true);
        areaInventory.SetActive(false);
        areaWatcherUI.SetActive(false);
        areaGameOver.SetActive(false);
        areaTrigger.SetActive(false);
        areaVote.SetActive(false);
        areaPlayerWorkProgressUI.SetActive(false);

        bool isMurder = (bool)PhotonNetwork.LocalPlayer.CustomProperties["isMurder"];
        areaNormalPlayerUI.SetActive(!isMurder);
        areaMurderPlayerUI.SetActive(isMurder);

        areaHPValue.color = new Color(.8f, .8f, .8f);
        areaHungryValue.color = new Color(.8f, .8f, .8f);
        areaThirstyValue.color = new Color(.8f, .8f, .8f);
        areaColdValue.color = new Color(.8f, .8f, .8f);

        EventManager.AddEvent("PUN :: Hit", (p) =>
        {
            photonView.RPC("Hit", (Player)p[1], (Player)p[0]);
        });

        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            VotePlayerListItem votePlayer = Instantiate(playerListPrefab, playerListParent).GetComponent<VotePlayerListItem>();
            votePlayer.Init(player, null);
            playerList.Add(votePlayer);
        }

        EventManager.AddData("InGameUI >> VoteUIActive", (p) => areaVote.activeSelf);
        EventManager.AddData("InGameUI >> VoteEndingPosition", (p) => voteEndingCamPosition);
        EventManager.AddEvent("InGameUI :: OpenVoteUI", (p) =>
        {
            if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["isDead"])
            {
                EventManager.SendEvent("Player :: SetWatching");
                areaDieUI.SetActive(false);
            }

            var properties = PhotonNetwork.LocalPlayer.CustomProperties;
            properties["voteMembers"] = 0;
            PhotonNetwork.LocalPlayer.SetCustomProperties(properties);

            Cursor.lockState = CursorLockMode.None;
            areaVote.SetActive(true);

            voteCount = 0;
            EventManager.SendEvent("InGameData :: AlreadyVoted", false);
            EventManager.SendEvent("Player :: WorkEnd");
            EventManager.SendEvent("InGameUI :: Vote :: InitTimer", false);

            for (int i = 0; i < playerList.Count; i++)
            {
                bool isDead = (bool)playerList[i].player.CustomProperties["isDead"];
                playerList[i].SetDie(isDead);
            }
            
            RefreshVoteAmount();
            EventManager.SendEvent("InGameUI :: VoteChatInit");
        });
        EventManager.AddEvent("InGameUI :: FinishVoteUI", (p) =>
        {
            EventManager.SendEvent("InGameData :: FinishVoteAnimationPlaying", true);
            EventManager.SendEvent("InGameUI :: PlayFinishVoteAnimation");
        });
        EventManager.AddEvent("InGameUI :: CloseVoteUI", (p) =>
        {
            Cursor.lockState = CursorLockMode.Locked;
            areaVote.SetActive(false);
            EventManager.SendEvent("InGameUI :: FinishVoteUI");
        });
        EventManager.AddEvent("InGameUI :: CompleteVote", (p) =>
        {
            Player who = (Player)p[0];
            Player target = (Player)p[1];

            for (int i = 0; i < playerList.Count; i++)
            {
                playerList[i].CloseVoteButton();
            }

            photonView.RPC("CompleteVote", RpcTarget.All, who, target);
        });
        EventManager.AddEvent("InGameUI :: FadeIn", (p) =>
        {
            areaFaded.SetTrigger("Faded");
            EventManager.SendEvent((string)p[0], p);
        });
        EventManager.AddEvent("InGameUI :: PlayFinishVoteAnimation", (p) =>
        {
            StartCoroutine(AnimationEnd());
            areaPlayerUI.SetActive(false);
            areaDieUI.SetActive(false);
            areaTrigger.SetActive(false);
            EventManager.SendEvent("InGameData :: PlayerPositionSetting");
        });
        EventManager.AddEvent("InGameUI :: FinishVoteAnimationPlaying", (p) =>
        {
            if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["isDead"])
            {
                EventManager.SendEvent("Player :: PlayerDieToWatch");
                areaWatcherUI.SetActive(true);
            }
            else
            {
                EventManager.SendEvent("InGameData :: PlayerPositionSetting");
                areaPlayerUI.SetActive(true);
            }
        });

        EventManager.AddEvent("Refresh Stamina", (p) =>
        {
            float value = (float)p[0];
            areaHPValue.fillAmount = value;

            if (value >= .6f) areaHPValue.color = new Color(.8f, .8f, .8f);
            else if (value >= .3f) areaHPValue.color = Color.white;
            else if (value >= .15f) areaHPValue.color = new Color(1, 1, .4f);
            else areaHPValue.color = new Color(1, .4f, .4f);
        });
        EventManager.AddEvent("Refresh Hungry" , (p) =>
        {
            float value = (float)p[0];
            areaHungryValue.fillAmount = value;

            if (value >= .6f) areaHungryValue.color = new Color(.8f, .8f, .8f);
            else if (value >= .3f) areaHungryValue.color = Color.white;
            else if (value >= .15f) areaHungryValue.color = new Color(1, 1, .4f);
            else areaHungryValue.color = new Color(1, .4f, .4f);
        });
        EventManager.AddEvent("Refresh Thirsty", (p) =>
        {
            float value = (float)p[0];
            areaThirstyValue.fillAmount = value;

            if (value >= .6f) areaThirstyValue.color = new Color(.8f, .8f, .8f);
            else if (value >= .3f) areaThirstyValue.color = Color.white;
            else if (value >= .15f) areaThirstyValue.color = new Color(1, 1, .4f);
            else areaThirstyValue.color = new Color(1, .4f, .4f);
        });
        EventManager.AddEvent("Refresh Breath"   , (p) =>
        {
            float value = (float)p[0];
            areaBreathValue.fillAmount = value;

            if (value <= .5f) areaBreathValue.color = new Color(.8f, .8f, .8f);
            else if (value <= .7f) areaBreathValue.color = new Color(1, .8f, .8f);
            else if (value <= .9f) areaBreathValue.color = new Color(1, .4f, .4f);
            else areaBreathValue.color = new Color(.8f, .2f, .2f);
        });
        EventManager.AddEvent("Refresh Wet"   , (p) =>
        {
            float value = (float)p[0];
            areaWetValue.fillAmount = value;
        });
        EventManager.AddEvent("Refresh Cold", (p) =>
        {
            float value = (float)p[0];
            areaThirstyValue.fillAmount = value;

            if (value >= .6f) areaThirstyValue.color = new Color(.8f, .8f, .8f);
            else if (value >= .3f) areaThirstyValue.color = Color.white;
            else if (value >= .15f) areaThirstyValue.color = new Color(1, 1, .4f);
            else areaThirstyValue.color = new Color(1, .4f, .4f);
        });
        EventManager.AddEvent("InGameUI :: SetDie", (p) =>
        {
            areaDieUI.SetActive(true);
            areaDieUI.GetComponent<PlayerDieUI>().SetDie((Player)p[0]);
        });
        EventManager.AddEvent("InGameUI :: Die", (p) =>
        {
            areaDieUI.SetActive(true);
            if (p[0].GetType() == typeof(DieMessage))
            {
                areaDieUI.GetComponent<PlayerDieUI>().Die((DieMessage)p[0]);
            }
            else if (p[0].GetType() == typeof(Player))
            {
                areaDieUI.GetComponent<PlayerDieUI>().SetDie((Player)p[0]);
            }
        });
        EventManager.AddEvent("InGameUI :: InWater", (p) =>
        {
            areaWater.SetActive(true);
        });
        EventManager.AddEvent("InGameUI :: OutWater", (p) =>
        {
            areaWater.SetActive(false);
        });
        EventManager.AddEvent("InGameUI :: WatchUI", (p) =>
        {
            areaPlayerUI.SetActive(false);
            areaDieUI.SetActive(false);
            areaWatcherUI.SetActive(true);
        });
        EventManager.AddEvent("InGameUI :: WatchPlayer", (p) =>
        {
            Player player = ((Transform)p[0]).GetComponent<PhotonView>().Owner;
            areaWatcherUI.GetComponent<WatcherUI>().SetPlayer(player);
        });
        EventManager.AddEvent("InGameUI :: GameOver", (p) =>
        {
            bool murderWin = (bool)p[0];
            areaPlayerUI.SetActive(false);
            areaDieUI.SetActive(false);
            areaWatcherUI.SetActive(false);
            areaGameOver.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            areaGameOver.GetComponent<GameOverUI>().SetGameOver(murderWin);
        });
        EventManager.AddEvent("InGameUI :: TriggerEnter", (p) =>
        {
            Collider col = (Collider)p[0];
            if (col.CompareTag("TreeZone"))
            {
                areaTrigger.SetActive(true);
                areaTrigger.GetComponent<TriggerUI>().SetMessage(WorkMessage.Treezone);
            }
            else if (col.CompareTag("WaterZone") || col.CompareTag("DeepWater"))
            {
                areaTrigger.SetActive(true);
                areaTrigger.GetComponent<TriggerUI>().SetMessage(WorkMessage.WaterZone);
            }
            else if (col.CompareTag("FishZone"))
            {
                areaTrigger.SetActive(true);
                areaTrigger.GetComponent<TriggerUI>().SetMessage(WorkMessage.FishZone);
            }
            else if (col.CompareTag("WarmZone"))
            {
                EventManager.SendEvent("Player :: EnterWarmZone");
            }
            else if (col.CompareTag("ReportArea"))
            {
                areaTrigger.SetActive(true);
                areaTrigger.GetComponent<TriggerUI>().SetMessage(WorkMessage.OpenVote);
            }
            else
            {
                areaTrigger.SetActive(true);
                areaTrigger.GetComponent<TriggerUI>().SetMessage(WorkMessage.None);
            }
        });
        EventManager.AddEvent("InGameUI :: TriggerExit", (p) =>
        {
            areaTrigger.SetActive(false);
            if (p != null)
            {
                Collider col = (Collider)p[0];
                if (col.CompareTag("WarmZone"))
                {
                    EventManager.SendEvent("Player :: ExitWarmZone");
                }
            }
        });
        EventManager.AddEvent("InGameUI :: WorkStart", (p) =>
        {
            areaPlayerWorkProgressUI.SetActive(true);
            areaPlayerWorkProgressUI.GetComponent<PlayerWorkProgressUI>().SetWork((WorkMessage)p[0]);
        });
        EventManager.AddEvent("InGameUI :: WorkEnd", (p) =>
        {
            areaPlayerWorkProgressUI.SetActive(false);
        });
        EventManager.AddEvent("InGameUI :: CreateMessage", (p) =>
        {
            InGameMessageUI messageUI = Instantiate(messageUIPrefab, canvas).GetComponent<InGameMessageUI>();
            messageUI.SetMessage((string)p[0]);
        });
        EventManager.AddEvent("InGameUI :: Hurt", (p) =>
        {
            GameObject blood = Instantiate(bloodUIPrefab, canvas);
        });
        EventManager.AddEvent("InGameUI :: SetKillCooldown", (p) =>
        {
            imgCooldownTimer.fillAmount = 1 - (float)p[0] / (float)p[1];
            imgCooldownTimer.gameObject.SetActive(!(bool)p[2]);

            txtCooldownTimer.text = $"{(float)p[1] - (float)p[0]:0.0}";
            txtCooldownTimer.gameObject.SetActive(!(bool)p[2]);
        });
    }

    private void RefreshVoteAmount()
    {
        voteAmount = 0;
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (!(bool)player.CustomProperties["isDead"])
            {
                voteAmount++;
            }
        }
    }

    private IEnumerator AnimationEnd()
    {
        areaFinishVote.SetActive(true);
        areaFinishVote.GetComponent<Animator>().SetTrigger("Restart");
        voteEndingCamAnimation.GetComponent<Animator>().SetTrigger("SetAnim");
        Player manyVotePlayer = null;
        int voteCount = 0;
        bool isEqualVotes = false;

        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            var properties = player.CustomProperties;
            int count = (int)properties["voteMembers"];
            if (count > 0)
            {
                if (count > voteCount)
                {
                    voteCount = count;
                    manyVotePlayer = player;
                }
                else
                {
                    if (count == voteCount)
                    {
                        isEqualVotes = true;
                    }
                }
            }
        }
        if (isEqualVotes) manyVotePlayer = null;
        areaFinishVote.GetComponent<FinishVoteBackgroundUI>().SetMessage(manyVotePlayer);
        yield return new WaitForSeconds(15f);
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (manyVotePlayer != null)
            {
                photonView.RPC("VoteDie", manyVotePlayer);
            }
        }
    }
    [PunRPC]
    public void Test()
    {
        Debug.Log("Test");
    }

    [PunRPC]
    public void CompleteVote(Player who, Player target)
    {
        RefreshVoteAmount();
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            var properties = target.CustomProperties;
            properties["voteMembers"] = (int)properties["voteMembers"] + 1;
            target.SetCustomProperties(properties);

            if (++voteCount >= voteAmount)
            {
                var roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
                roomProperties["Vote"] = false;
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
            }
        }
        foreach (var playerListItem in playerList)
        {
            if (playerListItem.player == who)
            {
                playerListItem.SetVote();
                break;
            }
        }
        EventManager.SendEvent("InGameUI :: AddAlamMessage", Strings.GetString(StringKey.InGameMessageCompleteVote, who.NickName));
    }


    [PunRPC]
    private void VoteDie()
    {
        EventManager.SendEvent("Data :: VoteDie");
    }

    [PunRPC]
    private void Hit(Player player)
    {
        EventManager.SendEvent("Data :: Hit", player);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
