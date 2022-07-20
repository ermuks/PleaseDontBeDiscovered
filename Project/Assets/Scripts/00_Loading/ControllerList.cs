using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControllerList : MonoBehaviour
{
    [SerializeField] private Image imgKeyBackground;
    [SerializeField] private TMP_Text txtDescript;
    [SerializeField] private TMP_Text txtKeyCode;
    [SerializeField] private Button btnOpenKeyboard;

    private KeySettings mySetting;

    public void Init(KeySettings descript, KeyCode keyCode)
    {
        mySetting = descript;
        btnOpenKeyboard.onClick.AddListener(() =>
        {
            Settings.instance.OpenKeyboard(descript);
        });
        Refresh();
    }

    public void Refresh()
    {
        txtDescript.text = Strings.GetString((StringKey)System.Enum.Parse(typeof(StringKey), "SettingsKeyDescript" + mySetting));
        txtKeyCode.text = Settings.instance.GetKey(mySetting).ToString();
    }
}
