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
    private GameObject chattingUI;

    [SerializeField]
    private Transform chatParent;

    private GameObject chatPrefab;
    private GameObject alamPrefab;
    private GameObject noticePrefab;

    [SerializeField] private ScrollRect chatList;
    [SerializeField] private TMP_InputField inputContent;
    [SerializeField] private Button btnSubmit;

    private void Awake()
    {
        chatPrefab = Resources.Load<GameObject>("Prefabs/UI/ChatMessage");
        alamPrefab = Resources.Load<GameObject>("Prefabs/UI/AlamMessage");
        noticePrefab = Resources.Load<GameObject>("Prefabs/UI/NoticeMessage");

        EventManager.AddEvent("Chatting :: Show", (p) => chattingUI.SetActive(true));
        EventManager.AddEvent("Chatting :: Hide", (p) => chattingUI.SetActive(false));

        inputContent.onSubmit.AddListener((v) =>
        {
            SendPunMessage();
        });
        btnSubmit.onClick.AddListener(() =>
        {
            SendPunMessage();
        });

        EventManager.AddEvent("AddAlamMessage", (p) =>
        {
            AddAlamMessage((string)p[0]);
        });
        EventManager.AddEvent("AddNoticeMessage", (p) =>
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
        MessageInfo item = Instantiate(chatPrefab, chatParent).GetComponent<MessageInfo>();
        item.SetMessage(player, message, richText);
        StartCoroutine(ReCalculateChattingHeight());
    }

    private void AddAlamMessage(string message)
    {
        MessageInfo item = Instantiate(alamPrefab, chatParent).GetComponent<MessageInfo>();
        item.SetMessage(message, true);
        StartCoroutine(ReCalculateChattingHeight());
    }

    private void AddNoticeMessage(string message)
    {
        MessageInfo item = Instantiate(noticePrefab, chatParent).GetComponent<MessageInfo>();
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}