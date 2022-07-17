using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class ChattingCallbacks : MonoBehaviourPun, IPunObservable
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

        EventManager.AddEvent("Chatting :: Clear", (p) =>
        {
            ChattingClear();
        });
        EventManager.AddEvent("Chatting :: AddAlamMessage", (p) =>
        {
            AddAlamMessage((string)p[0]);
        });
        EventManager.AddEvent("Chatting :: AddNoticeMessage", (p) =>
        {
            AddNoticeMessage((string)p[0]);
        });
    }

    private void SendPunMessage()
    {
        if (inputContent.text.Replace(" ","").Length > 0)
        {
            photonView.RPC("RecieveMessage", RpcTarget.All, PhotonNetwork.LocalPlayer, inputContent.text);
            inputContent.text = "";
            inputContent.ActivateInputField();
        }
    }

    [PunRPC]
    private void RecieveMessage(Player player, string messsage)
    {
        AddMessage(player, messsage, false);
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
        item.SetMessage(player, message, richText);
        StartCoroutine(ReCalculateChattingHeight());
    }

    private void AddAlamMessage(string message)
    {
        ChatMessage item = Instantiate(alamPrefab, chatParent).GetComponent<ChatMessage>();
        item.SetMessage(message, true);
        StartCoroutine(ReCalculateChattingHeight());
    }

    private void AddNoticeMessage(string message)
    {
        ChatMessage item = Instantiate(noticePrefab, chatParent).GetComponent<ChatMessage>();
        item.SetMessage(message, true);
        StartCoroutine(ReCalculateChattingHeight());
    }

    private IEnumerator ReCalculateChattingHeight()
    {
        yield return new WaitForEndOfFrame();
        Vector2 pos = chatList.content.anchoredPosition;
        pos.y = chatList.content.rect.height;
        chatList.content.anchoredPosition = pos;
    }

    private void ChattingClear()
    {
        foreach (Transform chat in chatParent)
        {
            Destroy(chat.gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
