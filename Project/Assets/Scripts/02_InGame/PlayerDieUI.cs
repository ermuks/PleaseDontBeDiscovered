using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using Photon;

public enum DieMessage
{
    None = 0,
    Hungry = 0,
    Falling,
    Vote,
    Breath,
}

public class PlayerDieUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtDieMessage;

    public void SetDie(Photon.Realtime.Player player)
    {
        txtDieMessage.text = Strings.GetString(StringKey.InGameDieMessage, player.NickName);
    }

    public void Die(DieMessage msg)
    {
        string message = "";
        switch (msg)
        {
            case DieMessage.Hungry:
                message = Strings.GetString(StringKey.InGameDieMessageHungry);
                break;
            case DieMessage.Falling:
                message = Strings.GetString(StringKey.InGameDieMessageFalling);
                break;
            case DieMessage.Vote:
                message = Strings.GetString(StringKey.InGameDieMessageVote);
                break;
            case DieMessage.Breath:
                message = Strings.GetString(StringKey.InGameDieMessageBreath);
                break;
            default:
                message = Strings.GetString(StringKey.InGameDieMessageNone);
                break;
        }
        txtDieMessage.text = message;
    }
}
