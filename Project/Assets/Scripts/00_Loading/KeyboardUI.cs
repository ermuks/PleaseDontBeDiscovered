using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardUI : MonoBehaviour
{
    private KeyCode[] keyCodes;

    private KeySettings currentKey;
    private KeyCode prevKey;

    private void Awake()
    {
        keyCodes = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
    }

    public void Open(KeySettings key)
    {
        currentKey = key;
        prevKey = Settings.instance.GetKey(key);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in keyCodes)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    Settings.instance.SetKey(currentKey, keyCode);
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
