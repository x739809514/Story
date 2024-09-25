using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    private GameObject targetObj;
    private Dictionary<string, AudioSource> soundDic;

    public AudioManager(GameObject obj)
    {
        targetObj = obj;
    }

    public void AddTypeMusic(string name, AudioClip clip, bool play = false)
    {
        soundDic ??= new Dictionary<string, AudioSource>();

        var source = targetObj.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.playOnAwake = play;

        soundDic.Add(name, source);
    }

    public void PlaySound(string name)
    {
        if (soundDic.TryGetValue(name, out var sound))
        {
            sound.Play();
        }
        else
        {
            Debug.LogError("Play error, No specific sound");
        }
    }

    public void StopSound(string name)
    {
        if (soundDic.TryGetValue(name, out var sound))
        {
            sound.Stop();
        }
        else
        {
            Debug.LogError("Stop error, No specific sound");
        }
    }
}