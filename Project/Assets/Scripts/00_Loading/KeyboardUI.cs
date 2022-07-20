using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardUI : MonoBehaviour
{
    private KeyCode[] keyCodes;

    private KeySettings currentKey;
    private KeyCode prevKey;

    [SerializeField] private Transform keyParent;
    private List<KeyCell> keyCells = new List<KeyCell>();

    private void Awake()
    {
        keyCodes = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
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
            foreach (KeyCode keyCode in keyCodes)
            {
                if (keyCode.ToString().ToLower().IndexOf("mouse") != -1) continue;
                if (Input.GetKeyDown(keyCode))
                {
                    Debug.Log(keyCode);
                    Settings.instance.SetKey(currentKey, keyCode);
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
