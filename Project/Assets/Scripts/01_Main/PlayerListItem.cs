using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

using Photon.Pun;
using Photon.Realtime;

public class PlayerListItem : MonoBehaviour, IPointerClickHandler
{
    private Player owner;

    [SerializeField]
    private TMP_Text txtNickname;

    [SerializeField]
    private Image statusObject;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            EventManager.SendEvent("MainUI :: OpenMasterMenu", eventData.position, owner);
        }
    }
}
