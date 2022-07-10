using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

using Photon.Pun;
using Photon.Realtime;

public class PlayerListItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Player owner;

    [SerializeField]
    private Text txtNickname;

    [SerializeField]
    private Image playerColor;

    [SerializeField]
    private Image statusObject;

    [SerializeField]
    private RectTransform areaNickname;
    [SerializeField]
    private GameObject areaPlayerManagementButtons;

    [SerializeField]
    private Color clrNone;
    [SerializeField]
    private Color clrNormal;
    [SerializeField]
    private Color clrReady;
    [SerializeField]
    private Color clrMaster;

    public void Refresh(Player player)
    {
        owner = player;
        if ((bool)player.CustomProperties["isReady"])
        {
            statusObject.color = clrReady;
        }
        else
        {
            if (player.IsMasterClient)
            {
                statusObject.color = clrMaster;
            }
            else
            {
                statusObject.color = clrNormal;
            }
        }
        txtNickname.text = player.NickName;
    }

    public void Clear()
    {
        statusObject.color = clrNone;
        txtNickname.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector2 size = areaNickname.sizeDelta;
        size.x = 196f;
        areaNickname.sizeDelta = size;
        areaPlayerManagementButtons.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Vector2 size = areaNickname.sizeDelta;
        size.x = 286f;
        areaNickname.sizeDelta = size;
        areaPlayerManagementButtons.SetActive(false);
    }
}
