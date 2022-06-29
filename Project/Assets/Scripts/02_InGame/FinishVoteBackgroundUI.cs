using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Photon.Pun;

public class FinishVoteBackgroundUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text txtIsMurder;
    [SerializeField]
    private TMP_Text txtMurderCount;

    private Material mat;
    public GameObject obj;
    public Color color = Vector4.one;
    public float value = 0;

    private void Awake()
    {
        mat = obj.GetComponent<Image>().material;
    }

    public void SetMessage(Player player)
    {
        int murderCount = 0;

        foreach (var p in PhotonNetwork.CurrentRoom.Players.Values)
        {
            var properties = p.CustomProperties;
            if (!(bool)properties["isDead"] && (bool)properties["isMurder"]) murderCount++;
        }
        if (player == null)
        {
            txtIsMurder.text = Strings.GetString(StringKey.InGameFinishVoteNone);
            txtMurderCount.text = Strings.GetString(StringKey.InGameFinishVoteMurderCount, murderCount);
        }
        else
        {
            if ((bool)player.CustomProperties["isMurder"])
            {
                txtIsMurder.text = Strings.GetString(StringKey.InGameFinishVoteMurderTrue, player.NickName);
                txtMurderCount.text = Strings.GetString(StringKey.InGameFinishVoteMurderCount, murderCount);
            }
            else
            {
                txtIsMurder.text = Strings.GetString(StringKey.InGameFinishVoteMurderFalse, player.NickName);
                txtMurderCount.text = Strings.GetString(StringKey.InGameFinishVoteMurderCount, murderCount);
            }
        }
    }

    private void Update()
    {
        mat.SetFloat("_Size", value);
        mat.SetColor("_Color", color);
    }

    private void EndAnimation()
    {
        EventManager.SendEvent("InGameUI :: FadeIn", "");
        EventManager.SendEvent("InGameData :: FinishVoteAnimationPlaying", false);
        gameObject.SetActive(false);
    }
}
