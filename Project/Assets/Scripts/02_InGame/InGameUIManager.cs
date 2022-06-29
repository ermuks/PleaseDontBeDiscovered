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

    [SerializeField] private GameObject areaNormalPlayerUI;
    [SerializeField] private GameObject areaMurderPlayerUI;
    [SerializeField] private Image imgCooldownTimer;
    [SerializeField] private TMP_Text txtCooldownTimer;

    [SerializeField] private GameObject areaTrigger;
    [SerializeField] private GameObject areaWater;
    [SerializeField] private GameObject areaDieUI;
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

    private void Awake()
    {
        messageUIPrefab = Resources.Load<GameObject>("Prefabs/UI/InGameMessageUI");
        bloodUIPrefab = Resources.Load<GameObject>("Prefabs/UI/BloodEffect");

        playerListPrefab = Resources.Load<GameObject>("Prefabs/UI/VotePlayerListItem");

        areaFinishVote.SetActive(false);
        areaDieUI.SetActive(false);
        areaPlayerUI.SetActive(true);
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

        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            VotePlayerListItem votePlayer = Instantiate(playerListPrefab, playerListParent).GetComponent<VotePlayerListItem>();
            votePlayer.Init(player, null);
            playerList.Add(votePlayer);
        }
        EventManager.AddData("InGameUI >> VoteUIActive", (p) => areaVote.activeInHierarchy);
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
            EventManager.SendEvent("InGameUI :: FadeIn", "InGameUI :: FinishVoteUI");
        });
        EventManager.AddEvent("InGameUI :: CompleteVote", (p) =>
        {
            Player who = (Player)p[0];
            Player target = (Player)p[1];

            for (int i = 0; i < playerList.Count; i++)
            {
                playerList[i].CloseVoteButton();
            }

            photonView.RPC("CompleteVote", RpcTarget.MasterClient, who, target);
        });
        EventManager.AddEvent("InGameUI :: FadeIn", (p) =>
        {
            areaFaded.SetTrigger("Fade In");
            EventManager.SendEvent((string)p[0], p);
        });
        EventManager.AddEvent("InGameUI :: PlayFinishVoteAnimation", (p) =>
        {
            StartCoroutine(AnimationEnd());
        });
        EventManager.AddEvent("InGameUI :: FinishVoteAnimationPlaying", (p) =>
        {
            areaMurderPlayerUI.SetActive(false);
            areaPlayerUI.SetActive(false);
            areaDieUI.SetActive(false);
        });

        EventManager.AddEvent("Refresh Stamina", (p) =>
        {
            float value = (float)p[0];
            areaHPValue.fillAmount = value;

            if (value >= 60) areaHPValue.color = new Color(.8f, .8f, .8f);
            else if (value >= 30) areaHPValue.color = Color.white;
            else if (value >= 15) areaHPValue.color = new Color(1, 1, .4f);
            else areaHPValue.color = new Color(1, .4f, .4f);
        });
        EventManager.AddEvent("Refresh Hungry" , (p) =>
        {
            float value = (float)p[0];
            areaHungryValue.fillAmount = value;

            if (value >= 60) areaHungryValue.color = new Color(.8f, .8f, .8f);
            else if (value >= 30) areaHungryValue.color = Color.white;
            else if (value >= 15) areaHungryValue.color = new Color(1, 1, .4f);
            else areaHungryValue.color = new Color(1, .4f, .4f);
        });
        EventManager.AddEvent("Refresh Thirsty", (p) =>
        {
            float value = (float)p[0];
            areaThirstyValue.fillAmount = value;

            if (value >= 60) areaThirstyValue.color = new Color(.8f, .8f, .8f);
            else if (value >= 30) areaThirstyValue.color = Color.white;
            else if (value >= 15) areaThirstyValue.color = new Color(1, 1, .4f);
            else areaThirstyValue.color = new Color(1, .4f, .4f);
        });
        EventManager.AddEvent("Refresh Cold"   , (p) =>
        {
            float value = (float)p[0];
            areaColdValue.fillAmount = value;

            if (value >= 60) areaColdValue.color = new Color(.8f, .8f, .8f);
            else if (value >= 30) areaColdValue.color = Color.white;
            else if (value >= 15) areaColdValue.color = new Color(1, 1, .4f);
            else areaColdValue.color = new Color(1, .4f, .4f);
        });
        EventManager.AddEvent("Refresh Wet"   , (p) =>
        {
            float value = (float)p[0];
            areaWetValue.fillAmount = value;
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
            Collider col = (Collider)p[0];
            areaTrigger.SetActive(false);
            if (col.CompareTag("WarmZone"))
            {
                EventManager.SendEvent("Player :: ExitWarmZone");
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
        Player manyVotePlayer = null;
        int voteCount = 0;
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            var properties = player.CustomProperties;
            int count = (int)properties["voteMembers"];
            if (count > voteCount)
            {
                if (count > 0)
                {
                    manyVotePlayer = player;
                }
            }
        }
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
            Debug.Log($"{who.NickName} 이(가) {target.NickName}을(를) 투표했습니다.");
        }

        EventManager.SendEvent("InGameUI :: AddAlamMessage", Strings.GetString(StringKey.InGameMessageCompleteVote, who.NickName));
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
