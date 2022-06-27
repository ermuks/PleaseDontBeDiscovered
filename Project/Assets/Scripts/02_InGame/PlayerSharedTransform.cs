using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerSharedTransform : MonoBehaviourPun, IPunObservable
{
    private Vector3 newPosition;
    private Quaternion newRotation;

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, .2f);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, .2f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            newPosition = (Vector3)stream.ReceiveNext();
            newRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
