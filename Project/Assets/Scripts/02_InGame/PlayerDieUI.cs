using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum DieMessage
{
    None = 0,
    Hungry = 0,
    Falling,
}

public class PlayerDieUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtDieMessage;

    public void SetDie(Photon.Realtime.Player player)
    {
        txtDieMessage.text = Strings.GetString(StringKey.InGameDieMessage).Replace("#Killer#", player.NickName);
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
            default:
                message = Strings.GetString(StringKey.InGameDieMessageNone);
                break;
        }
        txtDieMessage.text = message;
    }
}
