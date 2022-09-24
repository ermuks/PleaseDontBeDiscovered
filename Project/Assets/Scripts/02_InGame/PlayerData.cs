using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public enum PlayerColor
{
    Red = 0, Orange, Pink, Yellow, 
    Apricot, Lime, LightGreen, Green, 
    Mint, Cyan, Blue, Purple, 
    Lavender, Magenta, Black, Brown, 
    LightBrown, Gray
}

public class PlayerData : MonoBehaviourPun, IPunObservable
{
    public static Color[] colors = new Color[]
    {
            new Color(1f, 0f, 0f), // Red
            new Color(1f, .5f, 0f), // Orange
            new Color(1f, .5f, 1f), // Pink
            new Color(1f, 1f, 0f), // Yellow
            new Color(1f, .8f, .5f), // Apricot
            new Color(.5f, 1f, .5f), // Lime
            new Color(.2f, 1f, .2f), // LightGreen
            new Color(0f, 0f, .5f), // Green
            new Color(.5f, 1f, .8f), // Mint
            new Color(0f, 1f, 1f), // Cyan
            new Color(0f, 0f, 1f), // Blue
            new Color(.5f, 0f, 1f), // Purple
            new Color(.8f, .5f, 1f), // Lavender
            new Color(.2f, .2f, .2f), // Black
            new Color(.5f, .2f, 0f), // Brown
            new Color(.8f, .4f, .2f), // LightBrown
            new Color(.6f, .6f, .6f), // Gray
    };
    public static Color GetColor(Player player)
    {
        int index = (int)player.CustomProperties["color"];
        index = Mathf.Clamp(index, 0, colors.Length - 1);
        return colors[index];
    }
    public static Color GetColor(int index)
    {
        index = Mathf.Clamp(index, 0, colors.Length - 1);
        return colors[index];
    }

    [SerializeField] private Transform nickname;
    [SerializeField] private GameObject reportArea;
    [SerializeField] private GameObject[] childs;

    [SerializeField] private Light handLight;

    [SerializeField] private Transform objHair;
    [SerializeField] private Transform objHat;
    [SerializeField] private Transform objCloth;
    [SerializeField] private Transform objPants;
    [SerializeField] private Transform objGlovesLeft;
    [SerializeField] private Transform objGlovesRight;
    [SerializeField] private Transform objShoes;

    private bool handLightEnabled = false;

    public GameObject ReportArea => reportArea;
    private CharacterController controller;
    private Animator anim;

    [SerializeField]
    private SkinnedMeshRenderer skin;

    private void Awake()
    {
        Player player = photonView.Owner;
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        nickname.GetComponent<TMPro.TMP_Text>().text = player.NickName;
        skin.material.SetColor("_Color", GetColor(player));

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
            
        }
        nickname.rotation = Camera.main.transform.rotation;
    }

    private void WalkSound()
    {
        EventManager.SendEvent(
            "Sound :: Create 3D",
            $"FootstepGrass0{Random.Range(1, 4)}",
            controller.bounds.center + Vector3.down * controller.bounds.extents.y, 
            17f, 
            .4f);
    }

    public void DestroyPlayer()
    {
        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].gameObject.SetActive(false);
        }
        reportArea.SetActive(false);
    }

    private void EnableReportArea()
    {
        reportArea.SetActive(true);
    }

    private void VoteDie()
    {
        Die(DieMessage.Vote, false, false);
        EventManager.SendEvent("Player :: VoteDie");
    }

    public void ToggleLight()
    {
        handLight.enabled = !handLight.enabled;
        handLightEnabled = handLight.enabled;
        RefreshHandLight();
    }

    private void RefreshHandLight()
    {
        handLight.enabled = handLightEnabled;
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
        if (stream.IsWriting)
        {
            stream.SendNext(handLightEnabled);
        }
        else
        {
            bool value = (bool)stream.ReceiveNext();
            if (handLightEnabled != value)
            {
                handLightEnabled = value;
                RefreshHandLight();
            }
        }
    }
}