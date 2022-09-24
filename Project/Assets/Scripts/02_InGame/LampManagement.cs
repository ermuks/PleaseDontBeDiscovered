using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class LampManagement : MonoBehaviourPun, IPunObservable
{
    private Material matTurnOn;
    private Material matTurnOff;
    private Light _light;
    private MeshRenderer _renderer;

    private bool isOn = false;

    private void Awake()
    {
        matTurnOn = Resources.Load<Material>("Materials/Lamp On Road");
        matTurnOff = Resources.Load<Material>("Materials/Lamp Off Road");
        _light = transform.Find("Lamp Light").GetComponent<Light>();
        _renderer = transform.Find("Cube").GetComponent<MeshRenderer>();
        RefreshLamp();
    }

    public void TurnOnLamp()
    {
        isOn = true;
        RefreshLamp();
    }

    public void TurnOffLamp()
    {
        isOn = false;
        RefreshLamp();
    }

    private void RefreshLamp()
    {
        _light.enabled = isOn;
        _renderer.material = isOn ? matTurnOn : matTurnOff;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isOn);
        }
        else
        {
            isOn = (bool)stream.ReceiveNext();
        }
    }
}
