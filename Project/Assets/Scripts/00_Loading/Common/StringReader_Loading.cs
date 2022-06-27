using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class StringReader_Loading : MonoBehaviour
{
    [SerializeField] private TMP_Text txtInitializeConnectingMessage;

    private void Awake()
    {
        Strings.SetLanguage(Language.Korean);
        txtInitializeConnectingMessage.text = Strings.GetString(StringKey.InitializeConnectingMessage);
    }
}
