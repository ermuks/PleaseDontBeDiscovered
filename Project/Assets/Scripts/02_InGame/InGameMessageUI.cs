using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameMessageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtMessage;

    public void SetMessage(string msg)
    {
        txtMessage.text = msg;
    }

    private void EndAnimation()
    {
        Destroy(gameObject);
    }
}
