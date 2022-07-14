using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    AudioSource audio;

    public void Play(AudioClip clip, Vector3 pos, float maxDistance, float volume)
    {
        audio = GetComponent<AudioSource>();
        audio.clip = clip;
        audio.spatialBlend = 1;
        audio.maxDistance = maxDistance;
        audio.volume = volume;
        transform.position = pos;
        if (!audio.isPlaying) audio.Play();
        StartCoroutine(AutoDestroy());
    }

    public void Play(AudioClip clip, float volume)
    {
        audio = GetComponent<AudioSource>();
        audio.clip = clip;
        audio.spatialBlend = 0;
        audio.volume = volume;
        if (!audio.isPlaying) audio.Play();
        StartCoroutine(AutoDestroy());
    }

    private IEnumerator AutoDestroy()
    {
        if (audio == null) audio = GetComponent<AudioSource>();
        yield return new WaitUntil(() => !audio.isPlaying);
        //Destroy(gameObject);
    }
}
