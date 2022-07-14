using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    private static Dictionary<string, AudioClip> data = new Dictionary<string, AudioClip>();
    public static void AddItem(AudioClip audio) => data.Add(audio.name, audio);
    public static AudioClip GetItem(string name) => data[name];
}
