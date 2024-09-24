using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public AudioManager audioManager;
    
    private void Awake()
    {
        Instance=this;
        audioManager = new AudioManager(this.gameObject);
    }
}