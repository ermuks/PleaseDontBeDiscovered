using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyCell : MonoBehaviour
{
    [SerializeField] Image imgBackground;

    public KeyCode code;

    private KeySettings[] keys;

    private KeySettings currentKey;

    private void Awake()
    {
        keys = (KeySettings[])System.Enum.GetValues(typeof(KeySettings));
        System.Array.Resize(ref keys, keys.Length - 1);
        GetComponent<Button>().onClick.AddListener(() =>
        {
            Settings.instance.SetKey(currentKey, code);
            Settings.instance.CloseKeyboard();
        });
    }

    public void Refreash(KeySettings key)
    {
        currentKey = key;
        bool result = true;
        for (int i = 0; i < keys.Length; i++)
        {
            if (Settings.instance.GetKey(keys[i]) == code)
            {
                GetComponent<Image>().color = new Color(.94f, .64f, .72f);
                result = false;
                break;
            }
        }
        if (Settings.instance.GetKey(key) == code)
        {
            GetComponent<Image>().color = new Color(.92f, .92f, .44f);
            result = false;
        }
        if (result)
        {
            GetComponent<Image>().color = Color.white;
        }
        GetComponent<Button>().interactable = result;
    }
}