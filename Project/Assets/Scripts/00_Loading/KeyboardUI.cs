using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardUI : MonoBehaviour
{
    [SerializeField] private Button btnClose;
    private KeyCode[] ableKeys = new KeyCode[]
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E,
        KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
        KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O,
        KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T,
        KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z,

        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
        KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0,

        KeyCode.LeftShift, KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.CapsLock, KeyCode.Tab,

        KeyCode.BackQuote,

        KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4, KeyCode.F5, KeyCode.F6,
        KeyCode.F7, KeyCode.F8, KeyCode.F9, KeyCode.F10, KeyCode.F11,KeyCode.F12,
    };

    //private KeyCode[] keyCodes;

    private KeySettings currentKey;
    private KeyCode prevKey;

    [SerializeField] private Transform keyParent;
    private List<KeyCell> keyCells = new List<KeyCell>();

    private void Awake()
    {
        btnClose.onClick.AddListener(() => gameObject.SetActive(false));
        //keyCodes = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
        foreach (Transform item in keyParent)
        {
            if (item.GetComponent<KeyCell>() != null)
            {
                keyCells.Add(item.GetComponent<KeyCell>());
            }
        }
    }

    public void Open(KeySettings key)
    {
        currentKey = key;
        prevKey = Settings.instance.GetKey(key);
        for (int i = 0; i < keyCells.Count; i++)
        {
            keyCells[i].Refreash(key);
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.SetActive(false);
            }
            else
            {
                foreach (KeyCode keyCode in ableKeys)
                {
                    if (keyCode.ToString().ToLower().IndexOf("mouse") != -1) continue;
                    if (Input.GetKeyDown(keyCode))
                    {
                        Settings.instance.SetKey(currentKey, keyCode);
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
