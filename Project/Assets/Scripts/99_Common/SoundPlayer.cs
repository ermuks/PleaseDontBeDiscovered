using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    AudioSource audioSource;

    public void Play(AudioClip clip, Vector3 pos, float maxDistance, float volume)
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.spatialBlend = 1;
        audioSource.maxDistance = maxDistance;
        audioSource.volume = volume;
        transform.position = pos;
        if (!audioSource.isPlaying) audioSource.Play();
        StartCoroutine(AutoDestroy());
    }

    public void Play(AudioClip clip, float volume)
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.spatialBlend = 0;
        audioSource.volume = volume;
        if (!audioSource.isPlaying) audioSource.Play();
        StartCoroutine(AutoDestroy());
    }

    private IEnumerator AutoDestroy()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        yield return new WaitUntil(() => !audioSource.isPlaying);
        Destroy(gameObject);
    }
}
