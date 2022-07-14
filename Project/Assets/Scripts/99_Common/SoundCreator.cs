using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCreator : MonoBehaviour
{
    private GameObject soundPlayer;

    private void Awake()
    {
        soundPlayer = Resources.Load<GameObject>("Prefabs/SoundPlayer");
        EventManager.AddEvent("Sound :: Create 2D", (p) =>
        {
            SoundPlayer player = Instantiate(soundPlayer).GetComponent<SoundPlayer>();
            player.Play(SoundManager.GetItem((string)p[0]), (float)p[1]);
        });
        EventManager.AddEvent("Sound :: Create 3D", (p) =>
        {
            SoundPlayer player = Instantiate(soundPlayer).GetComponent<SoundPlayer>();
            player.Play(SoundManager.GetItem((string)p[0]), (Vector3)p[1], (float)p[2], (float)p[3]);
        });
    }
}
