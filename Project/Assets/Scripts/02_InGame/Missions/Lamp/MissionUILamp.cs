using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionUILamp : MonoBehaviour
{
    private static MissionUILamp instance;
    public static MissionUILamp Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<MissionUILamp>();
            return instance;
        }
    }

    bool crashLamp = false;
    bool normalLamp = false;

    public void TakeOutCrashLamp()
    {
        crashLamp = true;
        ConfirmLampStatus();
    }

    public void PutInLamp()
    {
        normalLamp = true;
        ConfirmLampStatus();
    }

    private void ConfirmLampStatus()
    {
        if (crashLamp && normalLamp)
        {
            // 미션 클리어
            Debug.Log("미션 완료");
        }
    }
}
