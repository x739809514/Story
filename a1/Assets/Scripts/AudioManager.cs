using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    private AudioSource typeSource;
    private GameObject targetObj;

    public AudioManager(GameObject obj)
    {
        targetObj = obj;
    }

    public void AddTypeMusic(AudioClip clip)
    {
        typeSource = targetObj.AddComponent<AudioSource>();
        typeSource.clip = clip;
        typeSource.loop = true;
    }

    public void StopTypeMusic()
    {
        typeSource.Stop();
    }

}