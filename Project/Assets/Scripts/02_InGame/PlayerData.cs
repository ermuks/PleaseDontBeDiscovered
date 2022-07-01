using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PlayerData : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private Transform nickname;

    private CharacterController controller;
    private Animator anim;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        nickname.GetComponent<TMPro.TMP_Text>().text = photonView.Owner.NickName;
        if (photonView.IsMine)
        {
            nickname.gameObject.SetActive(false);
        }
        EventManager.AddEvent("Data :: Die", (p) =>
        {
            Die(p[0], (bool)p[1], (bool)p[2]);
        });
        EventManager.AddEvent("Data :: Hit", (p) =>
        {
            Hit((Player)p[0]);
        });
        EventManager.AddEvent("Data :: VoteDie", (p) =>
        {
            VoteDie();
        });
    }

    private void Update()
    {
        float delta = Time.deltaTime;
        Vector3 point1 = controller.bounds.center + Vector3.up * (controller.height - controller.radius * 2) / 2;
        Vector3 point2 = controller.bounds.center - Vector3.up * (controller.height - controller.radius * 2) / 2;
        if (controller.isGrounded || Physics.CapsuleCast(point1, point2, controller.radius, Vector3.down, .02f, ~(1 << gameObject.layer)))
        {
            if (anim.GetBool("Run"))
            {

            }
        }
        nickname.LookAt(Camera.main.transform);
    }


    private void VoteDie()
    {
        Die(DieMessage.Vote, false, false);
    }

    private void Hit(Player player)
    {
        GetComponent<Animator>().SetBool("Hit", true);
        if ((bool)player.CustomProperties["isMurder"])
        {
            Die(player, true, true);
        }
    }

    private void Die(object obj, bool openUI, bool setDieAnim)
    {
        var p = PhotonNetwork.LocalPlayer.CustomProperties;
        p["isDead"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(p);

        if (obj.GetType() != typeof(Player)) obj = (DieMessage)obj;
        if (openUI) EventManager.SendEvent("Player :: Die", obj);
        if (setDieAnim) EventManager.SendEvent("InGameUI :: Die", obj);

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}