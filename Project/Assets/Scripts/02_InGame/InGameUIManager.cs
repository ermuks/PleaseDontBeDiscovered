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

    //[SerializeField] private Image areaHPValue;
    //[SerializeField] private Image areaHungryValue;
    //[SerializeField] private Image areaThirstyValue;
    //[SerializeField] private Image areaColdValue;
    //[SerializeField] private Image areaWetValue;
    //[SerializeField] private Image areaBreathValue;

    //[SerializeField] private GameObject areaNormalPlayerUI;
    //[SerializeField] private GameObject areaMurderPlayerUI;
    [SerializeField] private Image imgCooldownTimer;
    [SerializeField] private TMP_Text txtCooldownTimer;

    [SerializeField] private GameObject areaTriggerParent;
    [SerializeField] private GameObject areaTrigger;
    [SerializeField] private GameObject areaReport;
    [SerializeField] private GameObject areaWater;
    [SerializeField] private GameObject areaDieUI;
    [SerializeField] private GameObject areaPhone;
    //[SerializeField] private GameObject areaInventory;
    //[SerializeField] private GameObject areaPlayerUI;
    [SerializeField] private GameObject areaPlayerWorkProgressUI;
    [SerializeField] private GameObject areaWatcherUI;
    [SerializeField] private GameObject areaGameOver;

    [SerializeField] private GameObject areaVote;
    [SerializeField] private GameObject areaFinishVote;
    [SerializeField] private Transform playerListParent;
    private GameObject playerListPrefab;
    private List<VotePlayerListItem> votePlayerList = new List<VotePlayerListItem>();
    private int voteCount = 0;
    private int voteAmount = 0;

    private bool isVoting = false;
    private float maxTime;
    private float timer;

    private Coroutine voteTimer;

    [SerializeField] private Animator areaFaded;
    [SerializeField] private Transform voteEndingCamPosition;
    [SerializeField] private Transform voteEndingCamAnimation;

    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform murdererCamera;

    [SerializeField] private RectTransform minimap;
    [SerializeField] private RectTransform minimapPlayer;
    [SerializeField] private RectTransform minimapPlayerSight;
    [SerializeField] private Image minimapPlayerColor;

    private void Awake()
    {
        messageUIPrefab = Resources.Load<GameObject>("Prefabs/UI/InGameMessageUI");
        bloodUIPrefab = Resources.Load<GameObject>("Prefabs/UI/BloodEffect");

        playerListPrefab = Resources.Load<GameObject>("Prefabs/UI/VotePlayerListItem");

        minimapPlayerColor.color = PlayerData.GetColor((int)PhotonNetwork.LocalPlayer.CustomProperties["color"]);

        areaFinishVote.SetActive(true);
        areaDieUI.SetActive(true);
        areaWatcherUI.SetActive(true);
        areaGameOver.SetActive(true);
        areaTriggerParent.SetActive(true);
        areaReport.SetActive(true);
        areaTrigger.SetActive(true);
        areaVote.SetActive(true);
        areaPlayerWorkProgressUI.SetActive(true);
        areaFinishVote.SetActive(false);
        areaDieUI.SetActive(false);
        areaWatcherUI.SetActive(false);
        areaGameOver.SetActive(false);
        areaTriggerParent.SetActive(true);
        areaReport.SetActive(false);
        areaTrigger.SetActive(false);
        areaVote.SetActive(false);
        areaPlayerWorkProgressUI.SetActive(false);

        bool isMurderer = (bool)PhotonNetwork.LocalPlayer.CustomProperties["isMurderer"];

        maxTime = (float)PhotonNetwork.CurrentRoom.CustomProperties["voteTime"];

        EventManager.AddEvent("PUN :: Hit", (p) =>
        {
            photonView.RPC("Hit", (Player)p[1], (Player)p[0]);
        });

        EventManager.AddData("InGameUI >> playerCamera", (p) => playerCamera);
        EventManager.AddData("InGameUI >> murdererCamera", (p) => murdererCamera);

        EventManager.AddData("InGameUI >> VoteUIActive", (p) => areaVote.activeSelf);
        EventManager.AddData("InGameUI >> VoteEndingPosition", (p) => voteEndingCamPosition);
        EventManager.AddData("InGameUI >> FinishVoteAnimationActive", (p) => areaFinishVote.activeSelf);

        RefreshVotePlayerList();
        EventManager.AddEvent("InGameUI :: OpenVoteUI", (p) =>
        {
            RefreshVotePlayerList();
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

            if (voteTimer != null) StopCoroutine(voteTimer);

            isVoting = true;
            timer = 0;
            voteTimer = StartCoroutine(VoteTimer());

            RefreshVoteAmount();
            EventManager.SendEvent("InGameUI :: VoteChatInit");
        });
        EventManager.AddEvent("InGameUI :: RefreshVotePlayerList", (p) => RefreshVotePlayerList());
        EventManager.AddEvent("InGameUI :: FinishVoteUI", (p) =>
        {
            EventManager.SendEvent("InGameData :: FinishVoteAnimationPlaying", true);
            EventManager.SendEvent("InGameUI :: PlayFinishVoteAnimation");
        });
        EventManager.AddEvent("InGameUI :: EndVote", (p) =>
        {
            areaVote.GetComponent<VoteUI>().EndVote();
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

            photonView.RPC("CompleteVote", RpcTarget.All, who, target);
        });
        EventManager.AddEvent("InGameUI :: CompleteSkip", (p) =>
        {
            Player who = (Player)p[0];

            photonView.RPC("CompleteVote", RpcTarget.All, who, null);
        });
        EventManager.AddEvent("InGameUI :: CloseVoteButtons", (p) =>
        {
            for (int i = 0; i < votePlayerList.Count; i++)
            {
                votePlayerList[i].CloseVoteButton();
            }
        });
        EventManager.AddEvent("InGameUI :: FadeIn", (p) =>
        {
            areaFaded.SetTrigger("Faded");
            EventManager.SendEvent((string)p[0], p);
        });
        EventManager.AddEvent("InGameUI :: PlayFinishVoteAnimation", (p) =>
        {
            StartCoroutine(AnimationEnd());
            //areaPlayerUI.SetActive(false);
            areaDieUI.SetActive(false);
            areaTriggerParent.SetActive(false);
            EventManager.SendEvent("InGameData :: PlayerPositionSetting");
            EventManager.SendEvent("InGameData :: ClearDeadPlayer");
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
                //areaPlayerUI.SetActive(true);
            }
            areaTriggerParent.SetActive(true);
        });

        //EventManager.AddEvent("Refresh Stamina", (p) =>
        //{
        //    float value = (float)p[0];
        //    areaHPValue.fillAmount = value;

        //    if (value >= .6f) areaHPValue.color = new Color(.8f, .8f, .8f);
        //    else if (value >= .3f) areaHPValue.color = Color.white;
        //    else if (value >= .15f) areaHPValue.color = new Color(1, 1, .4f);
        //    else areaHPValue.color = new Color(1, .4f, .4f);
        //});
        EventManager.AddEvent("Refresh Minimap", (p) =>
        {
            Transform player = (Transform)p[0];
            float width = minimap.rect.width / 400f;
            float height = minimap.rect.height / 400f;
            //minimapPlayer.anchoredPosition = new Vector2(player.position.x * width, player.position.z * height);
            minimapPlayer.localRotation = Quaternion.Euler(0, 0, -player.eulerAngles.y);
            minimap.anchoredPosition = -new Vector2(player.position.x * width, player.position.z * height);
        });
        //EventManager.AddEvent("Refresh Hungry" , (p) =>
        //{
        //    float value = (float)p[0];
        //    areaHungryValue.fillAmount = value;

        //    if (value >= .6f) areaHungryValue.color = new Color(.8f, .8f, .8f);
        //    else if (value >= .3f) areaHungryValue.color = Color.white;
        //    else if (value >= .15f) areaHungryValue.color = new Color(1, 1, .4f);
        //    else areaHungryValue.color = new Color(1, .4f, .4f);
        //});
        //EventManager.AddEvent("Refresh Thirsty", (p) =>
        //{
        //    float value = (float)p[0];
        //    areaThirstyValue.fillAmount = value;

        //    if (value >= .6f) areaThirstyValue.color = new Color(.8f, .8f, .8f);
        //    else if (value >= .3f) areaThirstyValue.color = Color.white;
        //    else if (value >= .15f) areaThirstyValue.color = new Color(1, 1, .4f);
        //    else areaThirstyValue.color = new Color(1, .4f, .4f);
        //});
        //EventManager.AddEvent("Refresh Breath"   , (p) =>
        //{
        //    float value = (float)p[0];
        //    areaBreathValue.fillAmount = value;

        //    if (value <= .5f) areaBreathValue.color = new Color(.8f, .8f, .8f);
        //    else if (value <= .7f) areaBreathValue.color = new Color(1, .8f, .8f);
        //    else if (value <= .9f) areaBreathValue.color = new Color(1, .4f, .4f);
        //    else areaBreathValue.color = new Color(.8f, .2f, .2f);
        //});
        //EventManager.AddEvent("Refresh Wet"   , (p) =>
        //{
        //    float value = (float)p[0];
        //    areaWetValue.fillAmount = value;
        //});
        //EventManager.AddEvent("Refresh Cold", (p) =>
        //{
        //    float value = (float)p[0];
        //    areaColdValue.fillAmount = value;

        //    if (value >= .6f) areaColdValue.color = new Color(.8f, .8f, .8f);
        //    else if (value >= .3f) areaColdValue.color = Color.white;
        //    else if (value >= .15f) areaColdValue.color = new Color(1, 1, .4f);
        //    else areaColdValue.color = new Color(1, .4f, .4f);
        //});
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
            //areaPlayerUI.SetActive(false);
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
            bool murdererWin = (bool)p[0];
            //areaPlayerUI.SetActive(false);
            areaDieUI.SetActive(false);
            areaWatcherUI.SetActive(false);
            areaGameOver.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            areaGameOver.GetComponent<GameOverUI>().SetGameOver(murdererWin);
            EventManager.SendEvent("Player :: EndGame", murdererWin);
        });
        EventManager.AddEvent("InGameUI :: TriggerEnter", (p) =>
        {
            Collider col = (Collider)p[0];
            if (col.CompareTag("ReportArea"))
            {
                areaReport.SetActive(true);
                areaReport.GetComponent<ReportUI>().SetMessage();
            }
            else if (col.CompareTag("TreeZone"))
            {
                areaTrigger.SetActive(true);
                areaTrigger.GetComponent<TriggerUI>().SetMessage(WorkMessage.Treezone, col);
            }
            else if (col.CompareTag("WaterZone") || col.CompareTag("DeepWater"))
            {
                areaTrigger.SetActive(true);
                areaTrigger.GetComponent<TriggerUI>().SetMessage(WorkMessage.WaterZone, col);
            }
            else if (col.CompareTag("FishZone"))
            {
                areaTrigger.SetActive(true);
                areaTrigger.GetComponent<TriggerUI>().SetMessage(WorkMessage.FishZone, col);
            }
            else if (col.CompareTag("LightZone"))
            {
                areaTrigger.SetActive(true);
                areaTrigger.GetComponent<TriggerUI>().SetMessage(WorkMessage.LightZone, col);
            }
            else if (col.CompareTag("CampFire"))
            {
                areaTrigger.SetActive(true);
                areaTrigger.GetComponent<TriggerUI>().SetMessage(WorkMessage.CampFire, col);
            }
            else if (col.CompareTag("TableZone"))
            {
                areaTrigger.SetActive(true);
                areaTrigger.GetComponent<TriggerUI>().SetMessage(WorkMessage.TableZone, col);
            }
            else if (col.CompareTag("ChestZone"))
            {
                areaTrigger.SetActive(true);
                areaTrigger.GetComponent<TriggerUI>().SetMessage(WorkMessage.ChestZone, col);
            }
            else if (col.CompareTag("PictureZone"))
            {
                areaTrigger.SetActive(true);
                areaTrigger.GetComponent<TriggerUI>().SetMessage(WorkMessage.PictureZone, col);
            }
            else
            {
                areaTrigger.SetActive(true);
                areaTrigger.GetComponent<TriggerUI>().SetMessage(WorkMessage.None, col);
            }
        });
        EventManager.AddEvent("InGameUI :: TriggerExit", (p) =>
        {
            areaTrigger.SetActive(false);
            if (p != null)
            {
                Collider col = (Collider)p[0];
            }
        });
        EventManager.AddEvent("InGameUI :: ReportExit", (p) =>
        {
            areaReport.SetActive(false);
        });
        EventManager.AddEvent("InGameUI :: WorkStart", (p) =>
        {
            areaPlayerWorkProgressUI.SetActive(true);
            areaPlayerWorkProgressUI.GetComponent<PlayerWorkProgressUI>().SetWork((WorkMessage)p[0], (Collider)p[1]);
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
        //EventManager.AddEvent("InGameUI :: OpenInventory", (p) =>
        //{
        //    Cursor.lockState = CursorLockMode.None;
        //    areaInventory.SetActive(true);
        //    areaInventory.GetComponent<InventoryUI>().Init(((Collider)p[0]).GetComponent<PublicInventory>());
        //});
        //EventManager.AddEvent("InGameUI :: CloseInventory", (p) =>
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //    areaInventory.SetActive(false);
        //});
        EventManager.AddEvent("InGameUI :: TogglePhone", (p) =>
        {
            areaPhone.SetActive(!areaPhone.activeSelf);
            if (areaPhone.activeSelf)
            {

            }
            else
            {

            }
        });
    }

    private IEnumerator VoteTimer()
    {
        while (isVoting)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                timer += Time.deltaTime;

                if (timer >= maxTime)
                {
                    isVoting = false;
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        var roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
                        roomProperties["Vote"] = false;
                        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
                    }
                }
            }
            yield return null;
        }
    }

    private void RefreshVotePlayerList()
    {
        for (int i = 0; i < votePlayerList.Count; i++)
        {
            Destroy(votePlayerList[i].gameObject);
        }
        votePlayerList.Clear();

        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            VotePlayerListItem votePlayer = Instantiate(playerListPrefab, playerListParent).GetComponent<VotePlayerListItem>();
            votePlayer.Init(player);
            votePlayer.SetDie((bool)player.CustomProperties["isDead"]);
            votePlayerList.Add(votePlayer);
        }
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
        int skipCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["skipCount"];
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
        if (skipCount >= voteCount) manyVotePlayer = null;
        areaFinishVote.GetComponent<FinishVoteBackgroundUI>().SetMessage(manyVotePlayer);
        yield return new WaitUntil(() => !areaFinishVote.activeSelf);
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
            voteCount++;
            if (target != null)
            {
                var properties = target.CustomProperties;
                properties["voteMembers"] = (int)properties["voteMembers"] + 1;
                target.SetCustomProperties(properties);
            }
            else
            {
                var properties = PhotonNetwork.CurrentRoom.CustomProperties;
                properties["skipCount"] = (int)properties["skipCount"] + 1;
                PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
            }
            if (voteCount >= voteAmount)
            {
                var roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
                roomProperties["Vote"] = false;
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
            }
        }
        foreach (var playerListItem in votePlayerList)
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
        if (stream.IsWriting)
        {
            stream.SendNext(timer);
            EventManager.SendEvent("VoteUI :: RefreshTimer", maxTime, timer);
        }
        else
        {
            timer = (float)stream.ReceiveNext();
            EventManager.SendEvent("VoteUI :: RefreshTimer", maxTime, timer);
        }
    }
}
