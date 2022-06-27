using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class VotePlayerListItem : MonoBehaviour
{
    public Player player;
    [SerializeField] private Image imgProfile;
    [SerializeField] private GameObject imgDie;
    [SerializeField] private TMP_Text txtNickname;

    public void Init(Player player, Texture2D img)
    {
        this.player = player;
        if (img != null)
        {
            imgProfile.sprite = Sprite.Create(img, new Rect(Vector2.zero, new Vector2(img.width, img.height)), Vector2.zero);
        }
        txtNickname.text = player.NickName;
    }

    public void SetDie(bool isDead)
    {
        imgDie.SetActive(isDead);
    }
}
