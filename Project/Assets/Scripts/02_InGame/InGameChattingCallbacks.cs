using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class InGameChattingCallbacks : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    private Transform chatParent;

    private GameObject myPrefab;
    private GameObject otherPrefab;
    private GameObject alamPrefab;
    private GameObject noticePrefab;

    [SerializeField] private ScrollRect chatList;
    [SerializeField] private TMP_InputField inputContent;
    [SerializeField] private Button btnSubmit;

    private List<ChatMessage> messages = new List<ChatMessage>();

    private void Awake()
    {
        myPrefab = Resources.Load<GameObject>("Prefabs/UI/MyMessage");
        otherPrefab = Resources.Load<GameObject>("Prefabs/UI/ChatMessage");
        alamPrefab = Resources.Load<GameObject>("Prefabs/UI/AlamMessage");
        noticePrefab = Resources.Load<GameObject>("Prefabs/UI/NoticeMessage");

        inputContent.onSubmit.AddListener((v) =>
        {
            SendPunMessage();
        });
        btnSubmit.onClick.AddListener(() =>
        {
            SendPunMessage();
        });

        EventManager.AddEvent("InGameUI :: AddAlamMessage", (p) =>
        {
            AddAlamMessage((string)p[0]);
        });
        EventManager.AddEvent("InGameUI :: AddNoticeMessage", (p) =>
        {
            AddNoticeMessage((string)p[0]);
        });
        EventManager.AddEvent("InGameUI :: VoteChatInit", (p) =>
        {
            Init();
        });
    }

    private void SendPunMessage()
    {
        if (inputContent.text.Replace(" ", "").Length > 0)
        {
            photonView.RPC("InGameRecieveMessage", RpcTarget.All, PhotonNetwork.LocalPlayer, inputContent.text);
            inputContent.text = "";
            inputContent.ActivateInputField();
        }
    }

    [PunRPC]
    private void InGameRecieveMessage(Player player, string messsage)
    {
        bool deadPlayer = (bool)player.CustomProperties["isDead"];
        bool isDead = (bool)PhotonNetwork.LocalPlayer.CustomProperties["isDead"];
        bool isMurderer = (bool)PhotonNetwork.LocalPlayer.CustomProperties["isMurder"];
        //   dead    me
        //    t      t      t
        //    f      t      t
        //    t      f      f
        //    f      f      t
        if (deadPlayer)
        {
            if (isDead)
            {
                AddMessage(player, messsage, false);
            }
            else
            {
                if (isMurderer)
                {
                    AddMessage(player, messsage, false);
                }
            }
        }
        else
        {
            AddMessage(player, messsage, false);
        }
    }

    private void Init()
    {
        for (int i = 0; i < messages.Count; i++)
        {
            Destroy(messages[i].gameObject);
        }
        messages.Clear();
    }

    private void AddMessage(Player player, string message, bool richText)
    {
        GameObject chatPrefab;
        if (player == PhotonNetwork.LocalPlayer)
        {
            chatPrefab = myPrefab;
        }
        else
        {
            chatPrefab = otherPrefab;
        }
        ChatMessage item = Instantiate(chatPrefab, chatParent).GetComponent<ChatMessage>();
        item.SetMessage(player, message, richText, true);
        messages.Add(item);
        StartCoroutine(ReCalculateChattingHeight());
    }

    private void AddAlamMessage(string message)
    {
        ChatMessage item = Instantiate(alamPrefab, chatParent).GetComponent<ChatMessage>();
        item.SetMessage(message, true);
        messages.Add(item);
        StartCoroutine(ReCalculateChattingHeight());
    }

    private void AddNoticeMessage(string message)
    {
        ChatMessage item = Instantiate(noticePrefab, chatParent).GetComponent<ChatMessage>();
        item.SetMessage(message, true);
        messages.Add(item);
        StartCoroutine(ReCalculateChattingHeight());
    }

    private IEnumerator ReCalculateChattingHeight()
    {
        yield return new WaitForEndOfFrame();
        Vector2 pos = chatList.content.anchoredPosition;
        pos.y = chatList.content.rect.height;
        chatList.content.anchoredPosition = pos;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
